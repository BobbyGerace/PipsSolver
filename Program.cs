namespace PipsSolver;

class Program
{
  public static void Main(string[] args)
  {
    Console.WriteLine("Hello world!");
  }
}

class Board(Cell?[][] grid)
{
  public int Height { get; } = grid.Length;
  public int Width { get; } = grid[0].Length;
  public Cell?[][] Grid { get; } = grid;

  public void AddCell(Cell cell, int x, int y)
  {
    Grid[x][y] = cell;
  }

  public bool TryAddDomino(Domino domino, int x, int y)
  {
    var cells = GetCellsByDominoOrientationAndAnchor(domino.Orientation, x, y);

    if (cells is not (Cell anchor, Cell neighbor)) return false;

    if (anchor.Occupied && neighbor.Occupied) return false;

    anchor.Placement = new DominoPlacement(domino, DominoSide.Left);
    neighbor.Placement = new DominoPlacement(domino, DominoSide.Right);

    domino.Cells = cells;

    return true;
  }

  public (Cell, Cell)? GetCellsByDominoOrientationAndAnchor(DominoOrientation orientation, int x, int y) {
    var (xOffset, yOffset) = orientation switch
    {
      DominoOrientation.Right => (1, 0),
      DominoOrientation.Down => (0, 1),
      DominoOrientation.Left => (-1, 0),
      DominoOrientation.Up => (0, -1),
      _ => throw new Exception("Unknown DominoOrientation")
    };

    if (xOffset < 0 || xOffset >= Width) return null;
    if (yOffset < 0 || yOffset >= Height) return null;

    var anchorCell = Grid[x][y];
    var neighbor = Grid[x + xOffset][y + yOffset];

    if (anchorCell is Cell && neighbor is Cell) {
      return (anchorCell, neighbor);
    }
    else return null;
  }

  public void RemoveDomino(Domino domino)
  {
    if (domino.Cells is not (Cell anchor, Cell neighbor)) throw new Exception("Domino is already removed");

    anchor.Placement = null;
    neighbor.Placement = null;
    domino.Cells = null;
  }

  // This method assumes the cells have already been placed and this coordinate has one
  public void AddConstraintToCell(Constraint constraint, int x, int y)
  {
    var maybeCell = Grid[x][y];

    if (maybeCell is not Cell cell) throw new Exception($"Cell does not exist at {x},{y}");

    cell.Constraint = constraint;
    constraint.Cells.Add(cell);
  }
}

class Cell
{
  public DominoPlacement? Placement { get; set; } = null;
  public Constraint? Constraint { get; set; } = null;

  public bool Occupied { get => Placement is not null; }

  public int? Value
  {
    get { 
      if (Placement is null) return null;

      return Placement.Side == DominoSide.Left ? Placement.Domino.Left : Placement.Domino.Right;
    }
  }
}

record DominoPlacement(Domino Domino, DominoSide Side);

class Domino(int left, int right)
{
  public int Left { get; } = left;
  public int Right { get; } = right;

  public DominoOrientation Orientation { get; private set; } = DominoOrientation.Right;
  public bool Flipped { get; private set; } = false;
  public (Cell, Cell)? Cells { get; set; } = null;

  void Flip()
  {
    Flipped = !Flipped;
  }

  void Rotate()
  {
    Orientation = Orientation switch
    {
      DominoOrientation.Right => DominoOrientation.Down,
      DominoOrientation.Down => DominoOrientation.Left,
      DominoOrientation.Left => DominoOrientation.Up,
      DominoOrientation.Up => DominoOrientation.Right,
      _ => throw new Exception("Unknown Domino Orientation")
    };
  }

  void Reset()
  {
    Flipped = false;
    Orientation = DominoOrientation.Right;
  }
};

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

enum DominoSide
{
  Left,
  Right,
}

enum DominoOrientation
{
  Right,
  Down,
  Left,
  Up,
}
