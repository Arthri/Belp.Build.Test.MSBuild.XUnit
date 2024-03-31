﻿using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;

namespace Belp.Build.Test.MSBuild.Loggers;

/// <summary>
/// Provides extensions for <see cref="LoggerVerbosity"/>.
/// </summary>
internal static class LoggerVerbosityExtensions
{
    internal static LogLevel ToLogLevel(this LoggerVerbosity loggerVerbosity)
    {
        return loggerVerbosity switch
        {
            LoggerVerbosity.Diagnostic => LogLevel.Trace,
            LoggerVerbosity.Detailed => LogLevel.Debug,
            LoggerVerbosity.Normal => LogLevel.Information,
            LoggerVerbosity.Minimal => LogLevel.Warning,
            LoggerVerbosity.Quiet => LogLevel.None,
            _ => throw new NotSupportedException(),
        };
    }
}
