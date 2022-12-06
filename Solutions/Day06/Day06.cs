using AOC2022.Lib;

namespace AOC2022.Solutions.Day04;

[SolverMap(6)]
public class Day06 : Solver
{
    public object PartA()
    {
        var packet = ReadPacket();

        for (int i = 3; i < packet.Length; i++)
        {
            var lastFour = new HashSet<char>{packet[i-3],packet[i-2],packet[i-1],packet[i]};
            if(lastFour.Count()==4)
            {
                return i+1;
            }
        }

        throw new Exception();
    }

    public object PartB()
    {
        var packet = ReadPacket();

        for (int i = 13; i < packet.Length; i++)
        {
            var lastFourteen = new HashSet<char>(packet.Skip(i-13).Take(14));
            if(lastFourteen.Count()==14)
            {
                return i+1;
            }
        }

        throw new Exception();
    }

    private char[] ReadPacket()
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay06.txt");
        return lines[0].ToArray();
    }
}