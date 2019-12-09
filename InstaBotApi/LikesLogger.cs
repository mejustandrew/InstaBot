using System;
using System.IO;

namespace InstaBotApi
{
    static class LikesLogger
    {
        private const string LogPath = "Log.txt";
        private static int _likesGiven;

        public static void LogStartOfExecution()
        {
            var logStartText = File.Exists(LogPath) ? Environment.NewLine : "";
            logStartText += "Bot Run Started - " + DateTime.Now;
            File.AppendAllText(LogPath, logStartText);
        }

        public static void LogLikeGiven()
        {
            _likesGiven++;
        }

        public static void LogStopOfExecution()
        {
            var logStopText = Environment.NewLine + "Bot Run Stopped - " + DateTime.Now;
            logStopText += Environment.NewLine + "Likes given: " + _likesGiven;
            File.AppendAllText(LogPath, logStopText);
            _likesGiven = 0;
        }
    }
}
