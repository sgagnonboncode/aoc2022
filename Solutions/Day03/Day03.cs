using AOC2022.Lib;

namespace AOC2022.Solutions.Day03;

[SolverMap(3)]
public class Day03 : Solver
{
    private int GetItemPriority(char item)
    {
        return char.IsLower(item) ? (item -'a' + 1) : (item -'A' + 27);
    }

    public int PartA()
    {
        var lines = ParseRucksacks();

        var sumOfPriorities = 0;
        foreach (var line in lines)
        {
            var strLen = line.Length;
            var left = line.Substring(0, strLen / 2);
            var right = line.Substring(strLen / 2, strLen / 2);

            var uniqueLeft = left.ToCharArray().Distinct();
            var uniqueRight = right.ToCharArray().Distinct();

            var inBoth = uniqueLeft.Intersect(uniqueRight).ToArray();

            foreach (var item in inBoth)
            {
                sumOfPriorities += GetItemPriority(item);
            }
        }

        return sumOfPriorities;
    }

    public int PartB()
    {
        var lines = ParseRucksacks();

        var sumOfPriorities = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (i % 3 == 2)
            {
                var uniqueA = lines[i - 2].ToCharArray().Distinct();
                var uniqueB = lines[i - 1].ToCharArray().Distinct();
                var uniqueC = lines[i].ToCharArray().Distinct();

                var common = uniqueA.Intersect(uniqueB.Intersect(uniqueC)).ToArray();

                foreach (var item in common)
                {
                    sumOfPriorities += GetItemPriority(item);
                }
            }
        }

        return sumOfPriorities;
    }

    private string[] ParseRucksacks(bool relativeMode = false)
    {
        return File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay03.txt");
    }
}