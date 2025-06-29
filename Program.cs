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
            Pathfinder pathfinderUniversal = new Pathfinder(new UniversalLogWriter(new SecureLogWriter(new FileLogWriter())));
        }
    }

    public class Pathfinder
    {
        private ILogger _logger;

        public Pathfinder(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        public void Find(string message) =>
            _logger.WriteError(message);
    }

    public interface ILogger
    {
        void WriteError(string message);
    }

    public interface ISecureLogger : ILogger { }

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

    public class SecureLogWriter : ISecureLogger
    {
        private ILogger _logger;

        public SecureLogWriter(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        public void WriteError(string message)
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                _logger.WriteError(message);
        }
    }

    public class UniversalLogWriter : ConsoleLogWriter
    {
        private ISecureLogger _secureLogger;

        public UniversalLogWriter(ISecureLogger secureLogger)
        {
            if (_secureLogger == null)
                throw new ArgumentNullException(nameof(secureLogger));

            _secureLogger = secureLogger;
        }

        public override void WriteError(string message)
        {
            base.WriteError(message);
            _secureLogger.WriteError(message);
        }
    }
}