using System.Collections.Generic;

namespace CybersecurityChatbot.Models
{
    public class UserProfile
    {
        public string Name { get; set; }
        public Dictionary<string, string> Memory { get; set; }

        public UserProfile()
        {
            Memory = new Dictionary<string, string>();
            Name = "";  // avoid CS8618
        }

        public void Remember(string key, string value)
        {
            if (Memory.ContainsKey(key))
                Memory[key] = value;
            else
                Memory.Add(key, value);
        }

        public string Recall(string key)
        {
            return Memory.TryGetValue(key, out var val) ? val : null;
        }

        public string GetMemorySummary()
        {
            return Memory.Count == 0 ? "Nothing yet" : string.Join(", ", Memory.Keys);
        }
    }
}