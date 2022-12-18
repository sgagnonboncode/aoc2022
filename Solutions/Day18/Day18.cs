using System.Text;
using System.Text.RegularExpressions;
using AOC2022.Lib;

namespace AOC2022.Solutions.Day18;

[SolverMap(18)]
public class Day18 : Solver
{

    public int maxX = 0, maxY = 0, maxZ = 0, minX = 0, minY = 0, minZ = 0, sizeX = 0, sizeY = 0, sizeZ = 0;
    public Position[] positions = new Position[0];
    public int[,,] airMap = new int[0, 0, 0];

    public object PartA()
    {
        positions = ParseInput();

        var openFaces = positions.Length * 6;

        for (int i = 0; i < positions.Length - 1; i++)
        {
            for (int j = i + 1; j < positions.Length; j++)
            {
                var pi = positions[i];
                var pj = positions[j];

                if (pi.Top.Equals(pj))
                {
                    openFaces -= 2;
                }
                if (pi.Bottom.Equals(pj))
                {
                    openFaces -= 2;
                }
                if (pi.Left.Equals(pj))
                {
                    openFaces -= 2;
                }
                if (pi.Right.Equals(pj))
                {
                    openFaces -= 2;
                }
                if (pi.Front.Equals(pj))
                {
                    openFaces -= 2;
                }
                if (pi.Back.Equals(pj))
                {
                    openFaces -= 2;
                }
            }
        }

        return openFaces;
    }

