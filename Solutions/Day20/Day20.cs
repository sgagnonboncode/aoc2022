using System.Text;
using System.Text.RegularExpressions;
using AOC2022.Lib;

namespace AOC2022.Solutions.Day20;

[SolverMap(20)]
public class Day20 : Solver
{
    public struct EncryptedNumber
    {
        public int OriginalPosition;
        public long Value;
    }

    public static void MixNumbers(List<EncryptedNumber> numbers, EncryptedNumber n)
    {
        if (n.Value == 0)
        {
            return;
        }

        int from = 0;
        for (int k = 0; k < numbers.Count(); k++)
        {
            if (numbers[k].OriginalPosition == n.OriginalPosition)
            {
                from = k;
                break;
            }
        }

        int rot = numbers.Count() - 1;
        int to = (int)((from + n.Value) % rot);

        if (to < 0)
        {
            to += rot;
        }

        if (to == 0 && n.Value < 0)
        {
            // tweak cause par le fait que le nombre se deplace "A GAUCHE" de la cible
            to = rot;
        }

        if (to == from)
        {
            return;
        }

        numbers.RemoveAt(from);
        numbers.Insert(to, n);
    }

    public object PartA()
    {
        var original = ParseInput();
        var encrypted = new List<EncryptedNumber>();
        for (int i = 0; i < original.Length; i++)
        {
            encrypted.Add(new EncryptedNumber { OriginalPosition = i, Value = original[i] });
        }

        // Console.WriteLine("Initial: {0}", string.Join(", ", encrypted.Select(e => e.Value)));

        for (int i = 0; i < original.Length; i++)
        {
            int n = original[i];
            MixNumbers(encrypted, new EncryptedNumber { OriginalPosition = i, Value = n });
            // Console.WriteLine("Step {0} ({2}): {1}", i + 1, string.Join(", ", encrypted.Select(e => e.Value)), n);
        }

        var finalMix = encrypted.Select(n => n.Value).ToList();
        var zeroPos = finalMix.IndexOf(0);

        var z1 = finalMix[(zeroPos + 1000) % original.Length];
        var z2 = finalMix[(zeroPos + 2000) % original.Length];
        var z3 = finalMix[(zeroPos + 3000) % original.Length];


        return z1 + z2 + z3;
    }



    public object PartB()
    {
        var original = ParseInput().Select(n => (long)n * 811589153).ToArray();
        var encrypted = new List<EncryptedNumber>();
        for (int i = 0; i < original.Length; i++)
        {
            encrypted.Add(new EncryptedNumber { OriginalPosition = i, Value = original[i] });
        }

        // Console.WriteLine("Initial: {0}", string.Join(", ", encrypted.Select(e => e.Value)));

        for (int r = 0; r < 10; r++)
        {
            for (int i = 0; i < original.Length; i++)
            {
                long n = original[i];
                MixNumbers(encrypted, new EncryptedNumber { OriginalPosition = i, Value = n });
            }
            // Console.WriteLine("Step {0}: {1}", r + 1, string.Join(", ", encrypted.Select(e => e.Value)));
        }

        var finalMix = encrypted.Select(n => n.Value).ToList();
        var zeroPos = finalMix.IndexOf(0);

        var z1 = finalMix[(zeroPos + 1000) % original.Length];
        var z2 = finalMix[(zeroPos + 2000) % original.Length];
        var z3 = finalMix[(zeroPos + 3000) % original.Length];
        Console.WriteLine("{0}+{1}+{2}", z1, z2, z3);

        return z1 + z2 + z3;
    }

    public int[] ParseInput()
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay20.txt");
        return lines.Select(l => int.Parse(l)).ToArray();
    }
}
