using PipsSolver.Model;

namespace PipsSolver;

class Solver(Board board, List<Domino> dominos)
{
  HashSet<Domino> freeDominos = new (dominos);

  bool Solve()
  {
    return SolveAt(board.Cells[0]);
  }

  bool SolveAt(Cell cell)
  {
    foreach(var domino in freeDominos)
    {
      for (int i = 0; i < 8; i++)
      {
        if (i == 4) domino.Flip();
        
        if (board.TryAddDomino(domino, cell))
        {
          if (
            domino.Cells is not (Cell anchor, Cell other) 
            || (anchor.Constraint?.Satisfiable() ?? true)
            || (other.Constraint?.Satisfiable() ?? true)
          )
          {
            board.RemoveDomino(domino);
            continue;
          }

          freeDominos.Remove(domino);
          var neighbors = board.getDominoNeighbors(domino);
          foreach(var neighbor in neighbors)
          {
            if (SolveAt(neighbor) && board.Cells.All(c => c.Occupied || SolveAt(c))) return true;
          }

          board.RemoveDomino(domino);
          domino.Reset();
          freeDominos.Add(domino);
        }
      }
    }

    return false;
  }
}
