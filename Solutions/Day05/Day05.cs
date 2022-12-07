using System.Text;
using System.Text.RegularExpressions;
using AOC2022.Lib;

namespace AOC2022.Solutions.Day05;

[SolverMap(5)]
public class Day05 : Solver
{

    public class CraneSpace
    {
        public Stack<Char>[] Stacks = new Stack<Char>[0];

        public void Initialize(int nbStacks)
        {
            Stacks = new Stack<char>[nbStacks];
            for (int i = 0; i < nbStacks; i++)
            {
                Stacks[i] = new Stack<char>();
            }
        }

        public void Stack(int pos, char element)
        {
            Stacks[pos].Push(element);
        }

        public void ExecuteOneByOne(int nb, int from, int to)
        {
            for (int i = 0; i < nb; i++)
            {
                var el = Stacks[from - 1].Pop();
                Stacks[to - 1].Push(el);
            }
        }

        public void ExecuteMany(int nb, int from, int to)
        {
            var reverseOrder = new List<char>();

            for (int i = 0; i < nb; i++)
            {
                var el = Stacks[from - 1].Pop();
                reverseOrder.Add(el);
            }
            reverseOrder.Reverse();
            foreach (var el in reverseOrder)
            {
                Stacks[to - 1].Push(el);
            }
        }

        public string GetTopOfStacks()
        {
            var stringBuilder = new StringBuilder();
            foreach (var stack in Stacks)
            {
                stringBuilder.Append(stack.Peek());
            }

            return stringBuilder.ToString();
        }

        // public void SelfOutput()
        // {
        //     Console.WriteLine("<============");
        //     for (int i = 0; i < Stacks.Length; i++)
        //     {
        //         Console.WriteLine("Stack {0} : {1}", i + 1, string.Join(' ', Stacks[i].ToArray()));
        //     }
        // }
    }

    public object PartA()
    {
        var space = ExecuteProgram();
        return space.GetTopOfStacks();
    }

    public object PartB()
    {
        var space = ExecuteProgram(true);
        return space.GetTopOfStacks();
    }

    private CraneSpace ExecuteProgram(bool model9001 = false)
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay05.txt");
        var pattern = @"move (\d+) from (\d+) to (\d+)";

        var space = new CraneSpace();
        bool spaceDefined = false;
        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                if (spaceDefined)
                {
                    break;
                }

                // premiere ligne vide -> initialize stacks
                // 1   2   3   4   5   6   7   8   9 
                var nbEls = (lines[i - 1].Length + 1) / 4;
                space.Initialize(nbEls);

                // load em up
                for (int j = i - 2; j >= 0; j--)
                {
                    for (int k = 0; k < nbEls; k++)
                    {
                        var el = lines[j].ElementAt(k * 4 + 1);
                        if (!char.IsLetter(el))
                        {
                            continue;
                        }

                        space.Stack(k, el);
                    }
                }

                // space.SelfOutput();
                spaceDefined = true;
                continue;
            }

            // execute program
            if (spaceDefined)
            {
                var matches = Regex.Matches(lines[i], pattern);
                foreach (Match match in matches)
                {
                    var nb = int.Parse(match.Groups[1].Value);
                    var from = int.Parse(match.Groups[2].Value);
                    var to = int.Parse(match.Groups[3].Value);

                    if (model9001)
                    {
                        space.ExecuteMany(nb, from, to);
                    }
                    else
                    {
                        space.ExecuteOneByOne(nb, from, to);
                    }
                }
            }
        }

        // space.SelfOutput();
        return space;
    }
}