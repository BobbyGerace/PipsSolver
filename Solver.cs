using PipsSolver.Model;

namespace PipsSolver;

class Solver(Board board, List<Domino> dominos)
{
  static readonly bool Debug = Environment.GetEnvironmentVariable("DEBUG") == "true";

  public bool Solve()
  {
    return SolveAt(board.Cells[0]);
  }

  bool SolveAt(Cell cell)
  {
    foreach(var domino in dominos)
    {
      if (domino.Placed()) continue;

      // Skip the flip for dominos with equal sides
      var iterations = domino.Left == domino.Right ? 4 : 8;
      for (int i = 0; i < iterations; i++)
      {
        if (i != 0) domino.Rotate();
        if (i == 4) domino.Flip();
        
        if (board.TryAddDomino(domino, cell))
        {
          if (Debug)
          {
            board.Print();
            Console.WriteLine("---");
          }

          if (
            domino.Cells is not (Cell anchor, Cell other) 
            || !(anchor.Constraint?.Satisfiable() ?? true)
            || !(other.Constraint?.Satisfiable() ?? true)
          )
          {
            board.RemoveDomino(domino);
            continue;
          }

          var next = GetBestNextCell(domino);
          if (next is null || SolveAt(next)) return true;
  
          board.RemoveDomino(domino);
        }
      }
      
      domino.Reset();
    }

    return false;
  }

  Cell? GetBestNextCell(Domino domino)
  {
    var neighbors = board.GetDominoNeighbors(domino);

    if (domino.Cells is not (Cell anchor, Cell other))
    {
      throw new InvalidOperationException("Domino is not placed");
    }

    var leftCon  = anchor.Constraint;
    var rightCon = other.Constraint;

    Cell? anyNeighbor = null, hasConstraint = null;
    foreach (var neighbor in neighbors)
    {
      if (neighbor.Occupied) continue;

      if (neighbor.Constraint is { } nc && (nc == leftCon || nc == rightCon)) return neighbor;
      else if (neighbor.Constraint is { }) hasConstraint = neighbor;
      else anyNeighbor = neighbor;
    }

    if (hasConstraint is { }) return hasConstraint;
    if (anyNeighbor is { }) return anyNeighbor;

    return board.Cells.Find(c => !c.Occupied);
  }
}
