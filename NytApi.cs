using PipsSolver.Model;
using System.Net.Http.Json;

namespace PipsSolver;

static class NytApi
{
  private static readonly HttpClient client = new()
  {
    BaseAddress = new Uri("https://www.nytimes.com"),
  };

  public static async Task<(Board, List<Domino>)> GetBoardFromDate(Difficulty difficulty, string date)
  {
    using HttpResponseMessage response = await client.GetAsync($"/svc/pips/v1/{date}.json");

    response.EnsureSuccessStatusCode();

    var body = await response.Content.ReadFromJsonAsync<JsonResponseDto>();

    if (body is null) throw new Exception("Invalid JSON");
    
    Console.WriteLine($"Received data for {body.PrintDate}");

    var game = difficulty switch
    {
      Difficulty.Easy => body.Easy,
      Difficulty.Medium => body.Medium,
      Difficulty.Hard => body.Hard,
      _ => throw new Exception("Invalid difficulty"),
    };

    int Height = game.Regions.SelectMany(r => r.Indices).Max(i => i[0]) + 1;
    int Width = game.Regions.SelectMany(r => r.Indices).Max(i => i[1]) + 1;
    
    var board = new Board(new Cell?[Height, Width]);
    List<Constraint> constraints = [];

    foreach (var region in game.Regions)
    {
      Constraint? constraint = FromJsonType(region.Type, region.Target);

      foreach(var coords in region.Indices)
      {
        var x = coords[1];
        var y = coords[0];
        Cell cell = new(x, y);
        board.AddCell(cell);
        if (constraint is not null) board.AddConstraintToCell(constraint, x, y);
      }
    }
    
    var dominos = game.Dominoes.Select(dom => new Domino(dom[0], dom[1])).ToList();

    return (board, dominos);
  }

  private static Constraint? FromJsonType(string type, int? target)
  {
    return (type, target) switch
    {
      ("equals", _) => new AllEqualConstraint(),
      ("unequal", _) => new NoneEqualConstraint(),
      ("sum", int val) => new EqualNumConstraint(val),
      ("less", int val) => new LessThanNumConstraint(val),
      ("greater", int val) => new GreaterThanNumConstraint(val),
      ("empty", _) => null,
      _ => throw new Exception("Unknown json region type"),
    };
  }
}

record JsonResponseDto(string PrintDate, GameDataDto Easy, GameDataDto Medium, GameDataDto Hard);

record GameDataDto(List<List<int>> Dominoes, List<RegionDto> Regions);

record RegionDto(List<List<int>> Indices, string Type, int? Target);

enum Difficulty
{
  Easy,
  Medium,
  Hard
}
