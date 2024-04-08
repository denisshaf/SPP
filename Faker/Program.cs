using FakerProj.Entities;
using FakerProj.FakerLib;


var config = new FakerConfig();
config.Add<B, string, MyGenerator>(b => b.Name);
var faker = new Faker(config);

faker.AddGeneratorWithPlugin("D:\\Study\\СПП\\Лабы\\Faker\\Extensions\\UriGenerator.dll");
faker.AddGeneratorWithPlugin("D:\\Study\\СПП\\Лабы\\Faker\\Extensions\\BoolGenerator.dll");

Console.WriteLine(faker.Create<TestDTOClass>());