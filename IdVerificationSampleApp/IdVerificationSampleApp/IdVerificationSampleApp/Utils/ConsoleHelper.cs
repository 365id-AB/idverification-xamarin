using System;

namespace IdVerificationSampleApp.Utils
{
    public static class ConsoleHelper
    {
        public static void Print(string message)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[{Constants.AppTitle}] -> {message}");
#else
            System.Console.WriteLine($"[{Constants.AppTitle}] -> {message}");
#endif
        }
    }
}