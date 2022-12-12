using System.Text.RegularExpressions;
using AOC2022.Lib;

namespace AOC2022.Solutions.Day11;

public enum OperationKind
{
    AddValue,
    Multiply,
    Square
}

public struct InspectionResult
{
    public ulong WorryLevel;
    public int Target;
}


public class MonkeyActor
{
    public int MonkeyId { get; private set; }
    public Queue<ulong> Items;

    private OperationKind OperationKind;
    private ulong Operand;

    public ulong DivisorTest;
    private int TargetTestTrue;
    private int TargetTestFalse;

    public int InspectionCount { get; private set; }

    public MonkeyActor(string[] config)
    {
        MonkeyId = int.Parse(config[0].Replace(":", "").Split(' ')[1]);

        Items = new Queue<ulong>();
        foreach (var item in config[1].Replace("Starting items: ", "").Trim().Replace(" ", "").Split(",").Select(i => ulong.Parse(i)))
        {
            Items.Enqueue(item);
        }

        var rawOperation = config[2].Replace("Operation: new = old", "").Trim();
        if (rawOperation[0] == '+')
        {
            OperationKind = OperationKind.AddValue;
            Operand = ulong.Parse(rawOperation.Substring(2));
        }
        else if (rawOperation[0] == '*')
        {
            var rawOperand = rawOperation.Substring(2);
            if (rawOperand == "old")
            {
                OperationKind = OperationKind.Square;
            }
            else
            {
                OperationKind = OperationKind.Multiply;
                Operand = ulong.Parse(rawOperand);
            }
        }
        else
        {
            throw new ArgumentException();
        }

        DivisorTest = ulong.Parse(config[3].Replace("Test: divisible by ", "").Trim());
        TargetTestTrue = int.Parse(config[4].Replace("If true: throw to monkey", "").Trim());
        TargetTestFalse = int.Parse(config[5].Replace("If false: throw to monkey ", "").Trim());
    }

    public void CatchItem(ulong worryLevel)
    {
        Items.Enqueue(worryLevel);
    }

    public InspectionResult[] ExecuteRound(bool veryWorriedAboutMonkeys = false, ulong leastCommonDenominator = 0)
    {
        var results = new List<InspectionResult>();
        while (Items.Any())
        {
            InspectionCount++;
            var next = Items.Dequeue();

            var cpy = next;
            switch (OperationKind)
            {
                case OperationKind.AddValue:
                    next += Operand;
                    break;
                case OperationKind.Multiply:
                    next *= Operand;
                    break;
                case OperationKind.Square:
                    next *= next;
                    break;
            }

            if (veryWorriedAboutMonkeys)
            {
                // Les operations des singes sont trop anxiogenes pour etre contenues dans des nombres 64bit.
                // Pour contourner le probleme, on a se servir du fait que seul le nombre de passage compte:
                //   le programme demeure valide tant qu'on peut faire le 'test du diviseur' qui permet
                //   de savoir a qui lancer l'objet. La valeur elle même n'a pas d'importance si elle
                //   produit un résultat équivalent lors du 'test du diviseur'.
                // 
                // L'opérateur modulo (%) possède une propriété d'intérêt pour le problème:
                //   x % n = ( x % M ) % n , si n est un facteur de M
                //
                // Preuve:
                //  1) Soit  k = x % M
                //  2) Par définition de l'operateur %, il existe un nombre 'a' tel que  x = a*M + k , avec 0 <= k < M.
                //       Par exemple,  si le résultat de  x % M = 3 , il existe une valeur 'a' qui viendra faire la boucle sur l'opérateur %
                //          pour revenir au même résultat. 
                //  3) Puisque 'n' est un facteur de 'M', il existe un nombre 'b' tel que M = b * n, ainsi
                //         x = a*M + K = a*b*n + k
                //  4) En appliquant % n sur les deux cotés de l'équation:
                //     a) x % n = (a*b*n +k)%n = k % n , car a*b*n % n =0 
                //     b) en utilisant 1) :   x % n  = k % n = ( x % M ) % n    
                // 
                // On va choisir comme valeur M le produit de tous les diviseurs de tous les singes, (plus petit commun denominateur) 
                //  M = (div 0)(div 1)(div 2)...
                // Ainsi, l'équivalence  x % n = ( x % M ) % n sera tjrs valide.
                // 
                // par exemple, pour le nombre (x) 4000 , avec (n) = 13 et (M) lcd 96577 , soit (7429 * 13) , si on eleve au carre:
                //   4000*4000 = 16 000 000
                //  16000000 % 96577 = 64795
                //
                //    64795 % 13 = 3
                // 16000000 % 13 = 3   car  16000000 - 64795 = 15 935 205 et  15 935 205 = (1 225 785)(13)  = (165)(96577)(13)
                //
                // le nombre 4000 peut etre exprime :  (307)(13) + 9 , 
                //   au carre :  (307)(13)(307)(13) + (2)(9)(307)(13) + (9)(9)
                //   on peut ignorer les facteur de 13 de l equation car leur modulo 13 donnera zero
                //   (4000*4000) % 13 ->  (9 * 9) % 13 = 3
                //
                // le resultat demeure vrai meme si on continue
                //   ( 15999919 + 81 ) * (15,999,919 + 81)
                // =>   ((1230763)(13) + 81 ) * ((1230763)(13) + 81 )
                // =>      (1230763)(13)(1230763)(13) + (2)(81)(1230763)(13) + (81)(81)
                //  en decomposant 81 en (6)(13) + 3, ->
                //  ( ((6)(13)+3) ((6)(13)+3) ) % 13 ->   (3*3) % 13 = 9
                //
                //   si on compare avec le resultat reduit par le lcd:
                //   64795 * 64795  = 4198392025   ,  4198392025 % 13 = 9
                //      car  64795 peut s exprimer (4984)(13) + 3

                next = next % leastCommonDenominator;
            }
            else
            {
                next = next / 3;
            }

            results.Add(new InspectionResult
            {
                WorryLevel = next,
                Target = (next % DivisorTest == 0) ? TargetTestTrue : TargetTestFalse
            });
        }

        return results.ToArray();
    }
}


