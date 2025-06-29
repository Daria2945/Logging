using System;
using System.IO;

namespace Logging
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Pathfinder pathfinderConsole = new Pathfinder(new ConsoleLogWriter());
            Pathfinder pathfinderFile = new Pathfinder(new FileLogWriter());
            Pathfinder pathfinderSecureConsole = new Pathfinder(new SecureLogWriter(new ConsoleLogWriter()));
            Pathfinder pathfinderSecureFile = new Pathfinder(new SecureLogWriter(new FileLogWriter()));
            Pathfinder pathfinderUniversal = new Pathfinder(new UniversalLogWriter(new ConsoleLogWriter(), new SecureLogWriter(new FileLogWriter())));
        }
    }

    public class Pathfinder
    {
        private readonly ILogger _logger;

        public Pathfinder(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Find(string message) =>
            _logger.WriteError(message);
    }

    public interface ILogger
    {
        void WriteError(string message);
    }

    public class ConsoleLogWriter : ILogger
    {
        public virtual void WriteError(string message) =>
            Console.WriteLine(message);
    }

    public class FileLogWriter : ILogger
    {
        public virtual void WriteError(string message) =>
            File.WriteAllText("log.txt", message);
    }

    public class SecureLogWriter : ILogger
    {
        private readonly ILogger _logger;

        public SecureLogWriter(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void WriteError(string message)
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                _logger.WriteError(message);
        }
    }

    public class UniversalLogWriter : ILogger
    {
        private readonly ILogger _basicLogger;
        private readonly ILogger _additionalSecureLogger;

        public UniversalLogWriter(ILogger basicLogger, ILogger additionalSecureLogger)
        {
            _basicLogger = basicLogger ?? throw new ArgumentNullException(nameof(basicLogger));
            _additionalSecureLogger = additionalSecureLogger ?? throw new ArgumentNullException(nameof(additionalSecureLogger));
        }

        public void WriteError(string message)
        {
            _basicLogger.WriteError(message);
            _additionalSecureLogger.WriteError(message);
        }
    }
}