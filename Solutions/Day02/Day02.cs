using AOC2022.Lib;

namespace AOC2022.Solutions.Day02;

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
        Other = (Hand) (hand - 'A' + 1);
    }

    private void AssignPlayerHand(char hand)
    {
        Player = (Hand)(hand - 'X' + 1);
    }

    private void AssignRelativeHand(char desiredOutcome)
    {
        var shift = (int)(desiredOutcome - 'Y');
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

        if (Other == Player)
        {
            PlayerScore += DrawScore;
            return;
        }

        if ((Other == Hand.Rock && Player == Hand.Paper) ||
            (Other == Hand.Paper && Player == Hand.Scissor) ||
            (Other == Hand.Scissor && Player == Hand.Rock))
        {
            PlayerScore += VictoryScore;
        }
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