    public void BuildAirMap()
    {
        foreach (var p in positions)
        {
            if (p.X < minX)
            {
                minX = p.X;
            }

            if (p.X > maxX)
            {
                maxX = p.X;
            }

            if (p.Y < minY)
            {
                minY = p.Y;
            }

            if (p.Y > maxY)
            {
                maxY = p.Y;
            }

            if (p.Z < minZ)
            {
                minZ = p.Z;
            }

            if (p.Z > maxZ)
            {
                maxZ = p.Z;
            }
        }


        sizeX = maxX - minX + 1;
        sizeY = maxY - minY + 1;
        sizeZ = maxZ - minZ + 1;

        // Console.WriteLine("MinX{0} MaxX{1} SizeX{2}", minX, maxX, sizeX);
        // Console.WriteLine("MinY{0} MaxY{1} SizeY{2}", minY, maxY, sizeY);
        // Console.WriteLine("MinZ{0} MaxZ{1} SizeZ{2}", minZ, maxZ, sizeZ);

        // -2 sides
        // -1 occupied
        // 0 not air
        // 1 maybe air
        // 2 1x1x1 confirmed air cube
        airMap = new int[sizeX, sizeY, sizeZ];

        // the sides can never be air by definition
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                airMap[i, j, 0] = -2;
                airMap[i, j, sizeZ - 1] = -2;
            }
        }

        for (int k = 1; k < sizeZ - 1; k++)
        {
            for (int i = 0; i < sizeX; i++)
            {
                airMap[i, 0, k] = -2;
                airMap[i, sizeY - 1, k] = -2;
            }

            for (int j = 0; j < sizeY; j++)
            {
                airMap[0, j, k] = -2;
                airMap[sizeX - 1, j, k] = -2;
            }
        }
    }

    public bool PositionOutOfBounds(Position p)
    {
        return (p.X > maxX || p.X < minX || p.Y > maxY || p.Y < minY || p.Z > maxZ || p.Z < minZ);
    }

    public void SetAirMap(Position p, int value)
    {
        if (PositionOutOfBounds(p))
        {
            return;
        }

        var x = minX + p.X;
        var y = minY + p.Y;
        var z = minZ + p.Z;
        airMap[x, y, z] = value;
    }

    public void SetAirMapIfZero(Position p, int value)
    {
        if (PositionOutOfBounds(p))
        {
            return;
        }

        var x = minX + p.X;
        var y = minY + p.Y;
        var z = minZ + p.Z;
        if (airMap[x, y, z] != 0)
        {
            return;
        }

        airMap[x, y, z] = value;
    }

    public int GetAirMap(Position p)
    {
        if (PositionOutOfBounds(p))
        {
            return -2;
        }

        var x = minX + p.X;
        var y = minY + p.Y;
        var z = minZ + p.Z;
        return airMap[x, y, z];
    }

    public List<Position> FindCandidates()
    {
        var candidates = new List<Position>();

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                for (int k = 0; k < sizeZ; k++)
                {
                    if (airMap[i, j, k] == 1)
                    {
                        candidates.Add(new Position { X = minX + i, Y = minY + j, Z = minZ + k });
                    }
                }
            }
        }

        return candidates;
    }

    public bool PositionIsExteriorFace(Position p)
    {
        var v = GetAirMap(p);
        return v == -2 || v == 0;
    }

    public object PartB()
    {
        positions = ParseInput();
        BuildAirMap();

        foreach (var p in positions)
        {
            SetAirMap(p, -1);
        }

        foreach (var p in positions)
        {
            // seed gas
            SetAirMapIfZero(p.Top, 1);
            SetAirMapIfZero(p.Left, 1);
            SetAirMapIfZero(p.Right, 1);
            SetAirMapIfZero(p.Bottom, 1);
            SetAirMapIfZero(p.Front, 1);
            SetAirMapIfZero(p.Back, 1);
        }

        int pass = 1;
        var candidates = FindCandidates();
        int prevCount = candidates.Count();

        while (candidates.Count() > 0)
        {
            foreach (var candidate in candidates)
            {
                if (pass == 1)
                {
                    if (GetAirMap(candidate.Top) == -1 && GetAirMap(candidate.Bottom) == -1 && GetAirMap(candidate.Left) == -1 &&
                        GetAirMap(candidate.Right) == -1 && GetAirMap(candidate.Back) == -1 && GetAirMap(candidate.Front) == -1)
                    {
                        // simple case : cube is surrounded by blocks
                        SetAirMap(candidate, 2);
                        continue;
                    }
                }

                // test if gas touches problem boundary
                if (GetAirMap(candidate.Top) == -2 || GetAirMap(candidate.Bottom) == -2 || GetAirMap(candidate.Left) == -2 ||
                    GetAirMap(candidate.Right) == -2 || GetAirMap(candidate.Back) == -2 || GetAirMap(candidate.Front) == -2)
                {
                    // cannot expand -> grow boundary
                    SetAirMap(candidate, -2);
                    continue;
                }

                // expand gas
                SetAirMapIfZero(candidate.Top, 1);
                SetAirMapIfZero(candidate.Left, 1);
                SetAirMapIfZero(candidate.Right, 1);
                SetAirMapIfZero(candidate.Bottom, 1);
                SetAirMapIfZero(candidate.Front, 1);
                SetAirMapIfZero(candidate.Back, 1);
            }

            candidates = FindCandidates();
            pass++;

            if (pass > 10 && candidates.Count() == prevCount)
            {
                break;
            }

            prevCount = candidates.Count();
        }

        // count exterior faces
        int openFaces = 0;
        foreach (var p in positions)
        {
            if (PositionIsExteriorFace(p.Top))
            {
                openFaces++;
            }

            if (PositionIsExteriorFace(p.Bottom))
            {
                openFaces++;
            }

            if (PositionIsExteriorFace(p.Left))
            {
                openFaces++;
            }

            if (PositionIsExteriorFace(p.Right))
            {
                openFaces++;
            }

            if (PositionIsExteriorFace(p.Front))
            {
                openFaces++;
            }

            if (PositionIsExteriorFace(p.Back))
            {
                openFaces++;
            }
        }

        // var displayMap = new Dictionary<int, char>{
        //     {-2 , '#'},{-1 , 'X'}, { 0, '.' } , { 1,'?'}, {2,'A'}
        // };

        // for (int z = 0; z < sizeZ; z++)
        // {
        //     Console.WriteLine("Z= {0}", minZ + z);

        //     for (int y = 0; y < sizeY; y++)
        //     {
        //         for (int x = 0; x < sizeX; x++)
        //         {
        //             var v = airMap[x, y, z];
        //             Console.Write(displayMap[v]);
        //         }
        //         Console.Write(Environment.NewLine);
        //     }

        //     Console.WriteLine();
        // }

        return openFaces;
    }

    public struct Position
    {
        public int X;
        public int Y;
        public int Z;

        public Position Top { get => new Position { X = X, Y = Y, Z = Z + 1 }; }
        public Position Bottom { get => new Position { X = X, Y = Y, Z = Z - 1 }; }
        public Position Left { get => new Position { X = X - 1, Y = Y, Z = Z }; }
        public Position Right { get => new Position { X = X + 1, Y = Y, Z = Z }; }
        public Position Back { get => new Position { X = X, Y = Y + 1, Z = Z }; }
        public Position Front { get => new Position { X = X, Y = Y - 1, Z = Z }; }

        public bool Equals(Position other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }
    }

    public Position[] ParseInput()
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay18.txt");
        return lines.Select(l => l.Split(',')).Select(s => new Position { X = int.Parse(s[0]), Y = int.Parse(s[1]), Z = int.Parse(s[2]) }).ToArray();
    }
}
