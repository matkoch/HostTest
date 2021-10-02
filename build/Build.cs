using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AppVeyor;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitLab;
using Nuke.Common.CI.TeamCity;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

[CheckBuildProjectConfigurations]
[AppVeyor(
    AppVeyorImage.UbuntuLatest,
    AppVeyorImage.VisualStudioLatest,
    InvokedTargets = new[] { nameof(Compile) })]
[GitHubActions(
    "continuous",
    GitHubActionsImage.MacOsLatest,
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    On = new[] { GitHubActionsTrigger.Push },
    InvokedTargets = new[] { nameof(Compile) })]
[AzurePipelines(
    AzurePipelinesImage.MacOsLatest,
    AzurePipelinesImage.WindowsLatest,
    AzurePipelinesImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Compile) },
    CacheKeyFiles = new string[0])]
[TeamCity(
    VcsTriggeredTargets = new[] { nameof(Compile) })]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Compile);

    Target Variables => _ => _
        .Executes(() =>
        {
            EnvironmentInfo.Variables.OrderBy(x => x.Key).ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));
        });

    Target Compile => _ => _
        .Executes(() =>
        {
            const string Esc = "\u001b[";
            const string Reset = "\u001b[0m";

            for (var i = 0; i < 255; i++)
            {
                Console.Write($"{Esc}38;5;{i}m{i}{Reset}");
                if (i % 50 == 0)
                    Console.WriteLine();
            }

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message}{NewLine}{Exception}",
                    theme: new CustomAnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
                    {
                        [ConsoleThemeStyle.Text] = string.Empty,
                        [ConsoleThemeStyle.SecondaryText] = "\u001B[90m",
                        [ConsoleThemeStyle.TertiaryText] = "\u001B[90m", // timestamp
                        [ConsoleThemeStyle.Name] = "\u001b[34m",
                        [ConsoleThemeStyle.Invalid] = "\u001b[35m",
                        [ConsoleThemeStyle.Null] = "\u001b[33m",
                        [ConsoleThemeStyle.Number] = "\u001b[33m",
                        [ConsoleThemeStyle.String] = "\u001b[33m",
                        [ConsoleThemeStyle.Boolean] = "\u001b[33m",
                        [ConsoleThemeStyle.Scalar] = "\u001b[33m",
                        [ConsoleThemeStyle.LevelVerbose] = "\u001B[38;5;8m",
                        [ConsoleThemeStyle.LevelDebug] = string.Empty,
                        [ConsoleThemeStyle.LevelInformation] = "\u001B[38;5;50m",
                        [ConsoleThemeStyle.LevelWarning] = "\u001B[38;5;214m",
                        [ConsoleThemeStyle.LevelError] = "\u001B[38;5;196m",
                        [ConsoleThemeStyle.LevelFatal] = "\u001B[38;5;231m\u001B[48;5;9m"
                    }),
                    applyThemeToRedirectedOutput: true)
                .MinimumLevel.Verbose()
                .CreateLogger();


            Logger.Trace("Trace");
            Logger.Normal("Normal");
            Logger.Info("Info");
            Logger.Warn("Warn");
            Logger.Error("Error");

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

            Environment.Exit(0);
        });


    public static void Throwing()
    {
        throw new Exception("foo");
    }


    public ConsoleTheme Theme =>
        Host switch
        {
            AzurePipelines => new CustomAnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
            {
                [ConsoleThemeStyle.Text] = string.Empty,
                [ConsoleThemeStyle.SecondaryText] = "\u001B[90m",
                [ConsoleThemeStyle.TertiaryText] = "\u001B[90m", // timestamp
                [ConsoleThemeStyle.Name] = "\u001b[34m",
                [ConsoleThemeStyle.Invalid] = "\u001b[35m",
                [ConsoleThemeStyle.Null] = "\u001b[33m",
                [ConsoleThemeStyle.Number] = "\u001b[33m",
                [ConsoleThemeStyle.String] = "\u001b[33m",
                [ConsoleThemeStyle.Boolean] = "\u001b[33m",
                [ConsoleThemeStyle.Scalar] = "\u001b[33m",
                [ConsoleThemeStyle.LevelVerbose] = "\u001B[90m",
                [ConsoleThemeStyle.LevelDebug] = "\u001B[97m",
                [ConsoleThemeStyle.LevelInformation] = "\u001b[36;1m",
                [ConsoleThemeStyle.LevelWarning] = "\u001b[33;1m",
                [ConsoleThemeStyle.LevelError] = "\u001B[31;1m",
                [ConsoleThemeStyle.LevelFatal] = "\u001B[41;1m"
            }),
            TeamCity => new CustomAnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
            {
                [ConsoleThemeStyle.Text] = string.Empty,
                [ConsoleThemeStyle.SecondaryText] = "\u001B[90m",
                [ConsoleThemeStyle.TertiaryText] = "\u001B[90m", // timestamp
                [ConsoleThemeStyle.Name] = "\u001b[34m",
                [ConsoleThemeStyle.Invalid] = "\u001b[35m",
                [ConsoleThemeStyle.Null] = "\u001b[33m",
                [ConsoleThemeStyle.Number] = "\u001b[33m",
                [ConsoleThemeStyle.String] = "\u001b[33m",
                [ConsoleThemeStyle.Boolean] = "\u001b[33m",
                [ConsoleThemeStyle.Scalar] = "\u001b[33m",
                [ConsoleThemeStyle.LevelVerbose] = "\u001B[90m",
                [ConsoleThemeStyle.LevelDebug] = "\u001B[98m",
                [ConsoleThemeStyle.LevelInformation] = "\u001b[36m",
                [ConsoleThemeStyle.LevelWarning] = "\u001B[38;5;172m",
                [ConsoleThemeStyle.LevelError] = "\u001B[31m",
                [ConsoleThemeStyle.LevelFatal] = "\u001B[41m"
            }),
            GitHubActions => new CustomAnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
            {
                [ConsoleThemeStyle.Text] = string.Empty,
                [ConsoleThemeStyle.SecondaryText] = "\u001B[90m",
                [ConsoleThemeStyle.TertiaryText] = "\u001B[90m", // timestamp
                [ConsoleThemeStyle.Name] = "\u001b[34m",
                [ConsoleThemeStyle.Invalid] = "\u001b[35m",
                [ConsoleThemeStyle.Null] = "\u001b[33m",
                [ConsoleThemeStyle.Number] = "\u001b[33m",
                [ConsoleThemeStyle.String] = "\u001b[33m",
                [ConsoleThemeStyle.Boolean] = "\u001b[33m",
                [ConsoleThemeStyle.Scalar] = "\u001b[33m",
                [ConsoleThemeStyle.LevelVerbose] = "\u001B[90m",
                [ConsoleThemeStyle.LevelDebug] = "\u001B[98m",
                [ConsoleThemeStyle.LevelInformation] = "\u001b[36m",
                [ConsoleThemeStyle.LevelWarning] = "\u001b[33m",
                [ConsoleThemeStyle.LevelError] = "\u001B[31m",
                [ConsoleThemeStyle.LevelFatal] = "\u001B[41m"
            }),
            AppVeyor => new CustomAnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
            {
                [ConsoleThemeStyle.Text] = string.Empty,
                [ConsoleThemeStyle.SecondaryText] = "\u001B[90m",
                [ConsoleThemeStyle.TertiaryText] = "\u001B[90m", // timestamp
                [ConsoleThemeStyle.Name] = "\u001b[34m",
                [ConsoleThemeStyle.Invalid] = "\u001b[35m",
                [ConsoleThemeStyle.Null] = "\u001b[33m",
                [ConsoleThemeStyle.Number] = "\u001b[33m",
                [ConsoleThemeStyle.String] = "\u001b[33m",
                [ConsoleThemeStyle.Boolean] = "\u001b[33m",
                [ConsoleThemeStyle.Scalar] = "\u001b[33m",
                [ConsoleThemeStyle.LevelVerbose] = "\u001B[90m",
                [ConsoleThemeStyle.LevelDebug] = "\u001B[98m",
                [ConsoleThemeStyle.LevelInformation] = "\u001b[36m",
                [ConsoleThemeStyle.LevelWarning] = "\u001b[33m",
                [ConsoleThemeStyle.LevelError] = "\u001B[31m",
                [ConsoleThemeStyle.LevelFatal] = "\u001B[41m"
            }),
            GitLab => new CustomAnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
            {
                [ConsoleThemeStyle.Text] = string.Empty,
                [ConsoleThemeStyle.SecondaryText] = "\u001B[90m",
                [ConsoleThemeStyle.TertiaryText] = "\u001B[90m", // timestamp
                [ConsoleThemeStyle.Name] = "\u001b[34m",
                [ConsoleThemeStyle.Invalid] = "\u001b[35m",
                [ConsoleThemeStyle.Null] = "\u001b[33m",
                [ConsoleThemeStyle.Number] = "\u001b[33m",
                [ConsoleThemeStyle.String] = "\u001b[33m",
                [ConsoleThemeStyle.Boolean] = "\u001b[33m",
                [ConsoleThemeStyle.Scalar] = "\u001b[33m",
                [ConsoleThemeStyle.LevelVerbose] = "\u001B[90m",
                [ConsoleThemeStyle.LevelDebug] = "\u001B[98m",
                [ConsoleThemeStyle.LevelInformation] = "\u001b[36m",
                [ConsoleThemeStyle.LevelWarning] = "\u001b[33m",
                [ConsoleThemeStyle.LevelError] = "\u001B[31m",
                [ConsoleThemeStyle.LevelFatal] = "\u001B[41m"
            }),
            _ =>
                Environment.GetEnvironmentVariable("TERM") is { } term && term.StartsWithOrdinalIgnoreCase("xterm")
                    ? new CustomAnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
                    {
                        [ConsoleThemeStyle.Text] = string.Empty,
                        [ConsoleThemeStyle.SecondaryText] = "\u001B[90m",
                        [ConsoleThemeStyle.TertiaryText] = "\u001B[90m", // timestamp
                        [ConsoleThemeStyle.Name] = "\u001b[34m",
                        [ConsoleThemeStyle.Invalid] = "\u001b[35m",
                        [ConsoleThemeStyle.Null] = "\u001b[33m",
                        [ConsoleThemeStyle.Number] = "\u001b[33m",
                        [ConsoleThemeStyle.String] = "\u001b[33m",
                        [ConsoleThemeStyle.Boolean] = "\u001b[33m",
                        [ConsoleThemeStyle.Scalar] = "\u001b[33m",
                        [ConsoleThemeStyle.LevelVerbose] = "\u001B[90m",
                        [ConsoleThemeStyle.LevelDebug] = string.Empty,
                        [ConsoleThemeStyle.LevelInformation] = "\u001b[96m",
                        [ConsoleThemeStyle.LevelWarning] = "\u001b[93m",
                        [ConsoleThemeStyle.LevelError] = "\u001B[91m",
                        [ConsoleThemeStyle.LevelFatal] = "\u001B[38;5;231m\u001B[48;5;9m"
                    })
                    : new SystemConsoleTheme(new Dictionary<ConsoleThemeStyle, SystemConsoleThemeStyle>
                    {
                        [ConsoleThemeStyle.Text] = new(),
                        [ConsoleThemeStyle.SecondaryText] = new() { Foreground = ConsoleColor.Gray },
                        [ConsoleThemeStyle.TertiaryText] = new() { Foreground = ConsoleColor.Gray },
                        [ConsoleThemeStyle.Name] = new() { Foreground = ConsoleColor.Blue },
                        [ConsoleThemeStyle.Invalid] = new() { Foreground = ConsoleColor.DarkYellow },
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
                        [ConsoleThemeStyle.LevelFatal] = new()
                            { Foreground = ConsoleColor.White, Background = ConsoleColor.Red }
                    })
        };

    public class CustomAnsiConsoleTheme : AnsiConsoleTheme
    {
        public CustomAnsiConsoleTheme([NotNull] IReadOnlyDictionary<ConsoleThemeStyle, string> styles) : base(styles)
        {
        }
    }
}