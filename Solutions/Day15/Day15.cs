using System.Text;
using System.Text.RegularExpressions;
using AOC2022.Lib;

namespace AOC2022.Solutions.Day15;

[SolverMap(15)]
public class Day15 : Solver
{
    public static int TaxicabDistance(int x0, int y0, int x1, int y1)
    {
        return Math.Abs(x1 - x0) + Math.Abs(y1 - y0);
    }

    public struct ExclusionZone
    {
        public int CX;
        public int CY;
        public int BX;
        public int BY;
        public int Dist;
    }

    public object PartA()
    {
        var exclusionZones = ParseInput();

        int j = 2000000;
        var occupied = new HashSet<int>();
        for (int i = -10000000; i < 10000000; i++)
        {
            foreach (var zone in exclusionZones)
            {
                if (TaxicabDistance(i, j, zone.CX, zone.CY) <= zone.Dist)
                {
                    if(zone.BX == i && zone.BY==j)
                    {
                        continue;
                    }

                    occupied.Add(i);
                }
            }
        }

        return occupied.Count();
    }

    public object PartB()
    {
        // there no way my cpu can crunch 4000000x4000000 pixels before XMAS ...
        var exclusionZones = ParseInput();
        long maxRange = 4000000;

        for (int j = 0; j <= maxRange; j++)
        {
            // since theres only one point , it will be at one pixel away from the radius of the exclusion zones
            var candidates = new List<int>();

            foreach (var zone in exclusionZones)
            {
                var boundary = zone.Dist + 1;
                var dy = Math.Abs(zone.CY - j);
                if (dy > boundary)
                {
                    continue;
                }

                //        C 
                //       /
                // j ---x- 
                var dx = Math.Abs(boundary - dy);
                if (zone.CX - dx > 0)
                {
                    candidates.Add(zone.CX - dx);
                }
                if (zone.CY + dx < maxRange)
                {
                    candidates.Add(zone.CY + dx);
                }
            }

            foreach (var i in candidates)
            {
                if (exclusionZones.All(z => TaxicabDistance(i, j, z.CX, z.CY) > z.Dist))
                {
                    Console.WriteLine("Match at {0},{1}", i, j);
                    return (long)i * 4000000 + (long)j;
                }
            }
        }

        return -1;
    }

    public ExclusionZone[] ParseInput()
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay15.txt");
        var scanInfo = new List<ExclusionZone>();
        var pattern = @"Sensor at x=(.+), y=(.+): closest beacon is at x=(.+), y=(.+)";

        foreach (var line in lines)
        {
            var matches = Regex.Matches(line, pattern);
            foreach (Match match in matches)
            {
                var sx = int.Parse(match.Groups[1].Value);
                var sy = int.Parse(match.Groups[2].Value);
                var bx = int.Parse(match.Groups[3].Value);
                var by = int.Parse(match.Groups[4].Value);

                scanInfo.Add(new ExclusionZone
                {
                    CX = sx,
                    CY = sy,
                    BX = bx,
                    BY = by,
                    Dist = TaxicabDistance(sx, sy, bx, by)
                });
            }
        }

        return scanInfo.ToArray();
    }
}