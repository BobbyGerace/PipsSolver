using PipsSolver.Model;

namespace PipsSolver.IO;

class FileParser(string path)
{
  
  public (Board, List<Domino>) Parse()
  {
    using StreamReader reader = new StreamReader(path);

    List<string> cellLines = ReadSection(reader);
    List<string> constraintLines = ReadSection(reader);
    List<string> dominoLines = ReadSection(reader);

    var constraints = ParseConstraints(constraintLines);
    var grid = ParseGrid(cellLines, constraints);
    var dominos = ParseDominos(dominoLines);

    return (grid, dominos);
  }

  List<string> ReadSection(StreamReader reader)
  {
    List<string> output = [];
    while (reader.ReadLine() is { } line && !line.StartsWith("---"))
    {
      output.Add(line);
    }

    // remove empty padding at beginning and end
    return output
        .SkipWhile(string.IsNullOrWhiteSpace)
        .Reverse()
        .SkipWhile(string.IsNullOrWhiteSpace)
        .Reverse()
        .ToList();
  }

  Dictionary<char, Constraint> ParseConstraints(List<string> strs)
  {
    Dictionary<char, Constraint> constraintMap = new();

    foreach (var str in strs)
    {
      if (str.Trim().Length == 0) continue; 

      var splitStr = str.Split(':', 2, StringSplitOptions.TrimEntries);
      var name = splitStr.ElementAtOrDefault(0);
      var exp = splitStr.ElementAtOrDefault(1);

      var splitExp = exp!.Split(' ', 2, StringSplitOptions.TrimEntries);
      var op = splitExp.ElementAtOrDefault(0);
      var val = splitExp.ElementAtOrDefault(1);

      Constraint constraint = (op, val) switch
      {
        ("==", _) => new AllEqualConstraint(),
        ("!=", _) => new NoneEqualConstraint(),
        ("=", string value) => new EqualNumConstraint(int.Parse(value)),
        (">", string value) => new GreaterThanNumConstraint(int.Parse(value)),
        ("<", string value) => new LessThanNumConstraint(int.Parse(value)),
        _ => throw new Exception($"Unknown constraint {str}")
      };

      constraintMap.Add(name![0], constraint);
    }

    return constraintMap;
  }

  Board ParseGrid(List<string> strs, Dictionary<char, Constraint> constraintMap)
  {
    int Height = strs.Count;
    int Width = strs.Select(s => s.Length).Max();
    
    var board = new Board(new Cell?[Height, Width]);

    for (int y = 0; y < strs.Count; y++)
    {
      var str = strs[y];
      for (int x = 0; x < str.Length; x++)
      {
        char c = strs[y][x];
        if (c == ' ') continue;

        board.AddCell(new Cell(x, y));
        Constraint? constraint = null;
        if ((constraint = constraintMap.GetValueOrDefault(c)) is not null)
        {
          board.AddConstraintToCell(constraint, x, y);
        }
      }
    }

    return board;
  }

  List<Domino> ParseDominos(List<string> strs)
  {
    List<Domino> dominos = []; 
    foreach (var str in strs)
    {
      if (str.Trim().Length == 0) continue; 

      var nums = str.Split(' ', 2, StringSplitOptions.TrimEntries);
      var left = int.Parse(nums.ElementAt(0));
      var right = int.Parse(nums.ElementAt(1));
      dominos.Add(new Domino(left, right));
    }

    return dominos;
  }
}
