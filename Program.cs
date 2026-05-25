using System.Diagnostics;
using PipsSolver.IO;

namespace PipsSolver;

class Program
{
  public static void Main(string[] args)
  {
    var (board, constraints, dominos) = new FileParser(args[0]).Parse();

    var watch = Stopwatch.StartNew();
    var solved = new Solver(board, dominos).Solve();
    watch.Stop();

    if (solved)
    {
      Console.WriteLine($"Solved in {watch.ElapsedMilliseconds} ms");
      Console.WriteLine();
      Console.WriteLine("Underlined numbers represent horizontal dominos");
      board.Print();
    }
    else
    {
      Console.WriteLine("Game is not solvable");
    }
  }
}

