using AssemblyBrowserLibrary;
using AssemblyBrowserLibrary.Elements;
using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace AssemblyBrowserWindow
{
    internal class ViewModel : INotifyPropertyChanged
    {
        private readonly Browser assemblyBrowser = new();
        public RelayCommand SearchCommand { get; }

        public RelayCommand BuildCommand { get; }

        private string _path;
        public string Path
        {
            get => _path;
            set
            {               
               _path = value;
               OnPropertyChanged(nameof(Path));
            }
        }

        private Element _tree;
        public Element Tree
        {
            get { return _tree; }
            set
            {
                _tree = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public ViewModel()
        {
            SearchCommand = new RelayCommand(obj =>
            {
                var openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)                        
                    Path = openFileDialog.FileName;
            });

            BuildCommand = new RelayCommand(obj =>
            {
                Tree = assemblyBrowser.GetTreeHead(Path);
            });

            Path = "path";
        }
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
