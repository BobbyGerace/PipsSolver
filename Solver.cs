using PipsSolver.Model;

namespace PipsSolver;

class Solver(Board board, List<Domino> dominos)
{

  public bool Solve()
  {
    board.Print();
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
        
        Console.WriteLine($"Trying {domino} on {cell}");
        if (board.TryAddDomino(domino, cell))
        {
          board.Print();
          if (
            domino.Cells is not (Cell anchor, Cell other) 
            || !(anchor.Constraint?.Satisfiable() ?? true)
            || !(other.Constraint?.Satisfiable() ?? true)
          )
          {
            Console.WriteLine("Failed constraints!");
            board.RemoveDomino(domino);
            continue;
          }


          var neighbors = board.getDominoNeighbors(domino);
          Console.WriteLine($"Found neighbors: {neighbors}");
          Console.WriteLine($"Vacant neighbor count: {neighbors.Count(n => !n.Occupied)}");
          Console.WriteLine($"Vacant cell count: {board.Cells.Count(n => !n.Occupied)}");
          foreach (var c in board.Cells.Where(c => !c.Occupied)) Console.WriteLine(c.Pos);
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
