using System.Diagnostics;
using PipsSolver.Model;

namespace PipsSolver.IO;

using Coords = (int X, int Y);

  /**
  * Prints like this
  * ┌───────┬───┐
  * │ 1   2 │ 3 │
  * ├───────┤   │
  * │ 5   0 │ 6 │
  * └───────┴───┘
  *
  * 1. Create 4W+1 x 2H + 1 grid up front, fill with spaces
  * 2. For each cell, map coords to (4X + 2, 2Y + 1), replace with value. 
  *    Spaces pad left and right, not top / bottom.
  * 3. Draw line only if adjacent cell is part of same domino. Corners left empty
  * 4. Corners handled in second pase: then do second pass, filled in depending
  *    on neighboring sides
  */

class FancyPrinter
{
  
  char[,] OutputGrid;
  Board Board;
  int Height;
  int Width;

  public FancyPrinter(Board board)
  {
    Board = board;
    Height = 2 * Board.Height + 1;
    Width = 4 * Board.Width + 1;
    OutputGrid = BuildOutputGrid(board);
  }

  public void Print()
  {
    Console.WriteLine(ToString());
  }

  public override string ToString()
  {
    foreach (Cell cell in Board.Cells)
    {
      PaintCell(cell);
    }

    foreach (Cell cell in Board.Cells)
    {
      PaintCellCorners(cell);
    }

    var output = "";
    for (var y = 0; y < Height; y++)
    {
      for (var x = 0; x < Width; x++)
      {
        output += OutputGrid[y, x];
      }

      if (y < Height - 1) output += Environment.NewLine;
    }

    return output;
  }

  char[,] BuildOutputGrid(Board board)
  {
    var grid = new char[Height, Width];

    for (var y = 0; y < Height; y++)
    {
      for (var x = 0; x < Width; x++)
      {
        grid[y, x] = ' ';
      }
    }

    return grid;
  }   

  void PaintCell(Cell cell)
  {
    var upCell = SafeGet(Board.Grid, cell.Pos.X, cell.Pos.Y - 1, null);
    var rightCell = SafeGet(Board.Grid, cell.Pos.X + 1, cell.Pos.Y, null);
    var downCell = SafeGet(Board.Grid, cell.Pos.X, cell.Pos.Y + 1, null);
    var leftCell = SafeGet(Board.Grid, cell.Pos.X - 1, cell.Pos.Y, null);

    var (x, y) = MapCoords(cell.Pos);
  
    if (cell.Value is not { } value) throw new InvalidOperationException("No-value cell during print");

    SafeSet(OutputGrid, x, y, Char.Parse(value.ToString()));

    if (cell.Placement?.Domino != upCell?.Placement?.Domino) 
    {
      SafeSet(OutputGrid, x, y - 1, '─');
      SafeSet(OutputGrid, x - 1, y - 1, '─');
      SafeSet(OutputGrid, x + 1, y - 1, '─');
    }

    if (cell.Placement?.Domino != downCell?.Placement?.Domino) 
    {
      SafeSet(OutputGrid, x, y + 1, '─');
      SafeSet(OutputGrid, x - 1, y + 1, '─');
      SafeSet(OutputGrid, x + 1, y + 1, '─');
    }
    
    if (cell.Placement?.Domino != leftCell?.Placement?.Domino) SafeSet(OutputGrid, x - 2, y, '│');

    if (cell.Placement?.Domino != rightCell?.Placement?.Domino) SafeSet(OutputGrid, x + 2, y, '│');
  }

  T SafeGet<T>(T?[,] grid, int x, int y, T defaultValue)
  {
    if ( x < 0 || x >= grid.GetLength(1) || y < 0 || y >= grid.GetLength(0)) return defaultValue;
    
    return grid[y, x]!;
  }
  void SafeSet<T>(T?[,] grid, int x, int y, T value)
  {
    if ( x < 0 || x >= grid.GetLength(1) || y < 0 || y >= grid.GetLength(0)) return;

    grid[y, x] = value;
  }

  void PaintCellCorners(Cell cell)
  {
    var (cx, cy) = MapCoords(cell.Pos);

    (int x, int y)[] corners = [
      (cx - 2, cy - 1),
      (cx + 2, cy - 1),
      (cx + 2, cy + 1),
      (cx - 2, cy + 1),
    ];

    foreach (var (x, y) in corners)
    {
      var up = SafeGet(OutputGrid, x, y - 1, ' ');
      var down = SafeGet(OutputGrid, x, y + 1, ' ');
      var left = SafeGet(OutputGrid, x - 1, y, ' ');
      var right = SafeGet(OutputGrid, x + 1, y, ' ');
      
      char toWrite = (up, down, left, right) switch
      {
        ('│', '│', '─', '─') => '┼',
        (' ', '│', '─', '─') => '┬',
        ('│', ' ', '─', '─') => '┴',
        ('│', '│', ' ', '─') => '├',
        ('│', '│', '─', ' ') => '┤',
        (' ', ' ', '─', '─') => '─',
        ('│', '│', ' ', ' ') => '│',
        ('│', ' ', '─', ' ') => '┘',
        (' ', '│', ' ', '─') => '┌',
        ('│', ' ', ' ', '─') => '└',
        (' ', '│', '─', ' ') => '┐',
        _ => throw new UnreachableException($"Invalid domino configuration: {up}, {down}, {left}, {right}"),
      };

      SafeSet(OutputGrid, x, y, toWrite);
    }
  }

  Coords MapCoords(Coords coords) => (4 * coords.X + 2, 2 * coords.Y + 1);
}
