using System;
using System.Collections.Generic;
using System.Linq;
using CybersecurityChatbot.Models;

namespace CybersecurityChatbot.Services
{
    public enum Sentiment { Positive, Negative, Neutral }

    public class ResponseService
    {
        private readonly UserProfile _userProfile;
        private readonly List<string> _conversationHistory = new();
        public int MessageCount { get; private set; }

        // Exact commands
        private readonly Dictionary<string, string> _exactCommands = new(StringComparer.OrdinalIgnoreCase)
        {
            { "history", "" },
            { "stats", "" },
            { "help", "Sure! You can ask me about:\n• Passwords & 2FA\n• Phishing & scams\n• Ransomware & malware\n• VPN & privacy\n• Safe browsing\nOr type 'history' to see our chat, 'stats' for your usage." },
            { "what do you do", "I'm here to chat with you about staying safe online – think of me as your friendly cybersecurity buddy." },
            { "who are you", "I'm CyberGuard, but you can call me Guard. I help South Africans avoid online traps." },
            { "what can I ask", "Pretty much anything security-related! Try 'password tips', 'what is phishing?', 'explain 2FA', or just tell me about a problem you're facing." }
        };

        // Keyword responses using traditional dictionary initialization
        private readonly Dictionary<string, List<string>> _keywordResponses = new(StringComparer.OrdinalIgnoreCase)
        {
            { "password", new List<string> {
                "Great question! Think of a password like a key to your house – you wouldn't use the same key for every door, right? Use unique, long passwords (or passphrases) and turn on 2FA wherever you can.",
                "Passwords are the first line of defense. I always recommend a password manager – it remembers all your strong passwords so you don't have to. And never reuse the same one across sites!",
                "The best password is a long passphrase, like 'PurpleDolphin$wims@Night'. Easy to remember, hard to crack. Oh, and change it if you think it's been leaked."
            } },
            { "scam", new List<string> {
                "Scammers are sneaky. They might call, email, or text pretending to be your bank, the police, or even a family member in trouble. Always stop, verify through another channel, and never send money or share OTPs.",
                "If it sounds too good to be true (free iPhone? huge lottery win?), it's almost always a scam. Also, never click links in unexpected messages – go directly to the official website instead.",
                "In South Africa, we see a lot of 'momo' scams and fake job offers. Remember: real companies won't ask for your password or ask you to pay to get a job."
            } },
            { "phish", new List<string> {
                "Phishing is when a fake email or message tries to steal your login details. They often create urgent 'your account will be closed' stories. Hover over links before clicking – if the address looks weird, don't click!",
                "Imagine getting an email from 'Netflix' saying your payment failed. The link goes to a fake login page. Always type the real website address yourself, or use a bookmark.",
                "A good trick: look for spelling mistakes and weird sender addresses. Real companies have proper grammar and their domain matches (e.g., @bankname.co.za, not @bankname-secure.com)."
            } },
            { "ransomware", new List<string> {
                "Ransomware is nasty – it locks your files and demands payment. The best protection? Regular backups (offline or cloud). And never pay the ransom, because they might not unlock anything.",
                "This often comes via email attachments or fake software updates. Keep your antivirus on and think twice before opening unexpected attachments, even from friends (their account might be hacked).",
                "If you get hit, disconnect from the internet immediately and contact a professional. But honestly, preventing it is easier: don't download cracks or click on weird pop-ups."
            } },
            { "2fa", new List<string> {
                "Two-factor authentication (2FA) is like a second lock on your door. After your password, you enter a code from an app (like Google Authenticator) or an SMS. It stops hackers even if they steal your password.",
                "I always tell people: turn on 2FA for your email, social media, and banking. It's a few extra seconds that can save you a ton of trouble.",
                "SMS 2FA is okay, but app-based (TOTP) or hardware keys are safer. Some places even use biometrics like your fingerprint – that's also a form of 2FA."
            } },
            { "vpn", new List<string> {
                "A VPN (Virtual Private Network) creates a secure tunnel for your internet traffic. It's super useful on public Wi‑Fi (like at a café or airport) because it hides your activity from snoopers.",
                "Think of a VPN as a disguise for your computer. It makes it look like you're browsing from somewhere else, and it encrypts everything you send. Just choose a reputable provider – free ones often sell your data.",
                "I recommend using a VPN especially when doing online banking or shopping on public networks. But remember: a VPN isn't a magic shield – it doesn't stop malware or phishing."
            } },
            { "privacy", new List<string> {
                "Privacy is about controlling who sees your info. On social media, review your privacy settings – you don't need to share your birthday or address publicly. Also, be careful with quizzes that ask personal questions (they might be gathering answers to security questions).",
                "Ever noticed how ads seem to follow you? That's tracking. Use browser privacy features, or try a privacy-focused browser like Firefox with uBlock Origin.",
                "In South Africa, the POPIA law gives you rights over your personal data. You can ask companies to delete your info. Also, never post your ID number or utility bills online."
            } },
            { "safe browsing", new List<string> {
                "Stick to trusted websites (look for the padlock icon in the address bar). Avoid clicking on random pop-ups that say 'Your computer is infected' – that's a common trick.",
                "Keep your browser and apps updated. Updates often include security fixes. And use an ad blocker – many malicious ads look like normal buttons.",
                "When you search for something, be careful with sponsored results. Sometimes they lead to fake sites that look real. I usually scroll down to the actual links."
            } },
            { "malware", new List<string> {
                "Malware is short for malicious software – viruses, worms, spyware, etc. It can slow down your computer, steal your files, or spy on you. Always have an antivirus and keep it updated.",
                "Most malware spreads through email attachments, fake downloads, or infected USB sticks. If you're not 100% sure about a file, don't open it. And back up your important stuff separately.",
                "If your computer is acting weird (pop-ups, slowdowns, new toolbars), run a scan with Windows Defender or Malwarebytes. And consider reinstalling your OS if it's really bad."
            } }
        };

