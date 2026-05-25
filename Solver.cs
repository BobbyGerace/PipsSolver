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

      for (int i = 0; i < 8; i++)
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


          var neighbors = board.GetDominoNeighbors(domino);
          if (
            neighbors.All(n => n.Occupied || SolveAt(n)) 
            && board.Cells.All(c => c.Occupied || SolveAt(c))
          ) return true;
  
          board.RemoveDomino(domino);
        }
      }
      
      domino.Reset();
    }

    return false;
  }
}
