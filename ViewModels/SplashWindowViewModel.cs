using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using CybersecurityChatbot.Commands;

namespace CybersecurityChatbot.ViewModels
{
    public class SplashWindowViewModel : INotifyPropertyChanged
    {
        private readonly Window _splashWindow;
        private string _userName;

        public string UserName
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(); }
        }

        public ICommand LetGoCommand { get; }

        public SplashWindowViewModel(Window splashWindow)
        {
            _splashWindow = splashWindow;
            LetGoCommand = new RelayCommand(OnLetGo, CanLetGo);
        }

        private bool CanLetGo() => !string.IsNullOrWhiteSpace(UserName);

        private void OnLetGo()
        {
            var mainWindow = new Views.MainWindow(UserName);
            mainWindow.Show();
            _splashWindow.Close();   // close the splash window
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}