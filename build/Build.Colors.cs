using System;
using Nuke.Common;
using Nuke.Common.Utilities;

partial class Build
{
    Target Colors => _ => _
        .Executes(() =>
        {
            const string Esc = "\u001b[";
            const string Reset = "\u001b[0m";

            for (var i = 30; i < 47; i++)
                Console.Write($"{Esc}{i}m{i}  {Reset} ");
            Console.WriteLine();
            for (var i = 90; i < 107; i++)
                Console.Write($"{Esc}{i}m{i}  {Reset} ");
            Console.WriteLine();
            for (var i = 30; i < 47; i++)
                Console.Write($"{Esc}{i};1m{i};1{Reset} ");
            Console.WriteLine();
            for (var i = 90; i < 107; i++)
                Console.Write($"{Esc}{i};1m{i};1{Reset} ");
            Console.WriteLine();
            for (var i = 30; i < 47; i++)
                Console.Write($"{Esc}{i};2m{i};2{Reset} ");
            Console.WriteLine();
            for (var i = 90; i < 107; i++)
                Console.Write($"{Esc}{i};2m{i};2{Reset} ");
            Console.WriteLine();
            for (var i = 30; i < 47; i++)
                Console.Write($"{Esc}{i};3m{i};2{Reset} ");
            Console.WriteLine();
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

            for (var i = 0; i <= 255; i++)
            {
                var code = i.ToString().PadLeft(3, '0');
                Console.Write($"{Esc}38;5;{code};1m{code}{Reset} ");
                if ((i + 1) % 16 == 0)
                    Console.WriteLine();
            }
        });

    string GetAnsiCode(params string[] codes)
    {
        return $"\u001b[{codes.Join(";")}m";
    }
}