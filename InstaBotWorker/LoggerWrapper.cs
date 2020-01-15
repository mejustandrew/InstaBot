using Serilog;

namespace InstaBotWorker
{
    class LoggerWrapper : InstaBotApi.ILogger
    {
        public void Error(string message)
        {
            Log.Error(message);
        }

        public void Information(string message)
        {
            Log.Information(message);
        }

        public void Warning(string message)
        {
            Log.Warning(message);
        }
    }
}
