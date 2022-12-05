using AOC2022.Lib;

namespace AOC2022.Solutions.Day01;


[SolverMap(1)]
public class Day01 : Solver
{
    public object PartA()
    {
        var calories = ParseCaloriesAllocation();
        return calories.Max();
    }

    public object PartB()
    {
        var calories = ParseCaloriesAllocation();
        return calories.OrderDescending().Take(3).Sum();
    }

    private int[] ParseCaloriesAllocation()
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay01.txt");
        var calories = new List<int>();

        int current = 0;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                if (current == 0)
                {
                    continue;
                }

                calories.Add(current);
                current = 0;
            }
            else
            {
                current += int.Parse(line);
            }
        }

        return calories.ToArray();
    }

}