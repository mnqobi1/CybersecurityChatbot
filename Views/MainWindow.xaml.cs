using CybersecurityChatbot.Services;
using CybersecurityChatbot.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CybersecurityChatbot.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        public MainWindow(string userName)
        {
            NewMethod();
            _viewModel = new MainWindowViewModel(userName);
            DataContext = _viewModel;

            // Play voice greeting when window loads
            Loaded += (s, e) => AudioService.PlayGreeting();

            void NewMethod()
            {
                InitializeComponent();
            }
        }

        private void UserInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && _viewModel.SendCommand.CanExecute(null))
                _viewModel.SendCommand.Execute(null);
        }

        private void PromptButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag?.ToString() is string question)
            {
                _viewModel.UserInput = question;
                if (_viewModel.SendCommand.CanExecute(null))
                    _viewModel.SendCommand.Execute(null);
            }
        }
    }
}