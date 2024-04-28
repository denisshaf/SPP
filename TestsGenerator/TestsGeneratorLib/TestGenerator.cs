using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks.Dataflow;

namespace TestsGeneratorLib
{
    public static class TestGenerator
    {
        public static Task Generate(string[] files, string outputFolder, int maxThreads)
        {
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            var options = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxThreads };
            var readFileBlock = new TransformBlock<string, string>(
                async filePath => await File.ReadAllTextAsync(filePath),
                options
            );
            var getCompilationUnitBlock = new TransformBlock<string, List<(string OutputPath, CompilationUnitSyntax Unit)>>(
                fileContent => ParseTestFile(fileContent, outputFolder).ToList(),
                options
            );
            var writeToFileBlock = new ActionBlock<List<(string OutputPath, CompilationUnitSyntax Unit)>>(
                async result =>
                {
                    await Parallel.ForEachAsync(
                        result,
                        new ParallelOptions { MaxDegreeOfParallelism = maxThreads },
                        async (r, cancel) =>
                        {
                            await File.WriteAllTextAsync(r.OutputPath, r.Unit.NormalizeWhitespace().ToFullString(), cancel);
                            Console.WriteLine(r.Unit.NormalizeWhitespace().ToFullString());
                        }
                    );
                },
                options
            );

            var dataflowLinkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            readFileBlock.LinkTo(getCompilationUnitBlock, dataflowLinkOptions);
            getCompilationUnitBlock.LinkTo(writeToFileBlock, dataflowLinkOptions);

            foreach (string file in files)
            {
                readFileBlock.SendAsync(file);
            }

            readFileBlock.Complete();
            return writeToFileBlock.Completion;
        }

        public static IEnumerable<(string OutputPath, CompilationUnitSyntax Unit)> ParseTestFile(
            string fileContent,
            string outputFolder
        )
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(fileContent);
            var root = syntaxTree.GetCompilationUnitRoot();

            // Moq and XUnit
            var namespaces = GetRequiredNamespaces(root);

            var testNamespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Tests"));
            var namespaceDeclarations = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().ToList();

