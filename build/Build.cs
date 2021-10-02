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
using Nuke.Common.CI.TeamCity;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
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

    Target Compile => _ => _
        .Executes(() =>
        {
            const string Esc = "\u001b[";
            const string Reset = "\u001b[0m";

            for (var i = 0; i < 110; i++)
            {
                Console.Write($"{Esc}{i}m{i}{Reset} ");
                Console.Write($"{Esc}{i};1m{i};1{Reset} ");
                Console.Write($"{Esc}{i};2m{i};2{Reset} ");
                if (i % 10 == 0)
                    Console.WriteLine();
            }

            for (var i = 0; i < 250; i++)
            {
                //"\u001B[38;5;0079m"
                Console.Write($"{Esc}38;5;{i}m{i}{Reset} ");
            }

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code)
                .MinimumLevel.Verbose()
                .CreateLogger();

            Variables.OrderBy(x => x.Key).ForEach(x => Console.WriteLine($"{x.Key} = {x.Value}"));

            Logger.Trace("Trace");
            Logger.Normal("Normal");
            Logger.Info("Info");
            Logger.Warn("Warn");
            Logger.Error("Error");

            Log.Verbose("Ah, there you are!{Boolean} {Integer} {String} {@Object}", true, 1, "bluu",
                new { Foo = "bar", Bar = new { Foo = 1, Bar = true } });
            Log.Debug("Ah, there you are!{Boolean} {Integer} {String} {@Object}", true, 1, "bluu",
                new { Foo = "bar", Bar = new { Foo = 1, Bar = true } });
            Log.Information("Ah, there \u001b[36;1m you are!{Boolean} {Integer} {String} {@Object}", true, 1, "bluu",
                new { Foo = "bar", Bar = new { Foo = 1, Bar = true } });
            Log.Warning(new Exception("message"), "Ah, there you are!{Boolean} {Integer} {String} {@Object}", true, 1,
                "bluu", new { Foo = "bar", Bar = new { Foo = 1, Bar = true } });
            Log.Error(new Exception("message"), "Ah, there you are!{Boolean} {Integer} {String} {@Object}", true, 1,
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


    public AnsiConsoleTheme Theme =>
        Host switch
        {
            AzurePipelines => new CustomAnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
            {
                [ConsoleThemeStyle.Text] = "\u001b[36;1m",
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
            _ => new CustomAnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
            {
                [ConsoleThemeStyle.Text] = "",
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
                [ConsoleThemeStyle.LevelInformation] = "\u001b[96m",
                [ConsoleThemeStyle.LevelWarning] = "\u001b[93m",
                [ConsoleThemeStyle.LevelError] = "\u001B[91m",
                [ConsoleThemeStyle.LevelFatal] = "\u001B[101m"
            })
        };

    public class CustomAnsiConsoleTheme : AnsiConsoleTheme
    {
        public CustomAnsiConsoleTheme([NotNull] IReadOnlyDictionary<ConsoleThemeStyle, string> styles) : base(styles)
        {
        }
    }
}