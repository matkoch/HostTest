using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using Spectre.Console;

namespace ConsoleApp1
{
    class Program
    {
        private static Style Selected => Style.Plain
            .Foreground(Color.Turquoise2)
            .Decoration(Decoration.Bold);

        private static string PromptInput(string icon, string prompt)
        {
            return PromptInput<string>(icon, prompt);
        }

        private static T PromptInput<T>(string icon, string prompt)
        {
            AnsiConsole.Markup($":{icon}:  [bold turquoise2]{prompt}[/]:");
            return AnsiConsole.Ask<T>(string.Empty);
        }

        private static bool PromptConfirmation(string icon, string prompt)
        {
            return PromptSelection(icon, prompt, ("Yes", true), ("No", false));
        }

        private static T PromptSelection<T>(
            string icon,
            string prompt,
            params (string Description, T Value)[] choices)
        {
            var converter = new Func<T, string>(x => choices.Single(y => y.Value.Equals(x)).Description);
            AnsiConsole.MarkupLine($":{icon}:  [bold turquoise2]{prompt}[/]:");
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<T>()
                    .UseConverter(converter)
                    .AddChoices(choices.Select(x => x.Value))
                    .HighlightStyle(Selected));
            AnsiConsole.Cursor.Move(CursorDirection.Up, 1);
            AnsiConsole.MarkupLine($":{icon}:  [bold turquoise2]{prompt}[/]: {converter.Invoke(selection)}");
            return selection;
        }

        private static IReadOnlyList<T> PromptMultiSelection<T>(
            string icon,
            string prompt,
            params (string Description, T Value)[] choices)
        {
            var converter = new Func<T, string>(x => choices.Single(y => y.Value.Equals(x)).Description);
            AnsiConsole.MarkupLine($":{icon}:  [bold turquoise2]{prompt}[/]:");
            var selection = AnsiConsole.Prompt(
                new MultiSelectionPrompt<T>()
                    .UseConverter(converter)
                    .AddChoices(choices.Select(x => x.Value))
                    .HighlightStyle(Selected));
            AnsiConsole.Cursor.Move(CursorDirection.Up, 1);
            AnsiConsole.MarkupLine($":{icon}:  [bold turquoise2]{prompt}[/]: {string.Join(", ", selection.Select(converter))}");
            return selection;
        }

        static void Main(string[] args)
        {
            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [green]password[/]")
                    .PromptStyle("red")
                .AddChoice("foo")
                .AddChoice("bar"));
            var multiChoices = PromptMultiSelection("label", "foo123", ("First", "a"), ("Second", "b"));
            var choice1 = PromptSelection("label", "fooo123", ("First", 1), ("Second", 2));
            var projectName = PromptInput("label", "Build Project Name");


            var integer = AnsiConsole.Ask<int>("Integer?");
            Console.WriteLine(integer);

            var confirm = AnsiConsole.Confirm("Confirm?");
            Console.WriteLine(confirm);
        }
    }
}