            foreach (var namespaceDeclaration in namespaceDeclarations)
            {
                var classDeclarations = namespaceDeclaration.DescendantNodes().OfType<ClassDeclarationSyntax>();

                foreach (var classDeclaration in classDeclarations)
                {
                    bool isAbstract = classDeclaration.Modifiers.Any(modifier =>
                        modifier.IsKind(SyntaxKind.AbstractKeyword)
                    );
                    if (isAbstract)
                    {
                        continue;
                    }
                    var testClassDeclaration = GetTestClassDeclaration(classDeclaration);

                    var testFields = GetTestClassFields(classDeclaration);

                    var testConstructor = GetTestConstructor(classDeclaration, testClassDeclaration);

                    var testMethods = GetTestMethods(classDeclaration, testFields[0]);

                    var currentNamespaceUsing = SyntaxFactory.UsingDirective(
                        SyntaxFactory.ParseName(namespaceDeclaration.Name.ToString())
                    );
                    var compilationUnit = SyntaxFactory
                        .CompilationUnit()
                        .AddUsings([.. namespaces, currentNamespaceUsing])
                        .AddMembers(
                            testNamespaceDeclaration.AddMembers(
                                testClassDeclaration.AddMembers([.. testFields, testConstructor, .. testMethods])
                            )
                        );

                    yield return (
                        $"{outputFolder}{Path.DirectorySeparatorChar}{testClassDeclaration.Identifier.ValueText}.cs",
                        compilationUnit
                    );
                }
            }
        }

        private static List<UsingDirectiveSyntax> GetRequiredNamespaces(CompilationUnitSyntax root)
        {
            var namespaces = root.DescendantNodes().OfType<UsingDirectiveSyntax>().ToList();

            var usingDirectiveXunit = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Xunit"));
            var usingDirectiveMoq = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Moq"));

            namespaces.Add(usingDirectiveXunit);
            namespaces.Add(usingDirectiveMoq);

            return namespaces;
        }

        private static ConstructorDeclarationSyntax GetTestConstructor(
            ClassDeclarationSyntax classDeclaration,
            ClassDeclarationSyntax testClassDeclaration
        )
        {
            var testConstructor = SyntaxFactory
                .ConstructorDeclaration(testClassDeclaration.Identifier)
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(SyntaxFactory.ParameterList())
                .WithBody(SyntaxFactory.Block());

            var constructor = classDeclaration
                .DescendantNodes()
                .OfType<ConstructorDeclarationSyntax>()
                .MinBy(c => c.ParameterList.Parameters.Count);

            if (constructor is not null)
            {
                var parameters = constructor.ParameterList.Parameters;
                var testConstructorBody = parameters.Select(GetDefaultOrMockAssignment).ToList();
                testConstructorBody.Add(GetConstructorCall(classDeclaration));

                testConstructor = testConstructor.WithBody(SyntaxFactory.Block(testConstructorBody));
            }

            return testConstructor;
        }

        private static List<FieldDeclarationSyntax> GetTestClassFields(ClassDeclarationSyntax classDeclaration)
        {
            List<FieldDeclarationSyntax> testFields = [];

            var className = classDeclaration.Identifier.ValueText;

            // class instances
            var classField = SyntaxFactory
                .FieldDeclaration(
                    SyntaxFactory
                        .VariableDeclaration(SyntaxFactory.ParseTypeName(className))
                        .AddVariables(SyntaxFactory.VariableDeclarator($"{className.ToLower()}"))
                )
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
            testFields.Add(classField);
            
            // constructor with minimum number of parameters 
            var constructor = classDeclaration
                .DescendantNodes()
                .OfType<ConstructorDeclarationSyntax>()
                .MinBy(c => c.ParameterList.Parameters.Count);

            if (constructor is not null)
            {
                testFields.AddRange(GetInterfaceFields(constructor));
            }

            return testFields;
        }

        private static ClassDeclarationSyntax GetTestClassDeclaration(ClassDeclarationSyntax classDeclaration)
        {
            string className = classDeclaration.Identifier.ValueText;
            string testClassName = $"Test_{className}";

            return SyntaxFactory.ClassDeclaration(testClassName).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
        }

        private static List<FieldDeclarationSyntax> GetInterfaceFields(ConstructorDeclarationSyntax constructor)
        {
            List<FieldDeclarationSyntax> interfaceFields = [];

            var interfaceParameters = constructor
                .ParameterList.Parameters.Where(parameter =>
                    parameter.Type is SimpleNameSyntax simpleName && simpleName.Identifier.ValueText.StartsWith('I')
                )
                .ToList();

            foreach (var parameter in interfaceParameters)
            {
                var fieldName = $"{parameter.Identifier.ValueText.ToLower()}";
                var fieldType = SyntaxFactory.ParseTypeName($"Mock<{parameter.Type}>");
                var fieldDeclaration = SyntaxFactory
                    .FieldDeclaration(
                        SyntaxFactory
                            .VariableDeclaration(fieldType)
                            .AddVariables(SyntaxFactory.VariableDeclarator(fieldName))
                    )
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

                interfaceFields.Add(fieldDeclaration);
            }

            return interfaceFields;
        }

        private static ExpressionStatementSyntax GetDefaultOrMockAssignment(ParameterSyntax parameter)
        {
            ExpressionSyntax assignmentExpression;

            if (parameter.Type is SimpleNameSyntax simpleName && simpleName.Identifier.ValueText.StartsWith('I'))
            {
                assignmentExpression = SyntaxFactory
                    .ObjectCreationExpression(
                        SyntaxFactory
                            .GenericName(SyntaxFactory.Identifier("Mock"))
                            .WithTypeArgumentList(
                                SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(parameter.Type))
                            )
                    )
                    .WithArgumentList(SyntaxFactory.ArgumentList());

                return SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SyntaxFactory.IdentifierName($"{parameter.Identifier.ValueText.ToLower()}"),
                        assignmentExpression
                    )
                );
            }

            assignmentExpression = SyntaxFactory.DefaultExpression(parameter.Type ?? SyntaxFactory.ParseTypeName("object"));
            return SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName($"var {parameter.Identifier.ValueText.ToLower()}"),
                    assignmentExpression
                )
            );
        }

        private static ExpressionStatementSyntax GetConstructorCall(ClassDeclarationSyntax classDeclaration)
        {
            var constructorParameters = GetConstructor(classDeclaration)?.ParameterList.Parameters ?? [];

            var constructorParameterNames = constructorParameters
                .Select(parameter =>
                {
                    if (parameter.Type is SimpleNameSyntax simpleName && simpleName.Identifier.ValueText.StartsWith('I'))
                    {
                        return SyntaxFactory.IdentifierName($"{parameter.Identifier.ValueText.ToLower()}.Object");
                    }

                    return SyntaxFactory.IdentifierName($"{parameter.Identifier.ValueText.ToLower()}");
                })
                .ToList();

            var constructorArgumentsList = SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList(constructorParameterNames.Select(SyntaxFactory.Argument))
            );

            var constructorInvocation = SyntaxFactory
                .ObjectCreationExpression(SyntaxFactory.IdentifierName(classDeclaration.Identifier))
                .WithArgumentList(constructorArgumentsList);

            var constructorAssignmentExpression = SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName($"{classDeclaration.Identifier.ValueText.ToLower()}"),
                constructorInvocation
            );

            return SyntaxFactory.ExpressionStatement(constructorAssignmentExpression);
        }

        private static ConstructorDeclarationSyntax? GetConstructor(ClassDeclarationSyntax classDeclaration) =>
            classDeclaration
                .DescendantNodes()
                .OfType<ConstructorDeclarationSyntax>()
                .MinBy(c => c.ParameterList.Parameters.Count);

        private static List<MemberDeclarationSyntax> GetTestMethods(
            ClassDeclarationSyntax classDeclaration,
            FieldDeclarationSyntax classField
        )
        {
            List<MemberDeclarationSyntax> testMethods = [];
            var methodDeclarations = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();

            foreach (var methodDeclaration in methodDeclarations)
            {
                var testMethodDeclaration = GetTestMethodDeclaration(methodDeclaration);

                var methodParameters = methodDeclaration.ParameterList.Parameters;
                var methodAssignments = GetMethodAssignments(methodParameters);
                var methodReturnType = methodDeclaration.ReturnType;

                if (!IsVoid(methodReturnType))
                {
                    var actualExpression = GetMethodCallSyntax(
                        "var actual",
                        classField.Declaration.Variables.First().Identifier.ValueText,
                        methodDeclaration.Identifier.ValueText,
                        methodParameters
                    );
                    methodAssignments.Add(actualExpression);

                    var expectedValue = GetDefaultExpression(methodReturnType);
                    var expectedExpression = SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            SyntaxFactory.IdentifierName($"var expected"),
                            expectedValue
                        )
                    );
                    methodAssignments.Add(expectedExpression);

                    var assertEqualStatement = SyntaxFactory.ParseStatement("Assert.Equal(expected, actual);");
                    if (assertEqualStatement is ExpressionStatementSyntax assertEqualExpressionSyntax)
                    {
                        methodAssignments.Add(assertEqualExpressionSyntax);
                    }
                }

                var failStatement = SyntaxFactory.ParseStatement("Assert.Fail(\"autogenerated\");");
                if (failStatement is ExpressionStatementSyntax failExpressionStatement)
                {
                    methodAssignments.Add(failExpressionStatement);
                }

                testMethods.Add(testMethodDeclaration.WithBody(SyntaxFactory.Block(methodAssignments)));
            }

            return testMethods;
        }

        private static ExpressionStatementSyntax GetMethodCallSyntax(
            string variableName,
            string className,
            string methodName,
            SeparatedSyntaxList<ParameterSyntax> parameters
        ) =>
            SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName(variableName),
                    SyntaxFactory
                        .InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName(className),
                                SyntaxFactory.IdentifierName(methodName)
                            )
                        )
                        .WithArgumentList(
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList(
                                    parameters.Select(parameter =>
                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName(parameter.Identifier))
                                    )
                                )
                            )
                        )
                )
            );

        private static DefaultExpressionSyntax GetDefaultExpression(TypeSyntax? type) =>
            SyntaxFactory.DefaultExpression(type ?? SyntaxFactory.ParseTypeName("object"));

        private static MethodDeclarationSyntax GetTestMethodDeclaration(MethodDeclarationSyntax method) =>
            SyntaxFactory
                .MethodDeclaration(SyntaxFactory.ParseTypeName("void"), $"Test_{method.Identifier.ValueText}")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAttributeLists(
                    SyntaxFactory.AttributeList(
                        SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Attribute(SyntaxFactory.ParseName("Fact")))
                    )
                );

        private static List<ExpressionStatementSyntax> GetMethodAssignments(
            SeparatedSyntaxList<ParameterSyntax> parameters
        ) =>
            parameters
                .Select(parameter =>
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            SyntaxFactory.IdentifierName($"var {parameter.Identifier.ValueText}"),
                            GetDefaultExpression(parameter.Type)
                        )
                    )
                )
                .ToList();

        private static bool IsVoid(TypeSyntax type)
        {
            var returnTypeString = type.ToString();
            var voidType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)).Keyword.Text;

            return returnTypeString.Equals(voidType);
        }
    }
}
