using System;
using AOC2022.Lib;

namespace AOC2022;

internal class Program
{
    private static void Main(string[] args)
    {
        WriteFormattedConsoleRow(0xcc00, false, "▄█▄ ▄▄█ ▄ ▄ ▄▄▄ ▄▄ ▄█▄  ▄▄▄ ▄▄  ▄▄ ▄▄▄ ▄▄█ ▄▄▄\n");
        WriteFormattedConsoleRow(0xcc00, false, "█▄█ █ █ █ █ █▄█ █ █ █   █ █ █▄  █  █ █ █ █ █▄█\n");
        WriteFormattedConsoleRow(0xcc00, false, "█ █ █▄█ ▀▄▀ █▄▄ █ █ █▄  █▄█ █   █▄ █▄█ █▄█ █▄▄  /* 2022 */\n");
        WriteFormattedConsoleRow(0xffffff, false, "\n");

        if (args.Length != 1)
        {
            Console.WriteLine("Specifiez le # de la question a exécuter. (ex:  dotnet run 1 )");
            return;
        }

        var question = int.Parse(args[0]);

        Console.WriteLine("Question: {0}", question);


        var allSolutionTypes = typeof(Program).Assembly.GetTypes().Where(
            t => t.IsClass && typeof(Solver).IsAssignableFrom(t)
        );
        var daySolutionType = allSolutionTypes.Where(s=> s.GetCustomAttributes(false).OfType<SolverMapAttribute>().Any(a=>a.Day==question)).FirstOrDefault();

        if(daySolutionType==null){
            Console.WriteLine("Solution introuvable pour le jour #{0}",question);
            return;
        }        

        var solutionObj = Activator.CreateInstance(daySolutionType,true) as Solver;
        if(solutionObj ==null){
            throw new Exception("Probleme lors de l'initialisation de la solution.");
        }

        Console.WriteLine("Partie A: {0}", solutionObj.PartA());
        Console.WriteLine("Partie B: {0}", solutionObj.PartB());
    }

    public static void WriteFormattedConsoleRow(int rgb, bool bold, string text)
    {
        Console.Write($"\u001b[38;2;{rgb >> 16 & 255};{rgb >> 8 & 255};{rgb & 255}{(bold ? ";1" : "")}m{text}");
    }
}