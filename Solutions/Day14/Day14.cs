using System.Text;
using System.Text.RegularExpressions;
using AOC2022.Lib;

namespace AOC2022.Solutions.Day14;

[SolverMap(14)]
public class Day14 : Solver
{

    public static int Simulate(Dictionary<int, Dictionary<int, char>> map, int ox, int oy, int maxY, bool partA = true)
    {
        SetPoint(map, ox, oy, '+');

        int placed = 0;
        for (;;)
        {
            var sX = ox;
            var sY = oy;

            for (int j = 0; j <= maxY; j++)
            {
                if (GetPoint(map, sX, sY + 1) == '.')
                {
                    //down is free
                    sY++;
                    continue;
                }

                if (GetPoint(map, sX - 1, sY + 1) == '.')
                {
                    // left is free
                    sY++;
                    sX--;
                    continue;
                }

                if (GetPoint(map, sX + 1, sY + 1) == '.')
                {
                    //right is free
                    sY++;
                    sX++;
                    continue;
                }

                // cant move anymore
                break;
            }

            if (partA && sY > maxY)
            {
                break;
            }

            if (!partA && sY==0)
            {
                placed++;
                break;
            }

            placed++;
            SetPoint(map, sX, sY, 'O');
        }

        return placed;
    }

    public object PartA()
    {
        var segments = ParseInputPartA();
        var map = InitializeCaveGrid(segments);

        var maxY0 = segments.Max(s => s.Y0);
        var maxY1 = segments.Max(s => s.Y1);
        var maxY = maxY0 < maxY1 ? maxY0 : maxY1;

        int ox = 500;
        int oy = 0;

        var placed = Simulate(map, ox, oy, maxY);

        // for (int j = 0; j <= maxY + 1; j++)
        // {
        //     for (int i = 400; i <= 540; i++)
        //     {
        //         Console.Write(GetPoint(map, i, j));
        //     }
        //     Console.Write(Environment.NewLine);
        // }

        return placed;
    }

    public object PartB()
    {
        var segments = ParseInputPartA();
        var map = InitializeCaveGrid(segments);

        var maxY0 = segments.Max(s => s.Y0);
        var maxY1 = segments.Max(s => s.Y1);
        var maxY = maxY0 < maxY1 ? maxY0 : maxY1;
        maxY += 2;

        int ox = 500;
        int oy = 0;

        for (int x = -1000; x <= 1500; x++)
        {
            SetPoint(map, x, maxY, '#');
        }

        var placed = Simulate(map, ox, oy, maxY, false);

        // for (int j = 0; j <= maxY; j++)
        // {
        //     for (int i = 400; i <= 540; i++)
        //     {
        //         Console.Write(GetPoint(map, i, j));
        //     }
        //     Console.Write(Environment.NewLine);
        // }

        return placed;
    }

    public struct Segments
    {
        public int X0;
        public int Y0;
        public int X1;
        public int Y1;
    }

    public static void SetPoint(Dictionary<int, Dictionary<int, char>> map, int x, int y, char feature)
    {
        if (!map.ContainsKey(x))
        {
            map[x] = new Dictionary<int, char>();
        }

        map[x][y] = feature;
    }

    public static char GetPoint(Dictionary<int, Dictionary<int, char>> map, int x, int y)
    {
        if (!map.ContainsKey(x))
        {
            return '.';
        }

        if (!map[x].ContainsKey(y))
        {
            return '.';
        }

        return map[x][y];
    }

    public static Dictionary<int, Dictionary<int, char>> InitializeCaveGrid(IEnumerable<Segments> segments)
    {
        var segmentsArr = segments.ToArray();
        var map = new Dictionary<int, Dictionary<int, char>>();

        foreach (var segment in segmentsArr)
        {
            SetPoint(map, segment.X0, segment.Y0, '#');
            SetPoint(map, segment.X1, segment.Y1, '#');

            // determine orientation
            if (segment.Y0 == segment.Y1)
            {
                // horizontal
                var sign = segment.X0 > segment.X1 ? -1 : 1;
                for (int x = segment.X0 + sign; x != segment.X1; x += sign)
                {
                    SetPoint(map, x, segment.Y0, '#');
                }
            }
            else
            {
                // vertical
                var sign = segment.Y0 > segment.Y1 ? -1 : 1;
                for (int y = segment.Y0 + sign; y != segment.Y1; y += sign)
                {
                    SetPoint(map, segment.X0, y, '#');
                }

            }
        }

        return map;
    }

    public IEnumerable<Segments> ParseInputPartA()
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay14.txt");

        var segments = new List<Segments>();

        foreach (var line in lines)
        {
            var pointsStr = line.Split(" -> ");
            for (int i = 1; i < pointsStr.Length; i++)
            {
                var p0 = pointsStr[i - 1].Split(',').Select(p => int.Parse(p)).ToArray();
                var p1 = pointsStr[i].Split(',').Select(p => int.Parse(p)).ToArray();
                segments.Add(new Segments { X0 = p0[0], Y0 = p0[1], X1 = p1[0], Y1 = p1[1] });
            }
        }

        return segments;
    }
}