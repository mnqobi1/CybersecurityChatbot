using System.Windows;
using CybersecurityChatbot.ViewModels;

namespace CybersecurityChatbot.Views
{
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();      // ← was missing
            DataContext = new SplashWindowViewModel(this); // pass reference to close itself
        }
    }
}