using System.Text;
using System.Text.RegularExpressions;
using AOC2022.Lib;

namespace AOC2022.Solutions.Day17;

[SolverMap(17)]
public class Day17 : Solver
{
    public const int width = 7;
    public const int startOffset = 2;
    public const int startHeight = 3;
    public const long nbrocks = 5;
    public readonly int[] wiggle = new int[5] { 0, 1, 1, 3, 2 };

    public char[,] shapes = new char[,]{
        { '.','.','.','.',
          '.','.','.','.',
          '.','.','.','.',
          '#','#','#','#'},

        { '.','.','.','.',
          '.','#','.','.',
          '#','#','#','.',
          '.','#','.','.'},

        { '.','.','.','.',
          '.','.','#','.',
          '.','.','#','.',
          '#','#','#','.'},

        { '#','.','.','.',
          '#','.','.','.',
          '#','.','.','.',
          '#','.','.','.'},

        { '.','.','.','.',
          '.','.','.','.',
          '#','#','.','.',
          '#','#','.','.'},
    };

    public bool CanMove(Dictionary<long, char[]> grid, int rock, long y, int x)
    {
        for (int row = 0; row < 4; row++)
        {
            for (int col = x; col < width; col++)
            {
                if (col - x > 3)
                {
                    break;
                }

                var sh = shapes[rock, row * 4 + col - x];
                if (sh == '.')
                {
                    continue;
                }

                var gr = grid[y + 3 - row][col];
                if (gr != '.')
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void Apply(Dictionary<long, char[]> grid, int rock, long y, int x)
    {
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                if (col < x || (col - x) > 3)
                {
                    continue;
                }

                var sh = shapes[rock, row * 4 + col - x];
                if (sh != '.')
                {
                    grid[y + 3 - row][col] = (char)('1' + rock);
                }
            }
        }
    }

    private int SimulateBlock(Dictionary<long, char[]> grid, char[] movementPattern, int p, long maxlevel, long r)
    {
        int rock = (int)(r % (long)nbrocks);
        long y = maxlevel + 1 + startHeight;
        int x = startOffset;

        while (y >= 0)
        {
            // shift
            if (movementPattern[p] == '<')
            {
                if (x > 0 && CanMove(grid, rock, y, x - 1))
                {
                    x--;
                }
            }
            else
            {
                if ((4 - wiggle[rock] + x < width) && CanMove(grid, rock, y, x + 1))
                {
                    x++;
                }
            }

            p++;
            if (p >= movementPattern.Length)
            {
                p = 0;
            }

            // fall
            if (y > 0 && CanMove(grid, rock, y - 1, x))
            {
                y--;
            }
            else
            {
                break;
            }
        }

        // apply
        Apply(grid, rock, y, x);

        return p;
    }

    public object PartA()
    {
        var movementPattern = ParseInput();
        var grid = new Dictionary<long, char[]>();

        // warm up
        for (long i = 0; i < 10000; i++)
        {
            grid[i] = Enumerable.Repeat('.', width).ToArray();
        }

        long maxlevel = -1;
        int p = 0;

        for (long r = 0; r < 2022; r++)
        {
            p = SimulateBlock(grid, movementPattern, p, maxlevel, r);

            foreach (var kv in grid)
            {
                if (kv.Key >= maxlevel && kv.Value.Any(c => c != '.'))
                {
                    maxlevel = kv.Key;
                }
            }
        }

        // for (int d = maxlevel + 3; d >= 0; d--)
        // {
        //     Console.WriteLine("|{0}{1}{2}{3}{4}{5}{6}|", grid[d][0], grid[d][1], grid[d][2], grid[d][3], grid[d][4], grid[d][5], grid[d][6]);
        // }
        // Console.WriteLine("+-------+");
        // Console.WriteLine();

        return maxlevel + 1;
    }

    public object PartB()
    {
        var movementPattern = ParseInput();
        var grid = new Dictionary<long, char[]>();

        // warm up
        for (long i = 0; i < 10000; i++)
        {
            grid[i] = Enumerable.Repeat('.', width).ToArray();
        }

        long maxlevel = -1;
        int p = 0;
        long measure = 0;
        long skippedLevels = 0;

        const long upperBound = 1000000000000;
        for (long r = 0; r < upperBound; r++)
        {
            if (r == 3376)
            {
                // in my data , there is a repeating pattern starting at this point
                // every 1720 rows will yield 2732 height
                while (r + 1720 < upperBound)
                {
                    r += 1720;
                    skippedLevels += 2738;
                }
            }

            // if(r==111)
            // {
            //     // in the example data , there is a repeating pattern starting at this point
            //     // every (111-76) rows will yield 41+12 height
            //     while(r+(111-76) < upperBound)
            //     {
            //         r+=111-76;
            //         skippedLevels+=41+12;
            //     }
            // }

            // Console.WriteLine("Rock {0} Memory level {1} Real level {2}", r, maxlevel, maxlevel+ skippedLevels);

            int rock = (int)(r % (long)nbrocks);
            long y = maxlevel + 1 + startHeight;

            if (rock == 0 && maxlevel > 0 && grid[y - 4][3] != '.')
            {
                // Console.WriteLine("Rock {0} Level {1} Previous {2} Delta {3}", r + 1, maxlevel, measure, maxlevel - measure);
                measure = maxlevel;
            }

            p = SimulateBlock(grid, movementPattern, p, maxlevel, r);

            foreach (var kv in grid)
            {
                if (kv.Key >= maxlevel && kv.Value.Any(c => c != '.'))
                {
                    maxlevel = kv.Key;
                }
            }
        }

        return skippedLevels + maxlevel + 1;
    }

    public char[] ParseInput()
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay17.txt");
        return lines[0].ToCharArray();
    }
}