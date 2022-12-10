using System.Text.RegularExpressions;
using AOC2022.Lib;

namespace AOC2022.Solutions.Day10;


[SolverMap(10)]
public class Day10 : Solver
{
    public object PartA()
    {
        var lines = ParseInput();

        var signalPoints = new List<int>();
        var measurementPoints = new HashSet<int> { 20, 60, 100, 140, 180, 220 };

        int cycle = 0;
        int row = -1;
        int addRegister = 0;
        int x = 1;

        bool finalizeAddX = false;
        while (cycle < 220)
        {
            cycle++;
            if (measurementPoints.Contains(cycle))
            {
                signalPoints.Add(cycle * x);
            }

            // finalize multi-cycle instructions
            if (finalizeAddX)
            {
                x += addRegister;
                finalizeAddX = false;
                continue;
            }

            // advance program
            row++;
            if (lines.Length <= row)
            {
                // end of program
                Console.WriteLine("End of program");
                break;
            }

            var l = lines[row];
            if (l.StartsWith("addx"))
            {
                addRegister = int.Parse(l.Split(' ')[1]);
                finalizeAddX = true;
            }
        }

        return signalPoints.Sum();
    }

    public object PartB()
    {
        var lines = ParseInput();

        int cycle = 0;
        int programRow = -1;
        int addRegister = 0;
        int x = 1;
        bool finalizeAddX = false;

        string[] buffer = new string[6] { "", "", "", "", "", "" };
        while (cycle < 240)
        {
            cycle++;

            // drawing
            int displayRow = (cycle-1) / 40;
            int displayColumn = (cycle-1) % 40;
            int diff = x - displayColumn;
            bool spriteVisible = diff == -1 || diff == 0 || diff == 1;
            buffer[displayRow]+= spriteVisible ? "#":".";

            // finalize multi-cycle instructions
            if (finalizeAddX)
            {
                x += addRegister;
                finalizeAddX = false;
            }
            else
            {
                // advance program
                programRow++;
                if (lines.Length <= programRow)
                {
                    // end of program
                    Console.WriteLine("End of program");
                    break;
                }

                var l = lines[programRow];
                if (l.StartsWith("addx"))
                {
                    addRegister = int.Parse(l.Split(' ')[1]);
                    finalizeAddX = true;
                }
            }
        }

        // this solution needs human parsing !  (my value was RFKZCPEF)
        return '\n' + string.Join('\n',buffer).Replace("."," ");
    }

    public string[] ParseInput()
    {
        var lines = File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay10.txt");
        return lines;
    }
}