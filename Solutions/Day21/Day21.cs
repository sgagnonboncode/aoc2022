using System.Text;
using System.Text.RegularExpressions;
using AOC2022.Lib;

namespace AOC2022.Solutions.Day21;

[SolverMap(21)]
public class Day21 : Solver
{


    public object PartA()
    {
        var monkeys = ParseInput();
        var monkeyNames = new HashSet<string>(monkeys.Keys);

        var yelledNumbers = new Dictionary<string, long>();

        foreach (var monkey in monkeys.Where(m => m.Value.Operation == OperationKind.YellNumber))
        {
            yelledNumbers[monkey.Key] = monkey.Value.YellValue;
            // Console.WriteLine("{0}: {1}", monkey.Key, yelledNumbers[monkey.Key]);
        }

        while (!yelledNumbers.ContainsKey("root"))
        {
            var notYelled = monkeyNames.Except(yelledNumbers.Keys).ToArray();

            foreach (var name in notYelled)
            {
                var monkey = monkeys[name];
                var depA = monkey.DependsOn[0];
                var depB = monkey.DependsOn[1];

                if (yelledNumbers.ContainsKey(depA) && yelledNumbers.ContainsKey(depB))
                {
                    yelledNumbers[name] = monkey.MonkeyBusiness(yelledNumbers[depA], yelledNumbers[depB]);
                    // Console.WriteLine("{0}: {1}", name, yelledNumbers[name]);
                }
            }
        }

        return yelledNumbers["root"];
    }

    public object PartB()
    {
        var monkeys = ParseInput();
        const string humanKey = "humn";

        monkeys.Remove(humanKey);
        var root = monkeys["root"];
        var eqA = root.DependsOn[0];
        var eqB = root.DependsOn[1];
        monkeys.Remove("root");

        // simplify
        int nbReplaced = 1;
        while (nbReplaced > 0)
        {
            nbReplaced = 0;

            var subjects = monkeys.Where(kv => kv.Value.Operation != OperationKind.YellNumber).Select(kv => kv.Key).ToArray();
            foreach (var name in subjects)
            {
                if (monkeys[name].DependsOn[0] == humanKey || monkeys[name].DependsOn[1] == humanKey)
                {
                    continue;
                }

                var depA = monkeys[monkeys[name].DependsOn[0]];
                var depB = monkeys[monkeys[name].DependsOn[1]];

                if (depA.Operation != OperationKind.YellNumber || depB.Operation != OperationKind.YellNumber)
                {
                    continue;
                }

                var subj = monkeys[name];
                subj.YellValue = subj.MonkeyBusiness(depA.YellValue, depB.YellValue);
                subj.Operation = OperationKind.YellNumber;
                subj.DependsOn = new string[0];
                monkeys[name] = subj;
                nbReplaced++;
            }
        }

        var yelledNumbers = new Dictionary<string, long>();
        foreach (var kv in monkeys.Where(kv => kv.Value.Operation == OperationKind.YellNumber))
        {
            yelledNumbers[kv.Key] = kv.Value.YellValue;
        }

        // Console.WriteLine("Monkeys:{0}, Reduced:{1}", monkeys.Count(), monkeys.Where(kv => kv.Value.Operation != OperationKind.YellNumber).Count());
        // foreach (var m in monkeys)//.Where(kv => kv.Value.Operation != OperationKind.YellNumber))
        // {
        //     if (m.Value.DependsOn.Length == 0)
        //     {
        //         Console.WriteLine("  | {0} | {1}",
        //             m.Key, m.Value.YellValue);
        //         continue;
        //     }

        //     bool depHuman = (m.Value.DependsOn[0] == humanKey || m.Value.DependsOn[1] == humanKey);
        //     var depA = m.Value.DependsOn[0];
        //     var depB = m.Value.DependsOn[1];

        //     Console.WriteLine("{0} | {4} | {1} | {2} | {3}", depHuman ? "*" : " ",
        //         yelledNumbers.ContainsKey(depA) ? yelledNumbers[depA].ToString().PadRight(4) : depA,
        //         yelledNumbers.ContainsKey(depB) ? yelledNumbers[depB].ToString().PadRight(4) : depB,
        //         m.Value.Operation, m.Key);
        // }

        // dans mon input, il ny a qu un seul monkey qui depend de 'humn'.
        if (monkeys.Where(kv => kv.Value.DependsOn.Length > 0 && (kv.Value.DependsOn[0] == humanKey || kv.Value.DependsOn[1] == humanKey)).Count() != 1)
        {
            throw new Exception("Unexpected input");
        }

        // dans mon input , tout les nombres restant ont au moins une variable de definie sur les 2 dependances
        if (monkeys.Any(kv => kv.Value.DependsOn.Length > 0 &&
                !yelledNumbers.ContainsKey(kv.Value.DependsOn[0]) &&
                !yelledNumbers.ContainsKey(kv.Value.DependsOn[1])))
        {
            throw new Exception("Unexpected input");
        }

