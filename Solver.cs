using PipsSolver.Model;

namespace PipsSolver;

class Solver(Board board, List<Domino> dominos)
{

  public bool Solve()
  {
    HashSet<Domino> freeDominos = new (dominos);
    return SolveAt(board.Cells[0], freeDominos);
  }

  bool SolveAt(Cell cell, HashSet<Domino> freeDominos)
  {
    foreach(var domino in freeDominos)
    {
      HashSet<Domino> nextFreeDominos = new (freeDominos);
      nextFreeDominos.Remove(domino);

      for (int i = 0; i < 8; i++)
      {
        if (i != 0) domino.Rotate();
        if (i == 4) domino.Flip();
        
        if (board.TryAddDomino(domino, cell))
        {
          if (Environment.GetEnvironmentVariable("DEBUG") == "true")
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


          var neighbors = board.getDominoNeighbors(domino);
          if (
            neighbors.All(n => n.Occupied || SolveAt(n, nextFreeDominos)) 
            && board.Cells.All(c => c.Occupied || SolveAt(c, nextFreeDominos))
          ) return true;
  
          board.RemoveDomino(domino);
        }
      }
      
      domino.Reset();
    }

    return false;
  }
}
