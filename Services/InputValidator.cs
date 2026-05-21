using System;

namespace CybersecurityChatbot.Services;

public static class InputValidator
{
    public static bool IsValidQuestion(string input)
    {
        return !string.IsNullOrWhiteSpace(input);
    }

    public static bool IsExitCommand(string input)
    {
        var exitWords = new[] { "exit", "quit", "bye" };
        return Array.Exists(exitWords, w => w.Equals(input?.Trim().ToLower()));
    }
}