        private readonly List<string> _positiveWords = new() { "good", "great", "thanks", "helpful", "love", "awesome", "perfect", "cool", "you're right" };
        private readonly List<string> _negativeWords = new() { "bad", "hate", "scared", "confused", "worried", "attack", "awful", "terrible", "stressed", "nervous" };

        public ResponseService(UserProfile profile)
        {
            _userProfile = profile;
        }

        public (string reply, Sentiment mood) GetResponse(string rawInput)
        {
            MessageCount++;
            string input = rawInput.Trim();
            _conversationHistory.Add(input);

            Sentiment mood = DetectSentiment(input);

            if (IsExit(input))
                return (BuildGoodbye(), mood);

            if (_exactCommands.ContainsKey(input))
            {
                if (input.Equals("history", StringComparison.OrdinalIgnoreCase))
                    return (BuildHistory(), mood);
                if (input.Equals("stats", StringComparison.OrdinalIgnoreCase))
                    return (BuildStats(), mood);
                return (_exactCommands[input], mood);
            }

            if (ContainsAny(input, "what do you do", "what are you", "your purpose", "who are you", "what can I ask", "what can you tell me"))
                return (BuildPurposeResponse(), mood);

            string reply = GetKeywordMatch(input);
            if (reply != null)
                return (ApplySentimentPrefix(mood, reply), mood);

            string fallback = "Hmm, I'm not sure about that one. Could you ask me differently? I know about passwords, scams, phishing, 2FA, VPNs, privacy, and safe browsing. Or just type 'help'.";
            return (ApplySentimentPrefix(mood, fallback), mood);
        }

        private Sentiment DetectSentiment(string input)
        {
            if (_positiveWords.Any(w => input.Contains(w, StringComparison.OrdinalIgnoreCase)))
                return Sentiment.Positive;
            if (_negativeWords.Any(w => input.Contains(w, StringComparison.OrdinalIgnoreCase)))
                return Sentiment.Negative;
            return Sentiment.Neutral;
        }

        private string ApplySentimentPrefix(Sentiment mood, string reply)
        {
            return mood switch
            {
                Sentiment.Positive => $"😊 {reply}",
                Sentiment.Negative => $"I hear you – that can be worrying. {reply}",
                _ => reply
            };
        }

        private string GetKeywordMatch(string input)
        {
            foreach (var kvp in _keywordResponses)
            {
                if (input.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    var random = new Random();
                    int index = random.Next(kvp.Value.Count);
                    return kvp.Value[index];
                }
            }
            return null;
        }

        private bool IsExit(string input)
        {
            var exits = new[] { "exit", "quit", "bye", "goodbye" };
            return exits.Contains(input.ToLower());
        }

        private string BuildGoodbye()
        {
            string name = _userProfile.Recall("name") ?? "friend";
            string[] goodbyes = {
                $"Stay safe online, {name}! Chat again anytime.",
                $"Take care, {name}. Remember: think before you click!",
                $"Bye {name}! If anything suspicious pops up, you know where to find me."
            };
            return goodbyes[new Random().Next(goodbyes.Length)];
        }

        private string BuildHistory()
        {
            if (_conversationHistory.Count == 0)
                return "We haven't chatted yet – go ahead, ask me something!";
            return "Here's our recent chat:\n" + string.Join("\n", _conversationHistory.TakeLast(6).Select((t, i) => $"{i + 1}. {t}"));
        }

        private string BuildStats()
        {
            string name = _userProfile.Recall("name") ?? "Guest";
            return $"📊 Stats for {name}:\n- Messages from you: {MessageCount}\n- Our conversation length: {_conversationHistory.Count} turns\n- Keep asking – knowledge is power!";
        }

        private string BuildPurposeResponse()
        {
            string[] purposeResponses = {
                "I'm your friendly cybersecurity guide. I can help you understand online risks, give tips on staying safe, and answer questions about things like phishing, passwords, or scams. What would you like to know?",
                "Think of me as a chatty security expert. I explain things in plain English – no jargon, no judgement. Want to know about 2FA? Ransomware? Or just general online safety?",
                "My job is to help South Africans like you avoid getting hacked or scammed. Ask me anything like 'how to create a strong password' or 'what is phishing?' – I'll give you a straight answer."
            };
            return purposeResponses[new Random().Next(purposeResponses.Length)];
        }

        private bool ContainsAny(string input, params string[] phrases)
        {
            return phrases.Any(p => input.Contains(p, StringComparison.OrdinalIgnoreCase));
        }
    }
}