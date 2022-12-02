using AOC2022.Lib;

namespace AOC2022.Solutions.Day01;

public enum Hand
{
    Rock = 1,
    Paper = 2,
    Scissor = 3
}

public class RPSMatch
{
    public const int VictoryScore = 6;
    public const int DrawScore = 3;

    public Hand Other;
    public Hand Player;

    public int OtherScore;
    public int PlayerScore;

    public RPSMatch(char other, char player, bool relativeMode = false)
    {
        AssignOtherHand(other);

        if (relativeMode)
        {
            AssignRelativeHand(player);
        }
        else
        {
            AssignPlayerHand(player);
        }

        ComputeScore();
    }

    private void AssignOtherHand(char hand)
    {
        switch (hand)
        {
            case 'A':
                Other = Hand.Rock;
                break;
            case 'B':
                Other = Hand.Paper;
                break;
            case 'C':
                Other = Hand.Scissor;
                break;
            default:
                throw new ArgumentException();
        }
    }

    private void AssignPlayerHand(char hand)
    {
        switch (hand)
        {
            case 'X':
                Player = Hand.Rock;
                break;
            case 'Y':
                Player = Hand.Paper;
                break;
            case 'Z':
                Player = Hand.Scissor;
                break;
            default:
                throw new ArgumentException();
        }
    }

    private void AssignRelativeHand(char desiredOutcome)
    {
        var shift = 0;
        switch (desiredOutcome)
        {
            case 'X':
                shift = -1;
                break;
            case 'Y':
                shift = 0;
                break;
            case 'Z':
                shift = +1;
                break;
            default:
                throw new ArgumentException();
        }

        var hand = (int)Other + shift;
        if (hand == 4)
        {
            hand = 1;
        }
        else if (hand == 0)
        {
            hand = 3;
        }

        Player = (Hand)hand;
    }

    private void ComputeScore()
    {
        PlayerScore = (int)Player;
        OtherScore = (int)Other;

        if (Other == Player)
        {
            PlayerScore += DrawScore;
            OtherScore += DrawScore;
            return;
        }

        if ((Other == Hand.Rock && Player == Hand.Paper) ||
            (Other == Hand.Paper && Player == Hand.Scissor) ||
            (Other == Hand.Scissor && Player == Hand.Rock))
        {
            PlayerScore += VictoryScore;
            return;
        }

        if ((Other == Hand.Paper && Player == Hand.Rock) || 
            (Other == Hand.Scissor && Player == Hand.Paper) || 
            (Other == Hand.Rock && Player == Hand.Scissor))
        {
            OtherScore += VictoryScore;
            return;
        }

        throw new ArgumentException();
    }

}

[SolverMap(2)]
public class Day02 : Solver
{
    public int PartA()
    {
        var strategyGuide = ParseStrategyGuide();
        return strategyGuide.Sum(m => m.PlayerScore);
    }

    public int PartB()
    {
        var strategyGuide = ParseStrategyGuide(true);
        return strategyGuide.Sum(m => m.PlayerScore);
    }

    private RPSMatch[] ParseStrategyGuide(bool relativeMode = false)
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay02.txt");

        var strategyGuide = new List<RPSMatch>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                break;
            }

            var hands = line.Split(' ', 2).Select(s => s[0]).ToArray();
            strategyGuide.Add(new RPSMatch(hands[0], hands[1], relativeMode));
        }

        return strategyGuide.ToArray();
    }

}