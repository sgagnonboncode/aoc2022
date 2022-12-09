using System.Text.RegularExpressions;
using AOC2022.Lib;

namespace AOC2022.Solutions.Day09;

public enum Orientation
{
    Right = 'R',
    Up = 'U',
    Left = 'L',
    Down = 'D'
}

public struct Move
{
    public Orientation Orientation;
    public int Amount;
}

public class KnotPlankSimulation
{
    private readonly int[] KnotX;
    private readonly int[] KnotY;
    private readonly int nbKnots;

    public List<(int, int)> TailPositionHistory;

    public KnotPlankSimulation(int nbKnots)
    {
        this.nbKnots = nbKnots;
        KnotX = new int[nbKnots];
        KnotY = new int[nbKnots];

        TailPositionHistory = new List<(int, int)>();
        TailPositionHistory.Add((0, 0));
    }

    public void MovePlank(Move move)
    {
        for (int i = 0; i < move.Amount; i++)
        {
            switch (move.Orientation)
            {
                case Orientation.Right:
                    KnotX[0]++;
                    break;
                case Orientation.Left:
                    KnotX[0]--;
                    break;
                case Orientation.Up:
                    KnotY[0]++;
                    break;
                case Orientation.Down:
                    KnotY[0]--;
                    break;
            }

            for(int j=1;j<nbKnots;j++)
            {
                ApplyKnotLogic(j);
            }

            TailPositionHistory.Add((KnotX[nbKnots-1],KnotY[nbKnots-1]));
        }
    }

    private void ApplyKnotLogic(int n)
    {
        if (KnotX[n-1] == KnotX[n] && KnotY[n-1] == KnotY[n])
        {
            // same pos - no move required
            return;
        }

        if (KnotX[n-1] == KnotX[n] || KnotY[n-1] == KnotY[n])
        {
            // straight moves
            if (KnotX[n-1] - KnotX[n] == 2 && KnotY[n-1] == KnotY[n])
            {
                KnotX[n]++;
            }
            else if (KnotX[n-1] - KnotX[n] == -2 && KnotY[n-1] == KnotY[n])
            {
                KnotX[n]--;
            }
            else if (KnotY[n-1] - KnotY[n] == 2 && KnotX[n-1] == KnotX[n])
            {
                KnotY[n]++;
            }
            else if (KnotY[n-1] - KnotY[n] == -2 && KnotX[n-1] == KnotX[n])
            {
                KnotY[n]--;
            }
        }
        else
        {
            //diagonal moves
            var diffX = KnotX[n-1] - KnotX[n];
            var diffY = KnotY[n-1] - KnotY[n];

            if ((diffX == 1 || diffX == -1) && (diffY == 1 || diffY == -1))
            {
                // still touching
                return;
            }

            if (diffX > 0)
            {
                KnotX[n]++;
            }
            else
            {
                KnotX[n]--;
            }

            if(diffY >0)
            {
                KnotY[n]++;
            }
            else
            {
                KnotY[n]--;
            }
        }
    }
}

[SolverMap(9)]
public class Day09 : Solver
{
    public object PartA()
    {
        var moves = ParseInstructions();
        var simulation = new KnotPlankSimulation(2);

        foreach (var move in moves)
        {
            simulation.MovePlank(move);
        }

        return simulation.TailPositionHistory.Distinct().Count();
    }

    public object PartB()
    {
        var moves = ParseInstructions();
        var simulation = new KnotPlankSimulation(10);

        foreach (var move in moves)
        {
            simulation.MovePlank(move);
        }

        return simulation.TailPositionHistory.Distinct().Count();
    }

    public Move[] ParseInstructions()
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay09.txt");
        return lines.Select(l => l.Split(' ')).Select(s => new Move { Orientation = (Orientation)(s[0][0]), Amount = int.Parse(s[1]) }).ToArray();
    }
}