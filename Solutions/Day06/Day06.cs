using AOC2022.Lib;

namespace AOC2022.Solutions.Day04;

[SolverMap(6)]
public class Day06 : Solver
{
    public object PartA()
    {
        var packet = ReadPacket();
        return DetectMarkerPosition(packet,4);
    }

    public object PartB()
    {
        var packet = ReadPacket();
        return DetectMarkerPosition(packet,14);
    }

    private int DetectMarkerPosition(char[] packet, int markerSize)
    {
        var validationHashset = new HashSet<char>();
        int offset = markerSize-1;

        for (int i = offset; i < packet.Length; i++)
        {
            validationHashset.UnionWith(packet.Skip(i - offset).Take(markerSize));
            if (validationHashset.Count() == markerSize)
            {
                return i + 1;
            }

            validationHashset.Clear();
        }

        return -1;
    }

    private char[] ReadPacket()
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay06.txt");
        return lines[0].ToArray();
    }
}