[SolverMap(11)]
public class Day11 : Solver
{
    public object PartA()
    {
        var actors = ParseInput().OrderBy(a => a.MonkeyId).ToArray();
        for (int i = 0; i < 20; i++)
        {
            for (int m = 0; m < actors.Length; m++)
            {
                var inspected = actors[m].ExecuteRound();
                foreach (var item in inspected)
                {
                    actors[item.Target].CatchItem(item.WorryLevel);
                }
            }
        }

        foreach (var actor in actors)
        {
            Console.WriteLine("Monkey {0} inspected items {1} times.", actor.MonkeyId, actor.InspectionCount);
        }

        var top2 = actors.OrderByDescending(a => a.InspectionCount).Take(2).Select(a => a.InspectionCount).ToArray();
        return top2[0] * top2[1];
    }

    public object PartB()
    {
        var actors = ParseInput().OrderBy(a => a.MonkeyId).ToArray();

        ulong leastCommonDenominator = actors.Select(a => a.DivisorTest).Aggregate((ulong)1, (a, b) => a * b);
        Console.WriteLine("Least common denominator: {0}", leastCommonDenominator);

        for (int i = 0; i < 10000; i++)
        {
            for (int m = 0; m < actors.Length; m++)
            {
                var inspected = actors[m].ExecuteRound(true, leastCommonDenominator);
                foreach (var item in inspected)
                {
                    actors[item.Target].CatchItem(item.WorryLevel);
                }
            }
        }

        foreach (var actor in actors)
        {
            Console.WriteLine("Monkey {0} inspected items {1} times.", actor.MonkeyId, actor.InspectionCount);
        }

        var top2 = actors.OrderByDescending(a => a.InspectionCount).Take(2).Select(a => (long)a.InspectionCount).ToArray();
        return top2[0] * top2[1];
    }

    public MonkeyActor[] ParseInput()
    {
        var actors = new List<MonkeyActor>();
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay11ExA.txt");

        int lastStackIndex = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                actors.Add(new MonkeyActor(lines.Skip(lastStackIndex).Take(i - lastStackIndex + 1).ToArray()));
                lastStackIndex = i + 1;
            }
        }

        actors.Add(new MonkeyActor(lines.Skip(lastStackIndex).ToArray()));
        return actors.ToArray();
    }
}