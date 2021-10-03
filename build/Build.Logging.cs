using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AppVeyor;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitLab;
using Nuke.Common.CI.TeamCity;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.SystemConsole.Themes;
using Logger = Nuke.Common.Logger;

partial class Build
{
    Target Compile => _ => _
        .DependsOn(Colors, Variables)
        .Executes(() =>
        {
            ExtendedTheme.WriteNormal("Normal");
            ExtendedTheme.WriteSuccess("Info");
            ExtendedTheme.WriteWarning("Warn");
            ExtendedTheme.WriteError("Error");

            Log.Verbose("Ah, there you are!{Boolean} {Integer} {String} {@Object}", true, 1, "bluu",
                new { Foo = "bar", Bar = new { Foo = 1, Bar = true } });
            Log.Debug("Ah, there you are!{Boolean} {Integer} {String} {@Object}", true, 1, "bluu",
                new { Foo = "bar", Bar = new { Foo = 1, Bar = true } });
            Log.Information("Ah, there you are!{Boolean} {Integer} {String} {@Object}", true, 1, "bluu",
                new { Foo = "bar", Bar = new { Foo = 1, Bar = true } });
            Log.Warning(new Exception("message"), "Ah, there you are!{Boolean} {Integer} {String} {@Object}", true, 1,
                "bluu", new { Foo = "bar", Bar = new { Foo = 1, Bar = true } });
            Log.Error(new Exception("message"), "Ah, there you are!{Boolean} {Integer} {String} {@Object}", true, 1,
                "bluu", new { Foo = "bar", Bar = new { Foo = 1, Bar = true } });
            Log.Fatal(new Exception("message"), "Ah, there you are!{Boolean} {Integer} {String} {@Object}", true, 1,
                "bluu", new { Foo = "bar", Bar = new { Foo = 1, Bar = true } });

            try
            {
                var methodInfo = GetType().GetMethod(nameof(Throwing), BindingFlags.Static | BindingFlags.Public);
                methodInfo.Invoke(null, new object[0]);
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Empty);
            }
        });

    public static ConsoleTheme DefaultAnsi256ColorTheme => new ExtendedAnsiConsoleTheme(
        "\u001B[92;1m",
        new Dictionary<ConsoleThemeStyle, string>
        {
            [ConsoleThemeStyle.Text] = "\u001B[39m",
            [ConsoleThemeStyle.SecondaryText] = "\u001B[38;5;247m",
            [ConsoleThemeStyle.TertiaryText] = "\u001B[38;5;247m",
            [ConsoleThemeStyle.Name] = "\u001B[39;1m",
            [ConsoleThemeStyle.Invalid] = "\u001b[38;5;207m",
            [ConsoleThemeStyle.Null] = "\u001b[38;5;45m",
            [ConsoleThemeStyle.Number] = "\u001b[38;5;45m",
            [ConsoleThemeStyle.String] = "\u001b[38;5;45m",
            [ConsoleThemeStyle.Boolean] = "\u001b[38;5;45m",
            [ConsoleThemeStyle.Scalar] = "\u001b[38;5;45m",
            [ConsoleThemeStyle.LevelVerbose] = "\u001B[90;1m",
            [ConsoleThemeStyle.LevelDebug] = "\u001B[39;1m",
            [ConsoleThemeStyle.LevelInformation] = "\u001B[38;5;50;1m",
            [ConsoleThemeStyle.LevelWarning] = "\u001B[38;5;214;1m",
            [ConsoleThemeStyle.LevelError] = "\u001B[38;5;196;1m",
            [ConsoleThemeStyle.LevelFatal] = "\u001B[38;5;231;1m\u001B[48;5;196m"
        });

    public static ConsoleTheme DefaultSystemColorTheme => new ExtendedSystemConsoleTheme(
        new SystemConsoleThemeStyle { Foreground = ConsoleColor.Green },
        new Dictionary<ConsoleThemeStyle, SystemConsoleThemeStyle>
        {
            [ConsoleThemeStyle.Text] = new(),
            [ConsoleThemeStyle.SecondaryText] = new() { Foreground = ConsoleColor.Gray },
            [ConsoleThemeStyle.TertiaryText] = new() { Foreground = ConsoleColor.Gray },
            [ConsoleThemeStyle.Name] = new() { Foreground = ConsoleColor.Blue },
            [ConsoleThemeStyle.Invalid] = new() { Foreground = ConsoleColor.DarkRed },
            [ConsoleThemeStyle.Null] = new() { Foreground = ConsoleColor.Magenta },
            [ConsoleThemeStyle.String] = new() { Foreground = ConsoleColor.Magenta },
            [ConsoleThemeStyle.Number] = new() { Foreground = ConsoleColor.Magenta },
            [ConsoleThemeStyle.Boolean] = new() { Foreground = ConsoleColor.Magenta },
            [ConsoleThemeStyle.Scalar] = new() { Foreground = ConsoleColor.Magenta },
            [ConsoleThemeStyle.LevelVerbose] = new() { Foreground = ConsoleColor.Gray },
            [ConsoleThemeStyle.LevelDebug] = new(),
            [ConsoleThemeStyle.LevelInformation] = new() { Foreground = ConsoleColor.Cyan },
            [ConsoleThemeStyle.LevelWarning] = new() { Foreground = ConsoleColor.Yellow },
            [ConsoleThemeStyle.LevelError] = new() { Foreground = ConsoleColor.Red },
            [ConsoleThemeStyle.LevelFatal] = new() { Foreground = ConsoleColor.White, Background = ConsoleColor.Red }
        });

    public static ConsoleTheme AppVeyorColorTheme => new ExtendedAnsiConsoleTheme(
        "\u001B[92;1m",
        new Dictionary<ConsoleThemeStyle, string>
        {
            [ConsoleThemeStyle.Text] = string.Empty,
            [ConsoleThemeStyle.SecondaryText] = "\u001B[37;2m",
            [ConsoleThemeStyle.TertiaryText] = "\u001B[37;2m", // timestamp
            [ConsoleThemeStyle.Name] = "\u001b[37;1m",
            [ConsoleThemeStyle.Invalid] = "\u001b[95;1m",
            [ConsoleThemeStyle.Null] = "\u001b[34;1m",
            [ConsoleThemeStyle.Number] = "\u001b[34;1m",
            [ConsoleThemeStyle.String] = "\u001b[34;1m",
            [ConsoleThemeStyle.Boolean] = "\u001b[34;1m",
            [ConsoleThemeStyle.Scalar] = "\u001b[34;1m",
            [ConsoleThemeStyle.LevelVerbose] = "\u001B[37;2m",
            [ConsoleThemeStyle.LevelDebug] = "\u001B[98;1m",
            [ConsoleThemeStyle.LevelInformation] = "\u001b[36;1m",
            [ConsoleThemeStyle.LevelWarning] = "\u001b[33;1m",
            [ConsoleThemeStyle.LevelError] = "\u001B[31;1m",
            [ConsoleThemeStyle.LevelFatal] = "\u001B[41;1m"
        });

    public static ConsoleTheme AzurePipelinesColorTheme => new ExtendedAnsiConsoleTheme(
        "\u001B[32;1m",
        new Dictionary<ConsoleThemeStyle, string>
        {
            [ConsoleThemeStyle.Text] = string.Empty,
            [ConsoleThemeStyle.SecondaryText] = "\u001B[90m",
            [ConsoleThemeStyle.TertiaryText] = "\u001B[90m", // timestamp
            [ConsoleThemeStyle.Name] = "\u001b[37;1m",
            [ConsoleThemeStyle.Invalid] = "\u001b[91;1m",
            [ConsoleThemeStyle.Null] = "\u001b[34;1m",
            [ConsoleThemeStyle.Number] = "\u001b[34;1m",
            [ConsoleThemeStyle.String] = "\u001b[34;1m",
            [ConsoleThemeStyle.Boolean] = "\u001b[34;1m",
            [ConsoleThemeStyle.Scalar] = "\u001b[34;1m",
            [ConsoleThemeStyle.LevelVerbose] = "\u001B[90m",
            [ConsoleThemeStyle.LevelDebug] = "\u001B[97m",
            [ConsoleThemeStyle.LevelInformation] = "\u001b[36;1m",
            [ConsoleThemeStyle.LevelWarning] = "\u001b[33;1m",
            [ConsoleThemeStyle.LevelError] = "\u001B[31;1m",
            [ConsoleThemeStyle.LevelFatal] = "\u001B[41;1m"
        });

    public static void Throwing()
    {
        throw new Exception("foo");
    }

    public static ConsoleTheme Theme =>
        Host switch
        {
            AzurePipelines => AzurePipelinesColorTheme,
            AppVeyor => AppVeyorColorTheme,
            GitLab => DefaultAnsi256ColorTheme,
            TeamCity => DefaultAnsi256ColorTheme,
            GitHubActions => DefaultAnsi256ColorTheme,
            _ => Environment.GetEnvironmentVariable("TERM") is { } term && term.StartsWithOrdinalIgnoreCase("xterm")
                ? DefaultAnsi256ColorTheme
                : DefaultSystemColorTheme
        };

    public static IExtendedColorTheme ExtendedTheme => (IExtendedColorTheme)Theme;

    public static string OutputTemplate =>
        Host switch
        {
            TeamCity => "[{Level:u3}] {Message:l}{NewLine}{Exception}",
            _ => "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:l}{NewLine}{Exception}"
        };

    public interface IExtendedColorTheme
    {
        void WriteNormal(string text);
        void WriteSuccess(string text);
        void WriteWarning(string text);
        void WriteError(string text);
    }

    public class ExtendedSystemConsoleTheme : SystemConsoleTheme, IExtendedColorTheme
    {
        readonly SystemConsoleThemeStyle _successStyle;
        readonly IReadOnlyDictionary<ConsoleThemeStyle, SystemConsoleThemeStyle> _styles;

        public ExtendedSystemConsoleTheme(
            SystemConsoleThemeStyle successStyle,
            IReadOnlyDictionary<ConsoleThemeStyle, SystemConsoleThemeStyle> styles)
            : base(styles)
        {
            _successStyle = successStyle;
            _styles = styles;
        }

        public void WriteNormal(string text)
        {
            Write(text, _styles[ConsoleThemeStyle.LevelDebug]);
        }

        public void WriteSuccess(string text)
        {
            Write(text, _successStyle);
        }

        public void WriteWarning(string text)
        {
            Write(text, _styles[ConsoleThemeStyle.LevelWarning]);
        }

        public void WriteError(string text)
        {
            Write(text, _styles[ConsoleThemeStyle.LevelError]);
        }

        private void Write(string text, SystemConsoleThemeStyle style)
        {
            var previousForeground = Console.ForegroundColor;
            var previousBackground = Console.BackgroundColor;

            using (DelegateDisposable.CreateBracket(
                () =>
                {
                    Console.ForegroundColor = style.Foreground ?? previousForeground;
                    Console.ForegroundColor = style.Background ?? previousBackground;
                },
                () =>
                {
                    Console.ForegroundColor = previousForeground;
                    Console.BackgroundColor = previousBackground;
                }))
            {
                Console.WriteLine(text);
            }
        }
    }

    public class ExtendedAnsiConsoleTheme : AnsiConsoleTheme, IExtendedColorTheme
    {
        readonly string _successCode;
        readonly IReadOnlyDictionary<ConsoleThemeStyle, string> _styles;
        private const string AnsiStyleReset = "\u001B[0m";

        public ExtendedAnsiConsoleTheme(
            string successCode,
            IReadOnlyDictionary<ConsoleThemeStyle, string> styles)
            : base(styles)
        {
            _successCode = successCode;
            _styles = styles;
        }

        public void WriteNormal(string text)
        {
            Write(text, _styles[ConsoleThemeStyle.LevelDebug]);
        }

        public void WriteSuccess(string text)
        {
            Write(text, _successCode);
        }

        public void WriteWarning(string text)
        {
            Write(text, _styles[ConsoleThemeStyle.LevelWarning]);
        }

        public void WriteError(string text)
        {
            Write(text, _styles[ConsoleThemeStyle.LevelError]);
        }

        private void Write(string text, string code)
        {
            Console.WriteLine($"{code}{text}{AnsiStyleReset}");
        }
    }

    public class ConfigureLoggingAttribute : BuildExtensionAttributeBase, IOnTargetRunning, IOnBuildFinished
    {
        private static string Target;
        private static List<string> HighSeverityLogEvents = new();

        public ConfigureLoggingAttribute()
        {
            var configurationId = EnvironmentInfo.GetParameter<string>(BuildServerConfigurationGenerationAttributeBase.ConfigurationParameterName);
            if (configurationId != null)
                return;

            TemporaryDirectory.GlobFiles("build.*.log").OrderByDescending(x => x.ToString()).Skip(5)
                .ForEach(FileSystemTasks.DeleteFile);

            var loggingLevelSwitch = new LoggingLevelSwitch(LogEventLevel.Debug);
            var buildLogFile = TemporaryDirectory / "build.log";
            if (File.Exists(buildLogFile))
            {
                using var filestream = File.OpenWrite(buildLogFile);
                filestream.SetLength(0);
            }

            Log.Logger = new LoggerConfiguration()
                .Enrich.With<TargetEnricher>()
                .WriteTo.Console(
                    outputTemplate: OutputTemplate,
                    theme: Theme,
                    applyThemeToRedirectedOutput: true,
                    levelSwitch: loggingLevelSwitch)
                .WriteTo.File(
                    path: buildLogFile,
                    outputTemplate:
                    "{Timestamp:HH:mm:ss.fff} | {Level:u1} | {Target,15} | {Message:l}{NewLine}{@Exception}")
                .WriteTo.File(
                    path: Path.ChangeExtension(buildLogFile, $".{DateTime.Now:s}.log"),
                    outputTemplate:
                    "{Level:u1} | {Target,15} | {Message:l}{NewLine}{@Exception}")
                // .WriteTo.DelegatingTextSink(
                //     x => HighSeverityLogEvents.Add(x),
                //     outputTemplate: "{Target} ({Level:u3}): {Message}",
                //     restrictedToMinimumLevel: LogEventLevel.Warning)
                .WriteTo.InMemory(
                    outputTemplate: "{Target} ({Level:u3}): {Message}",
                    restrictedToMinimumLevel: LogEventLevel.Warning)
                .MinimumLevel.Verbose()
                .CreateLogger();
        }

        public void OnTargetRunning(NukeBuild build, ExecutableTarget target)
        {
            Target = target.Name;
        }


        public void OnBuildFinished(NukeBuild build)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(
                    outputTemplate: "[{Level:u3}] {Target}: {Message:l}{NewLine}",
                    theme: Theme,
                    applyThemeToRedirectedOutput: true)
                .CreateLogger();

            var nonEmptyLogEvents = InMemorySink.Instance.LogEvents.Where(x => !x.MessageTemplate.Text.IsNullOrEmpty());
            foreach (var logEvent in nonEmptyLogEvents)
                Log.Write(logEvent);
        }

        private class TargetEnricher : ILogEventEnricher
        {
            private LogEventProperty _lastValue;

            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                if (_lastValue == null || (string)((ScalarValue)_lastValue.Value).Value != Target)
                    _lastValue = new LogEventProperty(nameof(Target), new ScalarValue(Target));

                logEvent.AddPropertyIfAbsent(_lastValue);
            }
        }
    }
}