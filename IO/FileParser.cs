using PipsSolver.Model;

namespace PipsSolver.IO;

class FileParser(string path)
{
  
  public Board Parse()
  {
    List<string> cellLines = [];
    List<string> constraintLines = [];
    List<string> dominoLines = [];

    using (StreamReader reader = new StreamReader(path))
    {
      cellLines = ReadNonEmptyLines(reader);
      constraintLines = ReadNonEmptyLines(reader);
      dominoLines = ReadNonEmptyLines(reader);
    }

    var constraints = ParseConstraints(constraintLines);
    var grid = ParseGrid(cellLines, constraints);

    throw new Exception("TODO");
  }

  List<string> ReadNonEmptyLines(StreamReader reader)
  {
    List<string> output = [];
    string? line = null;
    while ((line = reader.ReadLine()) is string && line.Length > 0)
    {
      output.Add(line);
    }

    return output;
  }

  Dictionary<char, Constraint> ParseConstraints(List<string> strs)
  {
    Dictionary<char, Constraint> constraintMap = new();

    foreach (var str in strs)
    {
      var splitStr = str.Split(':', 2, StringSplitOptions.TrimEntries);
      var name = splitStr.ElementAtOrDefault(0);
      var exp = splitStr.ElementAtOrDefault(1);

      var splitExp = str.Split(' ', 2, StringSplitOptions.TrimEntries);
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
    int Width = strs.Count;
    int Height = strs.Select(s => s.Length).Max();
    
    var board = new Board(new Cell?[Width, Height]);

    for (int x = 0; x < strs.Count; x++)
    {
      var str = strs[x];
      for (int y = 0; y < str[y]; y++)
      {
        char c = strs[x][y];
        if (c == ' ') continue;

        board.AddCell(new Cell(), x, y);
        Constraint? constraint = null;
        if ((constraint = constraintMap.GetValueOrDefault(c)) is not null)
        {
          board.AddConstraintToCell(constraint, x, y);
        }
      }
    }

    return board;
  }
}
