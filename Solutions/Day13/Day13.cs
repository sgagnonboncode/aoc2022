using System.Text;
using System.Text.RegularExpressions;
using AOC2022.Lib;

namespace AOC2022.Solutions.Day13;

//
// !!! Challenge round !!!
//
//  Felt like reinventing the wheel and writing a Json parser from scratch
//  Otherwise would have been a VERY SHORT program with JsonValue and JsonArray
//
[SolverMap(13)]
public class Day13 : Solver
{
    public static string PopArray(string str)
    {
        var rTrim = str.Substring(0, str.Length - 1);
        return rTrim.Substring(1);
    }

    // This is needlessly complicated because I made the RecursiveCompare comparator pop the first and last bracket as a first operation.
    //  Tons of edge cases like "[7,8],5" to handle, which are not an array per definition.
    public static bool ValueIsArray(string str)
    {
        if (!str.StartsWith("[") || !str.EndsWith("]"))
        {
            return false;
        }

        var arr = str.ToCharArray();
        int level = 0;

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == '[')
            {
                if (level == 0 && i > 0)
                {
                    return false;
                }
                level++;
            }
            else if (arr[i] == ']')
            {
                level--;
            }
        }

        return level == 0;
    }

    // Needed because of edge cases like "[7,8],5" -> need to know if theres an array somewhere in the string.
    public static bool ValueHasBrackets(string str)
    {
        return str.Any(c => c == '[' || c == ']');
    }

    public static string[] TokenizeValue(string str)
    {
        var tokens = new List<string>();
        string[] split = str.Split(',');

        bool arrMode = false;
        int level = 0;
        var accumulator = new StringBuilder();
        for (int i = 0; i < split.Length; i++)
        {
            if (arrMode)
            {
                accumulator.Append(",");
                accumulator.Append(split[i]);

                if (split[i].StartsWith("[") || split[i].EndsWith("]"))
                {
                    level += split[i].Count(c => c == '[') - split[i].Count(c => c == ']');
                    if (level > 0)
                    {
                        continue;
                    }

                    tokens.Add(accumulator.ToString());
                    accumulator.Clear();
                    arrMode = false;
                }
                continue;
            }

            if (split[i].StartsWith("["))
            {
                level += split[i].Count(c => c == '[') - split[i].Count(c => c == ']');
                if (level == 0)
                {
                    tokens.Add(split[i]);
                    continue;
                }

                accumulator.Append(split[i]);
                arrMode = true;
                continue;
            }

            tokens.Add(split[i]);
        }

        return tokens.ToArray();
    }

    public int RecursiveCompare(string a, string b)
    {
        // Handling empty arrays "[]" comparison 
        //   A side effect of popping the external brackets ...
        var aEmpty = string.IsNullOrWhiteSpace(a);
        var bEmpty = string.IsNullOrWhiteSpace(b);
        if (aEmpty && bEmpty)
        {
            return 0;
        }
        if (aEmpty && !bEmpty)
        {
            return -1;
        }
        if (!aEmpty && bEmpty)
        {
            return 1;
        }

        // Handling value vs array comparison
        var aIsArr = ValueIsArray(a);
        var bIsArr = ValueIsArray(b);

        if (aIsArr && bIsArr)
        {
            // In retrospect, this line makes the array parsing needlessly complicated because it generates a ton of edge cases.
            return RecursiveCompare(PopArray(a), PopArray(b));
        }
        if (aIsArr && !bIsArr && !ValueHasBrackets(b))
        {
            return RecursiveCompare(a, "[" + b + "]");
        }
        if (!aIsArr && bIsArr && !ValueHasBrackets(a))
        {
            return RecursiveCompare("[" + a + "]", b);
        }

        // Tokenize and compare tokens
        var tokensA = TokenizeValue(a);
        var tokensB = TokenizeValue(b);

        for (int i = 0; i < tokensA.Length; i++)
        {
            if (i >= tokensB.Length)
            {
                return 1;
            }

            var isArrTA = ValueIsArray(tokensA[i]);
            var isArrTB = ValueIsArray(tokensB[i]);

            if (isArrTA && isArrTB)
            {
                // In retrospect, this line makes the array parsing needlessly complicated because it generates a ton of edge cases.
                var cmp = RecursiveCompare(PopArray(tokensA[i]), PopArray(tokensB[i]));
                if (cmp != 0)
                {
                    return cmp;
                }
            }
            else if (isArrTA && !isArrTB && !ValueHasBrackets(tokensB[i]))
            {
                var cmp = RecursiveCompare(tokensA[i], "[" + tokensB[i] + "]");
                if (cmp != 0)
                {
                    return cmp;
                }
            }
            else if (!isArrTA && isArrTB && !ValueHasBrackets(tokensA[i]))
            {
                var cmp = RecursiveCompare("[" + tokensA[i] + "]", tokensB[i]);
                if (cmp != 0)
                {
                    return cmp;
                }
            }
            else
            {
                var cmp = int.Parse(tokensA[i]).CompareTo(int.Parse(tokensB[i]));
                if (cmp != 0)
                {
                    return cmp;
                }
            }
        }

        return tokensA.Length == tokensB.Length ? 0 : -1;
    }


    public object PartA()
    {
        var pairs = ParseInputPartA();
        int tally = 0;
        for (int i = 0; i < pairs.Length; i++)
        {
            var cmp = RecursiveCompare(pairs[i].Left, pairs[i].Right);
            if (cmp <= 0)
            {
                tally += (i + 1);
            }
        }

        return tally;
    }

    public object PartB()
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay13.txt").Where(s => !string.IsNullOrWhiteSpace(s));
        var markers = new string[] { "[[2]]", "[[6]]" };
        var packets = new List<string>(lines);
        packets.AddRange(markers);
        packets.Sort((a, b) => RecursiveCompare(a, b));

        var sig2 = packets.IndexOf(markers[0]);
        var sig6 = packets.IndexOf(markers[1]);
        return (sig2 + 1) * (sig6 + 1);
    }

    public struct SignalPair
    {
        public string Left;
        public string Right;
    }

    public SignalPair[] ParseInputPartA()
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay13.txt");
        var pairs = new List<SignalPair>();
        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                pairs.Add(new SignalPair { Left = lines[i - 2], Right = lines[i - 1] });
            }
        }
        pairs.Add(new SignalPair { Left = lines[lines.Length - 2], Right = lines[lines.Length - 1] });

        return pairs.ToArray();
    }
}