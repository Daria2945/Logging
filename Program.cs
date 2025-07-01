using System;
using System.Collections.Generic;
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

            List<ILogger> loggers = new List<ILogger>
            {
                new ConsoleLogWriter(),
                new SecureLogWriter(new FileLogWriter())
            };

            Pathfinder pathfinderUniversal = new Pathfinder(new UniversalLogWriter(loggers));
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
        private readonly string _filePath = "log.txt";

        public virtual void WriteError(string message) =>
            File.WriteAllText(_filePath, message);
    }

    public class SecureLogWriter : ILogger
    {
        private readonly DayOfWeek _dayOfWeekToWriteLod = DayOfWeek.Friday;
        private readonly ILogger _logger;

        public SecureLogWriter(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void WriteError(string message)
        {
            if (DateTime.Now.DayOfWeek == _dayOfWeekToWriteLod)
                _logger.WriteError(message);
        }
    }

    public class UniversalLogWriter : ILogger
    {
        private IEnumerable<ILogger> _loggers;

        public UniversalLogWriter(IEnumerable<ILogger> loggers)
        {
            _loggers = loggers ?? throw new ArgumentNullException(nameof(loggers));
        }

        public void WriteError(string message)
        {
            foreach (var logger in _loggers)
                logger.WriteError(message);
        }
    }
}