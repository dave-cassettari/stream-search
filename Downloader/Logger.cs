using System;
using System.Diagnostics;

namespace StreamSearch.Downloader
{
    public static class Logger
    {
        public static void Log(string format, params object[] args)
        {
            Debug.WriteLine(format, args);
            Console.WriteLine(format, args);
        }
    }
}
