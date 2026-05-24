namespace PipsSolver.Model;

class Board(Cell?[,] grid)
{
  public int Width { get; } = grid.GetLength(0);
  public int Height { get; } = grid.GetLength(1);
  public Cell?[,] Grid { get; } = grid;
  public List<Cell> Cells { get; } = [];

  public void AddCell(Cell cell)
  {
    Grid[cell.Pos.X, cell.Pos.Y] = cell;
    Cells.Add(cell);
  }

  public bool TryAddDomino(Domino domino, Cell cell)
  {
    var (x, y) = cell.Pos;
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

    var anchorCell = Grid[x, y];
    var neighbor = Grid[x + xOffset, y + yOffset];

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
    var maybeCell = Grid[x, y];

    if (maybeCell is not Cell cell) throw new Exception($"Cell does not exist at {x},{y}");

    cell.Constraint = constraint;
    constraint.Cells.Add(cell);
  }

  // Assumes domino is placed
  public List<Cell> getDominoNeighbors(Domino domino)
  {
    if (domino.Cells is not (Cell anchor, _)) throw new Exception("Domino is not placed");

    var (x, y) = anchor.Pos;
    
    List<(int x, int y)> offsets = domino.Orientation switch
    {
      DominoOrientation.Right => [(2, 0), (1, 1), (0, 1), (-1, 0), (0, -1), (1, -1)],
      DominoOrientation.Down => [(1, 0), (1, 1), (0, 2), (-1, 1), (0, -1), (0, -1)],
      DominoOrientation.Left => [(1, 0), (0, 1), (-1, 1), (-2, 0), (-1, -1), (0, -1)],
      DominoOrientation.Up => [(1, 0), (0, 1), (0, -1), (-1, -1), (0, -2), (1, -1)],
      _ => throw new Exception("Unknown DominoOrientation")
    };

    return offsets.Select(offset => Grid[x + offset.x, y + offset.y]).OfType<Cell>().ToList();
  }

  public List<Cell> FreeCells => Cells.Where(c => !c.Occupied).ToList();
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
}

record DominoPlacement(Domino Domino, DominoSide Side);
