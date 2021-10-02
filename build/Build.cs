using System;
using System.Collections.Generic;
using System.Linq;
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

            for (var i = 0; i < 200; i++)
            {
                Console.Write($"{Esc}{i}m{i}{Reset} ");
                Console.Write($"{Esc}{i};1m{i};1{Reset} ");
                Console.Write($"{Esc}{i};2m{i};1{Reset} ");
                Console.Write($"{Esc}{i};3m{i};1{Reset} ");
                Console.Write($"{Esc}{i};4m{i};1{Reset} ");
                Console.Write($"{Esc}{i};5m{i};1{Reset} ");
                if (i % 10 == 0)
                    Console.WriteLine();
            }

            for (var i = 0; i < 250; i++)
            {//"\u001B[38;5;0079m"
                Console.Write($"{Esc}38;5;{i}m{i}{Reset} ");
            }

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss}|{Level:u3}] {Message}{NewLine}{Exception}", theme: Theme)
                .MinimumLevel.Verbose()
                .CreateLogger();

            Logger.Trace("Trace");
            Logger.Normal("Normal");
            Logger.Info("Info");
            Logger.Warn("Warn");
            Logger.Error("Error");

            Log.Verbose("Verbose");
            Log.Debug("Debug");
            Log.Information("Information");
            Log.Warning(new Exception("message"), "warning");
            Log.Error(new Exception("message"), "error");
        });

    public AnsiConsoleTheme Theme =>
        Host switch
        {
            _ => new AnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
            {
                [ConsoleThemeStyle.Text] = "\u001b[96;1m",
                [ConsoleThemeStyle.SecondaryText] = "\u001B[38;5;0246m",
                [ConsoleThemeStyle.TertiaryText] = "\u001B[38;5;0242m",
                [ConsoleThemeStyle.Invalid] = "\u001B[33;1m",
                [ConsoleThemeStyle.Null] = "\u001B[38;5;0038m",
                [ConsoleThemeStyle.Name] = "\u001B[38;5;0081m",
                [ConsoleThemeStyle.String] = "\u001B[38;5;0216m",
                [ConsoleThemeStyle.Number] = "\u001B[38;5;151m",
                [ConsoleThemeStyle.Boolean] = "\u001B[38;5;0038m",
                [ConsoleThemeStyle.Scalar] = "\u001B[38;5;0079m",
                [ConsoleThemeStyle.LevelVerbose] = "\u001B[37m",
                [ConsoleThemeStyle.LevelDebug] = "\u001B[37m",
                [ConsoleThemeStyle.LevelInformation] = "\u001b[96;1m",
                [ConsoleThemeStyle.LevelWarning] = "\u001b[93;1m",
                [ConsoleThemeStyle.LevelError] = "\u001B[38;5;0197m\u001B[48;5;0238m",
                [ConsoleThemeStyle.LevelFatal] = "\u001B[38;5;0197m\u001B[48;5;0238m"
            })
        };
}