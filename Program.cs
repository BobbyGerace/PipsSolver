using PipsSolver.IO;

namespace PipsSolver;

class Program
{
  public static void Main(string[] args)
  {
    var (board, constraints, dominos) = new FileParser(args[0]).Parse();

    Console.WriteLine(new Solver(board, dominos).Solve());
    // Console.WriteLine($"""
    //   Board: {board.Grid.GetLength(0)} x {board.Grid.GetLength(1)}
    //   Constraints: {constraints.Count}
    //   Dominos: {dominos.Count}
    // """);
  }
}

