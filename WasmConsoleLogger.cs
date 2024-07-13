using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChemKit;

public class ConsoleLogger : ILogger
{
    private readonly string _name;

    public ConsoleLogger(string name)
    {
        _name = name;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (formatter == null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        var message = formatter(state, exception);
        if (!string.IsNullOrEmpty(message))
        {
            Console.WriteLine($"{logLevel}: {_name} - {message}");
            if (exception != null)
            {
                Console.WriteLine(exception.ToString());
            }
        }
    }
}

public class ConsoleLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, ConsoleLogger> _loggers = new ConcurrentDictionary<string, ConsoleLogger>();

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new ConsoleLogger(name));
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}

public static class ConsoleLoggerExtensions
{
    public static ILoggingBuilder AddConsoleLogger(this ILoggingBuilder builder)
    {
        builder.AddProvider(new ConsoleLoggerProvider());
        return builder;
    }
}