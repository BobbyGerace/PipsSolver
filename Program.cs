using System.Diagnostics;
using PipsSolver.IO;
using PipsSolver.Model;

namespace PipsSolver;

class Program
{
  public static void Main(string[] args)
  {
    if (args[0] == "--perf") Perf();
    else SolveFile(args[0]);
  }

  public static void SolveFile(string filePath)
  {
    if (ParseAndSolve(filePath) is not (Board board, double ms))
    {
      Console.WriteLine("Game is not solvable");
      return;
    }

    Console.WriteLine($"Solved in {ms} ms");
    new FancyPrinter(board).Print();
  }

  public static void Perf()
  {
    var files = Directory.EnumerateFiles("./examples");

    // Run once and throw away to warm up JIT
    ParseAndSolve(files.First());

    List<double> easys = [];
    List<double> mediums = [];
    List<double> hards = [];

    foreach (string filePath in files)
    {
      if (ParseAndSolve(filePath) is not (_, double ms))
      {
        throw new Exception("Unsolvable game in examples folder");
      }

      Console.WriteLine($"{filePath}: {ms}");

      if (filePath.Contains("easy")) easys.Add(ms);
      else if (filePath.Contains("medium")) mediums.Add(ms);
      else if (filePath.Contains("hard")) hards.Add(ms);
    }

    Console.WriteLine("");
    Console.WriteLine("---");
    Console.WriteLine("");

    Console.WriteLine($"Easy avg: {easys.Average()}");
    Console.WriteLine($"Medium avg: {mediums.Average()}");
    Console.WriteLine($"Hard avg: {hards.Average()}");

    var all = easys.Concat(mediums).Concat(hards);

    Console.WriteLine($"Overall avg: {all.Average()}");
    Console.WriteLine($"Max: {all.Max()}");
  }

  private static (Board board, double ms)? ParseAndSolve(string filePath)
  {
      var (board, constraints, dominos) = new FileParser(filePath).Parse();

      var watch = Stopwatch.StartNew();
      var solved = new Solver(board, dominos).Solve();
      watch.Stop();

      return (board, watch.Elapsed.TotalMilliseconds);
  }
}

