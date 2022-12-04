using AOC2022.Lib;

namespace AOC2022.Solutions.Day04;

[SolverMap(4)]
public class Day04 : Solver
{
    public class TeamAssignment
    {
        public HashSet<int> A;
        public HashSet<int> B;

        public TeamAssignment(string rawTeamAssignment)
        {
            var split = rawTeamAssignment.Split(',').ToArray();
            A = ParseRange(split[0]);
            B = ParseRange(split[1]);
        }
 
        private HashSet<int> ParseRange(string range)
        {
            var split = range.Split('-');
            var min = int.Parse(split[0]);
            var max = int.Parse(split[1]);

            var assignment = new HashSet<int>();
            for(int i=min;i<=max;i++){
                assignment.Add(i);
            }

            return assignment;
        }

        public bool HasCompleteOverlap()
        {
            if(B.All(b=> A.Any(a=>a==b))){
                return true;
            }

            if(A.All(a=> B.Any(b=>a==b))){
                return true;
            }

            return false;
        }

        public bool HasSomeOverlap()
        {
            return A.Intersect(B).Any();
        }
    }

    public int PartA()
    {
        var teamAssignments = ParseAssignments();
        return teamAssignments.Where(t=> t.HasCompleteOverlap()).Count();
    }

    public int PartB()
    {
        var teamAssignments = ParseAssignments();
        return teamAssignments.Where(t=> t.HasSomeOverlap()).Count();
    }

    private TeamAssignment[] ParseAssignments(bool relativeMode = false)
    {
        return File.ReadAllLines(AppContext.BaseDirectory + @"PuzzleInput\inputDay04.txt").Select(r=> new TeamAssignment(r)).ToArray();
    }
}