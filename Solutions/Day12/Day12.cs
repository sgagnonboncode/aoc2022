using System.Text.RegularExpressions;
using AOC2022.Lib;

namespace AOC2022.Solutions.Day12;



[SolverMap(12)]
public class Day12 : Solver
{

    public struct Step
    {
        public int X;
        public int Y;
    }

    public const int Infinity = 10000000;

    public bool CanMove(int curX, int curY, int x1, int y1, int[][] heightMap)
    {
        var curHeight = heightMap[curY][curX];
        var newHeight = heightMap[y1][x1];
        return newHeight - curHeight <= 1;
    }

    public Step[] ReconstructPath(Step[][] cameFrom, Step current)
    {
        var path = new List<Step>();
        var next = current;

        for (; ; )
        {
            var from = cameFrom[next.Y][next.X];
            if (from.X == -1 && from.Y == -1)
            {
                break;
            }
            path.Add(from);
            next = from;
        }

        path.Reverse();
        return path.ToArray();
    }

    // Algorithme A*
    // https://en.wikipedia.org/wiki/A*_search_algorithm
    public int SolvePath(int sizeX, int sizeY, int startX, int startY, int goalX, int goalY, int[][] heightMap)
    {
        var gScore = new int[sizeY][];
        var fScore = new int[sizeY][];
        var cameFrom = new Step[sizeY][];

        for (int j = 0; j < sizeY; j++)
        {
            gScore[j] = Enumerable.Repeat(Infinity, sizeX).ToArray();
            fScore[j] = Enumerable.Repeat(Infinity, sizeX).ToArray();
            cameFrom[j] = Enumerable.Repeat<Step>(new Step { X = -1, Y = -1 }, sizeX).ToArray();
        }

        var openSet = new List<Step>() { new Step { X = startX, Y = startY } };
        var neighbors = new List<Step>();

        gScore[startY][startX] = 0;
        fScore[startY][startX] = heightMap[goalY][goalX] - heightMap[startY][startX];

        while (openSet.Any())
        {
            var current = openSet.OrderBy(s => fScore[s.Y][s.X]).First();
            if (current.X == goalX && current.Y == goalY)
            {
                // DONE
                var reconstructed = ReconstructPath(cameFrom, current);
                return reconstructed.Length;
            }

            openSet.Remove(current);

            neighbors.Clear();
            if (current.X > 0 && CanMove(current.X, current.Y, current.X - 1, current.Y, heightMap))
            {
                neighbors.Add(new Step { X = current.X - 1, Y = current.Y });
            }
            if (current.Y > 0 && CanMove(current.X, current.Y, current.X, current.Y - 1, heightMap))
            {
                neighbors.Add(new Step { X = current.X, Y = current.Y - 1 });
            }
            if (current.X + 1 < sizeX && CanMove(current.X, current.Y, current.X + 1, current.Y, heightMap))
            {
                neighbors.Add(new Step { X = current.X + 1, Y = current.Y });
            }
            if (current.Y + 1 < sizeY && CanMove(current.X, current.Y, current.X, current.Y + 1, heightMap))
            {
                neighbors.Add(new Step { X = current.X, Y = current.Y + 1 });
            }

            foreach (var neighbor in neighbors)
            {
                var localGScore = gScore[current.Y][current.X] + 1;
                if (localGScore < gScore[neighbor.Y][neighbor.X])
                {
                    cameFrom[neighbor.Y][neighbor.X] = current;
                    gScore[neighbor.Y][neighbor.X] = localGScore;
                    fScore[neighbor.Y][neighbor.X] = localGScore + heightMap[goalY][goalX] - heightMap[neighbor.Y][neighbor.X];
                    if (!openSet.Any(n => n.X == neighbor.X && n.Y == neighbor.Y))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return -1;
    }

    public object PartA()
    {
        var lines = ParseInput();
        var sizeX = lines[0].Length;
        var sizeY = lines.Length;

        var startX = 0;
        var startY = 0;
        var goalX = 0;
        var goalY = 0;

        var heightMap = new int[sizeY][];
        for (int j = 0; j < sizeY; j++)
        {
            heightMap[j] = new int[sizeX];


            var row = lines[j].ToCharArray();
            for (int i = 0; i < sizeX; i++)
            {
                if (row[i] == 'S')
                {
                    startX = i;
                    startY = j;
                    heightMap[j][i] = 0;
                }
                else if (row[i] == 'E')
                {
                    goalX = i;
                    goalY = j;
                    heightMap[j][i] = 'z' - 'a';
                }
                else
                {
                    heightMap[j][i] = row[i] - 'a';
                }
            }
        }
        return SolvePath(sizeX, sizeY, startX, startY, goalX, goalY, heightMap);
    }

    public object PartB()
    {
        var lines = ParseInput();
        var sizeX = lines[0].Length;
        var sizeY = lines.Length;

        var startingLocations = new List<Step>();
        var goalX = 0;
        var goalY = 0;

        var heightMap = new int[sizeY][];
        for (int j = 0; j < sizeY; j++)
        {
            heightMap[j] = new int[sizeX];

            var row = lines[j].ToCharArray();
            for (int i = 0; i < sizeX; i++)
            {
                if (row[i] == 'S')
                {
                    heightMap[j][i] = 0;
                }
                else if (row[i] == 'E')
                {
                    goalX = i;
                    goalY = j;
                    heightMap[j][i] = 'z' - 'a';
                }
                else
                {
                    heightMap[j][i] = row[i] - 'a';
                }

                if (heightMap[j][i] == 0)
                {
                    startingLocations.Add(new Step { X = i, Y = j });
                }
            }
        }

        // brute force all the things !!!!  o/
        return startingLocations.Select(s => SolvePath(sizeX, sizeY, s.X, s.Y, goalX, goalY, heightMap)).Where(s => s > 0).Min();
    }

    public string[] ParseInput()
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay12.txt");
        return lines;
    }
}