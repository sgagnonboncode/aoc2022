namespace AOC2022.Lib;

[AttributeUsage(AttributeTargets.Class)]
public class SolverMapAttribute: Attribute {
    public int Day;

    public SolverMapAttribute(int day){
        Day = day;
    }
}

public interface Solver {
    object PartA();
    object PartB();
}