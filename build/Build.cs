using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.CI.AppVeyor;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitLab;
using Nuke.Common.CI.TeamCity;
using Nuke.Common.Execution;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

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
    NonEntryTargets = new[] { nameof(Variables), nameof(Colors) },
    CacheKeyFiles = new string[0])]
[TeamCity(
    VcsTriggeredTargets = new[] { nameof(Compile) },
    NonEntryTargets = new[] { nameof(Variables), nameof(Colors) })]
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

    private string GetAnsiCode(params string[] codes)
    {
        return $"\u001b[{codes.Join(";")}m";
    }

    Target Colors => _ => _
        .Executes(() =>
        {
            const string Esc = "\u001b[";
            const string Reset = "\u001b[0m";

            for (var i = 30; i < 47; i++)
                Console.Write($"{Esc}{i}m{i}  {Reset} ");
            for (var i = 90; i < 107; i++)
                Console.Write($"{Esc}{i}m{i}  {Reset} ");
            Console.WriteLine();
            for (var i = 30; i < 47; i++)
                Console.Write($"{Esc}{i};1m{i};1{Reset} ");
            for (var i = 90; i < 107; i++)
                Console.Write($"{Esc}{i};1m{i};1{Reset} ");
            Console.WriteLine();
            for (var i = 30; i < 47; i++)
                Console.Write($"{Esc}{i};2m{i};2{Reset} ");
            for (var i = 90; i < 107; i++)
                Console.Write($"{Esc}{i};2m{i};2{Reset} ");
            Console.WriteLine();
            for (var i = 30; i < 47; i++)
                Console.Write($"{Esc}{i};3m{i};2{Reset} ");
            for (var i = 90; i < 107; i++)
                Console.Write($"{Esc}{i};3m{i};2{Reset} ");
            Console.WriteLine();

            for (var i = 0; i < 255; i++)
            {
                var code = i.ToString().PadLeft(3, '0');
                Console.Write($"{Esc}38;5;{code}m{code}{Reset} ");
                if ((i + 1) % 16 == 0)
                    Console.WriteLine();
            }

            Console.WriteLine();

            for (var i = 0; i < 255; i++)
            {
                var code = i.ToString().PadLeft(3, '0');
                Console.Write($"{Esc}38;5;{code};1m{code}{Reset} ");
                if ((i + 1) % 16 == 0)
                    Console.WriteLine();
            }
        });

    public ConsoleTheme DefaultAnsi256ColorTheme => new AnsiConsoleTheme(
        new Dictionary<ConsoleThemeStyle, string>
        {
            [ConsoleThemeStyle.Text] = "\u001B[39m",
            [ConsoleThemeStyle.SecondaryText] = "\u001B[38;5;247m",
            [ConsoleThemeStyle.TertiaryText] = "\u001B[38;5;247m",
            [ConsoleThemeStyle.Name] = "\u001B[39;1m",
            [ConsoleThemeStyle.Invalid] = "\u001b[35m",
            [ConsoleThemeStyle.Null] = "\u001b[38;5;207m",
            [ConsoleThemeStyle.Number] = "\u001b[38;5;207m",
            [ConsoleThemeStyle.String] = "\u001b[38;5;207m",
            [ConsoleThemeStyle.Boolean] = "\u001b[38;5;207m",
            [ConsoleThemeStyle.Scalar] = "\u001b[38;5;207m",
            [ConsoleThemeStyle.LevelVerbose] = "\u001B[90;1m",
            [ConsoleThemeStyle.LevelDebug] = "\u001B[39;1m",
            [ConsoleThemeStyle.LevelInformation] = "\u001B[38;5;50;1m",
            [ConsoleThemeStyle.LevelWarning] = "\u001B[38;5;214;1m",
            [ConsoleThemeStyle.LevelError] = "\u001B[38;5;196;1m",
            [ConsoleThemeStyle.LevelFatal] = "\u001B[38;5;231;1m\u001B[48;5;196m"
        });

    public ConsoleTheme DefaultSystemColorTheme => new SystemConsoleTheme(
        new Dictionary<ConsoleThemeStyle, SystemConsoleThemeStyle>
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
            [ConsoleThemeStyle.LevelFatal] = new() { Foreground = ConsoleColor.White, Background = ConsoleColor.Red }
        });

    public ConsoleTheme AppVeyorColorTheme => new CustomAnsiConsoleTheme(
        new Dictionary<ConsoleThemeStyle, string>
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
        });

    public ConsoleTheme AzurePipelinesColorTheme => new CustomAnsiConsoleTheme(
        new Dictionary<ConsoleThemeStyle, string>
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
        });

    Target Compile => _ => _
        .DependsOn(Colors, Variables)
        .Executes(() =>
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message}{NewLine}{Exception}",
                    theme: Theme,
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
                // Log.Error(ex, string.Empty);
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
            AzurePipelines => AzurePipelinesColorTheme,
            AppVeyor => AppVeyorColorTheme,
            _ => Environment.GetEnvironmentVariable("TERM") is { } term && term.StartsWithOrdinalIgnoreCase("xterm")
                ? DefaultAnsi256ColorTheme
                : DefaultSystemColorTheme
        };

    public class CustomAnsiConsoleTheme : AnsiConsoleTheme
    {
        public CustomAnsiConsoleTheme([NotNull] IReadOnlyDictionary<ConsoleThemeStyle, string> styles) : base(styles)
        {
        }
    }
}