        Console.WriteLine("Root : {0} = {1}", eqA, eqB);
        // dans mon input, root contiens au moins 1 des 2 valeur apres simplification
        // a partir de la , il suffit simplement de faire les operation inverse
        if (yelledNumbers.ContainsKey(eqA))
        {
            yelledNumbers[eqB] = yelledNumbers[eqA];
        }
        else if (yelledNumbers.ContainsKey(eqB))
        {
            yelledNumbers[eqA] = yelledNumbers[eqB];
        }
        else
        {
            throw new Exception("Unexpected input");
        }

        nbReplaced = 1;
        while (nbReplaced > 0)
        {
            nbReplaced = 0;

            var subjects = monkeys.Where(kv => kv.Value.Operation != OperationKind.YellNumber).Select(kv => kv.Key).ToArray();
            foreach (var name in subjects)
            {
                if (!yelledNumbers.ContainsKey(name))
                {
                    continue;
                }

                var depA = monkeys[name].DependsOn[0];
                var depB = monkeys[name].DependsOn[1];

                if (!yelledNumbers.ContainsKey(depA) && !yelledNumbers.ContainsKey(depB))
                {
                    continue;
                }

                var yellLeft = yelledNumbers.ContainsKey(depA);
                if (yellLeft)
                {
                    // m = yL (operation) depB
                    // m = L + A -> m - L = A
                    // m = L - A -> - (m - L) = A
                    // m = L * A -> m / L = A
                    // m = L / A -> A = L / m
                    var yL = yelledNumbers[depA];
                    long depBVal = 0;
                    switch (monkeys[name].Operation)
                    {
                        case OperationKind.Add:
                            depBVal = yelledNumbers[name] - yL;
                            break;
                        case OperationKind.Substract:
                            depBVal = -(yelledNumbers[name] - yL);
                            break;
                        case OperationKind.Multiply:
                            depBVal = yelledNumbers[name] / yL;
                            break;
                        case OperationKind.Divide:
                            depBVal = yL / yelledNumbers[name];
                            break;
                        default:
                            throw new Exception();
                    }

                    var subj = monkeys[name];
                    subj.YellValue = yelledNumbers[name];
                    subj.Operation = OperationKind.YellNumber;
                    subj.DependsOn = new string[0];
                    monkeys[name] = subj;

                    yelledNumbers[depB] = depBVal;
                }
                else
                {
                    // m = depA (operation) yR
                    // m = A + R -> m - R = A
                    // m = A - R -> m + r = A
                    // m = A * R -> m / R = A
                    // m = A / R -> m * R = A

                    var yR = yelledNumbers[depB];
                    long depAVal = 0;
                    switch (monkeys[name].Operation)
                    {
                        case OperationKind.Add:
                            depAVal = yelledNumbers[name] - yR;
                            break;
                        case OperationKind.Substract:
                            depAVal = yelledNumbers[name] + yR;
                            break;
                        case OperationKind.Multiply:
                            depAVal = yelledNumbers[name] / yR;
                            break;
                        case OperationKind.Divide:
                            depAVal = yelledNumbers[name] * yR;
                            break;
                        default:
                            throw new Exception();
                    }

                    var subj = monkeys[name];
                    subj.YellValue = yelledNumbers[name];
                    subj.Operation = OperationKind.YellNumber;
                    subj.DependsOn = new string[0];
                    monkeys[name] = subj;

                    yelledNumbers[depA] = depAVal;
                }

                nbReplaced++;
            }
        }

        return yelledNumbers[humanKey];
    }


    public enum OperationKind
    {
        YellNumber = 0,
        Add,
        Multiply,
        Divide,
        Substract
    }

    public struct MonkeyLogic
    {
        public string Name;
        public string[] DependsOn;
        public OperationKind Operation;
        public long YellValue;

        public long MonkeyBusiness(long a, long b)
        {
            switch (Operation)
            {
                case OperationKind.Add:
                    return a + b;
                case OperationKind.Substract:
                    return a - b;
                case OperationKind.Multiply:
                    return a * b;
                case OperationKind.Divide:
                    return a / b;
            }

            // impossible
            throw new ArgumentException("Invalid operation");
        }
    }

    public OperationKind MapOperator(char op)
    {
        switch (op)
        {
            case '+':
                return OperationKind.Add;
            case '-':
                return OperationKind.Substract;
            case '*':
                return OperationKind.Multiply;
            case '/':
                return OperationKind.Divide;
        }

        throw new ArgumentException("Invalid operator");
    }

    public Dictionary<string, MonkeyLogic> ParseInput()
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay21.txt");
        var monkeys = new Dictionary<string, MonkeyLogic>();


        foreach (var line in lines)
        {
            var split = line.Split(':');
            var name = split[0];

            if (long.TryParse(split[1].TrimStart(), out long value))
            {
                monkeys[name] = new MonkeyLogic
                {
                    DependsOn = new string[0],
                    Operation = OperationKind.YellNumber,
                    YellValue = value,
                    Name = name
                };
            }
            else
            {
                var monkeyA = split[1].Substring(1, 4);
                var monkeyB = split[1].Substring(1 + 4 + 3, 4);
                var op = MapOperator(split[1][1 + 4 + 1]);

                monkeys[name] = new MonkeyLogic
                {
                    DependsOn = new string[2] { monkeyA, monkeyB },
                    Operation = op,
                    Name = name
                };
            }
        }

        return monkeys;
    }
}
