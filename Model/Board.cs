using System.Diagnostics;

namespace PipsSolver.Model;

class Board(Cell?[,] grid)
{
  public int Height { get; } = grid.GetLength(0);
  public int Width { get; } = grid.GetLength(1);
  public Cell?[,] Grid { get; } = grid;
  public List<Cell> Cells { get; } = [];

  public void AddCell(Cell cell)
  {
    Grid[cell.Pos.Y, cell.Pos.X] = cell;
    Cells.Add(cell);
  }

  public bool TryAddDomino(Domino domino, Cell cell)
  {
    var (x, y) = cell.Pos;
    var cells = GetCellsByDominoOrientationAndAnchor(domino.Orientation, x, y);

    if (cells is not (Cell anchor, Cell neighbor)) return false;

    if (anchor.Occupied || neighbor.Occupied) return false;

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
      _ => throw new UnreachableException("Unknown DominoOrientation")
    };

    var neighborX = x + xOffset;
    var neighborY = y + yOffset;

    if (neighborX < 0 || neighborX >= Width) return null;
    if (neighborY < 0 || neighborY >= Height) return null;

    var anchorCell = Grid[y, x];
    var neighbor = Grid[y + yOffset, x + xOffset];

    if (anchorCell is Cell && neighbor is Cell) {
      return (anchorCell, neighbor);
    }
    else return null;
  }

  public void RemoveDomino(Domino domino)
  {
    if (domino.Cells is not (Cell anchor, Cell neighbor)) throw new InvalidOperationException("Domino is already removed");

    anchor.Placement = null;
    neighbor.Placement = null;
    domino.Cells = null;
  }

  // This method assumes the cells have already been placed and this coordinate has one
  public void AddConstraintToCell(Constraint constraint, int x, int y)
  {
    var maybeCell = Grid[y, x];

    if (maybeCell is not Cell cell) throw new InvalidOperationException($"Cell does not exist at {x},{y}");

    cell.Constraint = constraint;
    constraint.Cells.Add(cell);
  }

  // Assumes domino is placed
  public List<Cell> GetDominoNeighbors(Domino domino)
  {
    if (domino.Cells is not (Cell anchor, _)) throw new InvalidOperationException("Domino is not placed");

    var (x, y) = anchor.Pos;
    
    List<(int x, int y)> offsets = domino.Orientation switch
    {
      DominoOrientation.Right => [(2, 0), (1, 1), (0, 1), (-1, 0), (0, -1), (1, -1)],
      DominoOrientation.Down => [(1, 0), (1, 1), (0, 2), (-1, 1), (-1, 0), (0, -1)],
      DominoOrientation.Left => [(1, 0), (0, 1), (-1, 1), (-2, 0), (-1, -1), (0, -1)],
      DominoOrientation.Up => [(1, 0), (0, 1), (-1, 0), (-1, -1), (0, -2), (1, -1)],
      _ => throw new UnreachableException("Unknown DominoOrientation")
    };

    return offsets
      .Select(offset => (x: x + offset.x, y: y + offset.y))
      .Where(pos => pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height)
      .Select(pos => Grid[pos.y, pos.x])
      .OfType<Cell>()
      .ToList();
  }

  public List<Cell> FreeCells => Cells.Where(c => !c.Occupied).ToList();

  public void DebugPrint()
  {
    for (int y = 0; y < Height; y++)
    {
      for (int x = 0; x < Width; x++)
      {
        var cell = Grid[y, x];
        if (cell is null) 
        {
          Console.Write(' ');
          continue;
        }

        WriteCell(cell);
      }
      Console.WriteLine();
    }
  }

  void WriteCell(Cell cell)
  {
    if (cell.Constraint?.Satisfiable() is false) Console.ForegroundColor = ConsoleColor.Red;

    string text = cell.Value?.ToString() ?? "*";

    var isHorizontal = cell.Placement?.Domino.Orientation == DominoOrientation.Left
      || cell.Placement?.Domino.Orientation == DominoOrientation.Right;

    if (isHorizontal) WriteUnderline(text);
    else Console.Write(text);

    Console.ResetColor();
  }

  static void WriteUnderline(string s)
  {
    Console.Write("\x1B[4m" + s + "\x1B[0m");
  }
}

class Cell(int x, int y)
{
  public DominoPlacement? Placement { get; set; } = null;
  public Constraint? Constraint { get; set; } = null;

  public bool Occupied { get => Placement is not null; }

  public (int X, int Y) Pos { get; } = (x, y);

  public int? Value
  {
    get { 
      if (Placement is null) return null;

      return Placement.Side == DominoSide.Left ? Placement.Domino.Left : Placement.Domino.Right;
    }
  }

  public override string ToString()
  {
    return $"Cell({x},{y})";
  }
}

record DominoPlacement(Domino Domino, DominoSide Side);
