using System.Text.RegularExpressions;
using AOC2022.Lib;

namespace AOC2022.Solutions.Day08;

[SolverMap(8)]
public class Day08 : Solver
{
    public object PartA()
    {
        var map = ParseTreeMap();

        var lx = map[0].Length;
        var ly = map.Length;

        // contour tjrs visible
        int visible = 2 * ly + (2 * (lx - 2));

        for (int j = 1; j < ly - 1; j++)
        {
            for (int i = 1; i < lx - 1; i++)
            {
                if (TreeIsVisible(map, i, j))
                {
                    visible++;
                }
            }
        }

        return visible;
    }

    public object PartB()
    {
        var map = ParseTreeMap();

        var lx = map[0].Length;
        var ly = map.Length;

        // contour a un tree score de 0 par definition
        int bestTreeScore = 0;
        
        for (int j = 1; j < ly - 1; j++)
        {
            for (int i = 1; i < lx - 1; i++)
            {
                int score = TreeScore(map, i, j);
                if (score > bestTreeScore)
                {
                    bestTreeScore = score;
                }
            }
        }

        return bestTreeScore;
    }

    private static bool TreeIsVisible(int[][] map, int i, int j)
    {
        var lx = map[0].Length;
        var ly = map.Length;
        var height = map[j][i];

        // Nord
        bool canSee = true;
        for (int jj = 0; jj < j; jj++)
        {
            if (map[jj][i] >= height)
            {
                canSee = false;
                break;
            }
        }

        if (canSee)
        {
            return true;
        }

        // Sud
        canSee = true;
        for (int jj = ly - 1; jj > j; jj--)
        {
            if (map[jj][i] >= height)
            {
                canSee = false;
                break;
            }
        }

        if (canSee)
        {
            return true;
        }

        // Est
        canSee = true;
        for (int kk = lx - 1; kk > i; kk--)
        {
            if (map[j][kk] >= height)
            {
                canSee = false;
                break;
            }
        }

        if (canSee)
        {
            return true;
        }

        // Ouest 
        canSee = true;
        for (int kk = 0; kk < i; kk++)
        {
            if (map[j][kk] >= height)
            {
                canSee = false;
                break;
            }
        }

        return canSee;
    }

    private static int TreeScore(int[][] map, int i, int j)
    {
        var lx = map[0].Length;
        var ly = map.Length;
        var height = map[j][i];

        int localScore = 1;

        // Nord
        int northScore = 0;
        for (int jj = j - 1; jj >= 0; jj--)
        {
            northScore++;
            if (map[jj][i] >= height)
            {
                break;
            }
        }
        localScore *= northScore;

        // Sud
        int southScore = 0;
        for (int jj = j + 1; jj <= ly - 1; jj++)
        {
            southScore++;
            if (map[jj][i] >= height)
            {
                break;
            }
        }
        localScore *= southScore;

        // Est
        int eastScore = 0;
        for (int kk = i + 1; kk <= lx - 1; kk++)
        {
            eastScore++;
            if (map[j][kk] >= height)
            {
                break;
            }
        }
        localScore *= eastScore;

        // Ouest 
        int westScore = 0;
        for (int kk = i - 1; kk >= 0; kk--)
        {
            westScore++;
            if (map[j][kk] >= height)
            {
                break;
            }
        }
        localScore *= westScore;

        return localScore;
    }

    public int[][] ParseTreeMap()
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay08.txt");
        return lines.Select(l => l.ToArray().Select(c => c - '0').ToArray()).ToArray();
    }

}