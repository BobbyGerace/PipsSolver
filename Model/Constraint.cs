namespace PipsSolver.Model;

abstract class Constraint
{
  public List<Cell> Cells { get; } = [];

  protected List<int> Values() => Cells.Select(cell => cell.Value).OfType<int>().ToList();
  protected bool Filled() => Cells.All(cell => cell.Value is { });

  public abstract bool Satisfiable();
}

class AllEqualConstraint : Constraint
{
  public override bool Satisfiable()
  {
    var values = Values();
    return values.All(value => value == values[0]);
  }
}

class NoneEqualConstraint : Constraint
{
  public override bool Satisfiable()
  {
    var values = Values();
    return values.Distinct().Count() == values.Count;
  }
}

class EqualNumConstraint(int num) : Constraint
{
  public override bool Satisfiable()
  {
    var sum = Values().Sum();
    return Filled() ? sum == num : sum <= num;
  }
}

class LessThanNumConstraint(int num) : Constraint
{
  public override bool Satisfiable()
  {
    return Values().Sum() < num;
  }
}

class GreaterThanNumConstraint(int num) : Constraint
{
  public override bool Satisfiable()
  {
    return !Filled() || Values().Sum() > num;
  }
}
