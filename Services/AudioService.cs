using System;
using System.IO;
using System.Media;

namespace CybersecurityChatbot.Services
{
    public static class AudioService
    {
        public static void PlayGreeting()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "greeting.wav");
                if (File.Exists(path))
                {
                    using var player = new SoundPlayer(path);
                    player.PlaySync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("greeting.wav not found in Assets folder");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Audio error: {ex.Message}");
            }
        }
    }
}