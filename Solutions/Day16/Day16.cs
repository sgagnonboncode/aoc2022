using System.Text;
using System.Text.RegularExpressions;
using AOC2022.Lib;

namespace AOC2022.Solutions.Day16;

[SolverMap(16)]
public class Day16 : Solver
{

    public const int infinity = 999999999;
    public Dictionary<string, HashSet<string>> adjacency = new Dictionary<string, HashSet<string>>();
    public int[,] nodeDist = new int[0, 0];
    public string[] names = new string[0];
    public Dictionary<string, int> namesIndex = new Dictionary<string, int>();

    public int[] nzNodes = new int[0];
    public int[] nodesFlow = new int[0];

    public void ComputeNodeDistances(Dictionary<string, ValveDefinition> defs)
    {
        names = defs.Keys.OrderBy(n => n).ToArray();
        nodesFlow = new int[names.Length];

        for (int n = 0; n < names.Length; n++)
        {
            namesIndex[names[n]] = n;
            nodesFlow[n] = defs[names[n]].FlowRate;
        }
        nzNodes = defs.Where(kv => kv.Value.FlowRate > 0).Select(kv => namesIndex[kv.Key]).ToArray();

        nodeDist = new int[0, 0];
        foreach (var d in defs)
        {
            adjacency[d.Key] = new HashSet<string>(d.Value.Connections);
        }

        // Floyd-Warshall
        nodeDist = new int[names.Length, names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            for (int j = 0; j < names.Length; j++)
            {
                if (i == j)
                {
                    nodeDist[i, j] = 0;
                }
                else if (adjacency[names[i]].Contains(names[j]))
                {
                    nodeDist[i, j] = 1;
                    nodeDist[j, i] = 1;
                }
                else
                {
                    nodeDist[i, j] = infinity;
                    nodeDist[j, i] = infinity;
                }
            }
        }

        for (int k = 0; k < names.Length; k++)
        {
            for (int i = 0; i < names.Length; i++)
            {
                for (int j = 0; j < names.Length; j++)
                {
                    if (nodeDist[i, j] > nodeDist[i, k] + nodeDist[k, j])
                    {
                        nodeDist[i, j] = nodeDist[i, k] + nodeDist[k, j];
                    }
                }
            }
        }

    }

    public (int, string) RecursiveSolvePartA(int current, int tLeft, int visitable, int cummulFlow, string path)
    {
        var newFlow = current == 0 ? 0 : cummulFlow + (tLeft * nodesFlow[current]);

        int bestVal = -1;
        int bestIdx = -1;
        string bestPath = "";
        if (visitable != 0)
        {
            for (int i = 0; i < nzNodes.Length; i++)
            {
                int flag = 1 << i;
                if ((visitable & flag) == 0)
                {
                    continue;
                }

                var dist = nodeDist[current, nzNodes[i]];
                if (dist > tLeft || dist == 0)
                {
                    continue;
                }


                (var cur, var p) = RecursiveSolvePartA(nzNodes[i], tLeft - dist - 1, visitable - flag, newFlow, path);

                if (cur > bestVal)
                {
                    bestIdx = i;
                    bestVal = cur;
                    bestPath = p;
                }
            }
        }

        if (visitable == 0 || bestIdx == -1)
        {
            return (newFlow, names[current] + path);
        }

        if (current == 0)
        {
            return (bestVal, bestPath);
        }

        return (bestVal, names[current] + bestPath);
    }


    public Dictionary<int, int> pathValues = new Dictionary<int, int>();

    public void RecursiveExplorePartB(int current, int tLeft, int visitable, int cummulFlow)
    {
        pathValues[visitable] = int.Max(pathValues.GetValueOrDefault(visitable, 0), cummulFlow);

        for (int i = 0; i < nzNodes.Length; i++)
        {
            int flag = 1 << i;
            if ((visitable & flag) == 0)
            {
                continue;
            }

            var dist = nodeDist[current, nzNodes[i]];
            var remaining = tLeft - dist - 1;
            if (remaining < 0)
            {
                continue;
            }

            var newFlow = cummulFlow + (remaining * nodesFlow[nzNodes[i]]);
            RecursiveExplorePartB(nzNodes[i], remaining, visitable - flag, newFlow);
        }
    }

    public object PartA()
    {
        var defs = ParseInput();
        ComputeNodeDistances(defs);

        //bitmask des nodes non-zero
        int visitable = 0;
        for (int i = 0; i < nzNodes.Length; i++)
        {
            visitable += 1 << i;
        }

        return RecursiveSolvePartA(namesIndex["AA"], 30, visitable, 0, "");
    }

    public object PartB()
    {
        var defs = ParseInput();
        ComputeNodeDistances(defs);

        //bitmask des nodes non-zero
        int visitable = 0;
        for (int i = 0; i < nzNodes.Length; i++)
        {
            visitable += 1 << i;
        }

        RecursiveExplorePartB(namesIndex["AA"], 26, visitable, 0);

        int best = -1;
        foreach (var kvA in pathValues.Where(kv=>kv.Key >0 && kv.Key < visitable))
        {
            var cA = visitable - kvA.Key;

            foreach (var kvB in pathValues.Where(kv=>kv.Key >0 && kv.Key < visitable))
            {
                var cB = visitable - kvB.Key;

                if ( (cA & cB) != 0)
                {
                    // on cherche un complement elephant + player tel que les nodes ne sont
                    // visitees qu une seule fois
                    continue;
                }

                var cur = kvA.Value + kvB.Value;
                if (cur > best)
                {
                    // Console.WriteLine("{0}({1}) + {2}({3}) = {4}   {5}", kvA.Key, kvA.Value, kvB.Key, kvB.Value, cur, kvA.Key & kvB.Key);
                    best = cur;
                }
            }
        }
        return best;
    }

    public struct ValveDefinition
    {
        public string Name;
        public int FlowRate;
        public string[] Connections;
    }

    public Dictionary<string, ValveDefinition> ParseInput()
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay16.txt");
        var defs = new Dictionary<string, ValveDefinition>();
        var pattern = @"Valve (.+) has flow rate=(.+); tunnel.? lead.? to valve.? (.+)";

        foreach (var line in lines)
        {
            var matches = Regex.Matches(line, pattern);
            foreach (Match match in matches)
            {
                var n = match.Groups[1].Value;
                defs[n] = new ValveDefinition
                {
                    Name = n,
                    FlowRate = int.Parse(match.Groups[2].Value),
                    Connections = match.Groups[3].Value.Split(',').Select(s => s.Trim()).ToArray()
                };
            }
        }

        return defs;
    }
}