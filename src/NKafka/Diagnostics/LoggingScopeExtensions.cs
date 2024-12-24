using Microsoft.Extensions.Logging;

using NKafka.Resources;

namespace NKafka.Diagnostics;

internal static class LoggingScopeExtensions
{
    private const int _MAX_COUNT_PARAMS = 4;

    public static LoggingScope Begin(this ILogger logger, string component)
    {
        return logger.Begin(component, string.Empty);
    }

    public static LoggingScope Begin(this ILogger logger, string component, string param1)
    {
        return logger.Begin(component, param1, string.Empty);
    }

    public static LoggingScope Begin(this ILogger logger, string component, string param1, string param2)
    {
        return logger.Begin(component, param1, param2, string.Empty);
    }

    public static LoggingScope Begin(this ILogger logger, string component, string param1, string param2, string param3)
    {
        if (string.IsNullOrEmpty(component))
        {
            throw new ArgumentNullException(nameof(component), ExceptionMessages.LoggingScopeExtensions_ComponentCannotBeNull);
        }
        var disposables = new List<IDisposable?>(_MAX_COUNT_PARAMS) { logger.BeginScope(component) };

        if (!string.IsNullOrEmpty(param1))
        {
            disposables.Add(logger.BeginScope(param1));
        }

        if (!string.IsNullOrEmpty(param2))
        {
            disposables.Add(logger.BeginScope(param2));
        }

        if (!string.IsNullOrEmpty(param3))
        {
            disposables.Add(logger.BeginScope(param3));
        }

        var result = new LoggingScope([.. disposables]);

        return result;
    }
}