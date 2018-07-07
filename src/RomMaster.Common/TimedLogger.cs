namespace RomMaster.Common
{
    using System;
    using Microsoft.Extensions.Logging;

    public class TimedLogger<T> : ILogger<T>
    {
        private readonly ILogger logger;

        public TimedLogger(ILogger logger) => this.logger = logger;

        public TimedLogger(ILoggerFactory loggerFactory) 
            : this(new Logger<T>(loggerFactory))
        {
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) =>
            logger.Log(logLevel, eventId, state, exception, (s, ex) => $"[{DateTime.UtcNow:HH:mm:ss.fff}]: {formatter(s, ex)}");

        public bool IsEnabled(LogLevel logLevel) => logger.IsEnabled(logLevel);

        public IDisposable BeginScope<TState>(TState state) => logger.BeginScope(state);
    }
}
