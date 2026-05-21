using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CybersecurityChatbot.Models;
using CybersecurityChatbot.Services;
using CybersecurityChatbot.Commands;

namespace CybersecurityChatbot.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ResponseService _responseService;
        private readonly UserProfile _userProfile;
        private string _userInput;
        private string _statusMessage;

        public ObservableCollection<ChatMessage> Messages { get; }
        public ICommand SendCommand { get; }

        public string UserInput
        {
            get => _userInput;
            set { _userInput = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public MainWindowViewModel(string userName)
        {
            _userProfile = new UserProfile();
            _userProfile.Remember("name", userName);
            _responseService = new ResponseService(_userProfile);
            Messages = new ObservableCollection<ChatMessage>();
            SendCommand = new RelayCommand(SendMessage, () => !string.IsNullOrWhiteSpace(UserInput));

            // Welcome message
            AddBotMessage($"Hello {userName}! I'm your Cybersecurity Assistant. Ask me about passwords, scams, phishing, or type 'help'.");
        }

        private void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(UserInput)) return;

            string input = UserInput.Trim();
            AddUserMessage(input);

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                AddBotMessage("Stay safe online! Goodbye.");
                System.Windows.Application.Current.Shutdown();
                return;
            }

            var (reply, mood) = _responseService.GetResponse(input);
            AddBotMessage(reply);
            StatusMessage = $"Mood: {mood}  |  Messages sent: {_responseService.MessageCount}";
            UserInput = "";
        }

        private void AddUserMessage(string text) => Messages.Add(new ChatMessage("User", text));
        private void AddBotMessage(string text) => Messages.Add(new ChatMessage("Bot", text));

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}