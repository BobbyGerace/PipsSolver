namespace PipsSolver.Model;

abstract class Constraint
{
  public List<Cell> Cells { get; set; } = [];

  protected List<int> Values() => Cells.Select(cell => cell.Value).OfType<int>().ToList();
  protected bool Filled() => Cells.All(cell => cell.Value is not null);

  abstract public bool Satisfied();
  abstract public bool Satisfiable();
}

class AllEqualConstraint : Constraint
{
  public override bool Satisfied()
  {
    var values = Values();
    return Filled() && values.All(value => value == values[0]);
  }

  public override bool Satisfiable()
  {
    var values = Values();
    return values.All(value => value == values[0]);
  }
}

class NoneEqualConstraint : Constraint
{
  public override bool Satisfied()
  {
    return Filled() && Values().Distinct().Count() == Values().Count;
  }

  public override bool Satisfiable()
  {
    return Values().Distinct().Count() == Values().Count;
  }
}

class EqualNumConstraint(int num) : Constraint
{
  public override bool Satisfied()
  {
    return Filled() && Values().Sum() == num;
  }

  public override bool Satisfiable()
  {
    return Values().Sum() <= num;
  }
}

class LessThanNumConstraint(int num) : Constraint
{
  public override bool Satisfied()
  {
    return Filled() && Values().Sum() < num;
  }

  public override bool Satisfiable()
  {
    return Values().Sum() < num;
  }
}

class GreaterThanNumConstraint(int num) : Constraint
{
  public override bool Satisfied()
  {
    return Filled() && Values().Sum() > num;
  }

  public override bool Satisfiable()
  {
    return !Filled() || Values().Sum() > num;
  }
}
