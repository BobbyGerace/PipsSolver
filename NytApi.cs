using PipsSolver.Model;
using System.Net.Http.Json;

namespace PipsSolver;

static class NytApi
{
  private static HttpClient client = new()
  {
    BaseAddress = new Uri("https://www.nytimes.com"),
  };

  public static async Task<(Board, List<Constraint>, List<Domino>)> GetBoardFromDate()
  {
    using HttpResponseMessage response = await client.GetAsync("/svc/pips/v1/2026-06-11.json");

    response.EnsureSuccessStatusCode();

    var body = await response.Content.ReadFromJsonAsync<JsonResponseDto>();

    if (body is null) throw new Exception("Invalid JSON");
    
    Console.WriteLine($"Received data for {body.PrintDate}");

    var game = body.Easy;

    int Height = game.Regions.SelectMany(r => r.Indices).Max(i => i[0]) + 1;
    int Width = game.Regions.SelectMany(r => r.Indices).Max(i => i[1]) + 1;
    
    var board = new Board(new Cell?[Height, Width]);
    List<Constraint> constraints = [];

    foreach (var region in game.Regions)
    {
      Constraint? constraint = Constraint.FromJsonType(region.Type, region.Target);

      foreach(var coords in region.Indices)
      {
        Cell cell = new(coords[1], coords[0]);
        board.AddCell(cell);
        if (constraint is { }) board.AddConstraintToCell(constraint, coords[1], coords[0]);
      }

      if (constraint is { }) constraints.Add(constraint);
    }
    
    var dominos = game.Dominoes.Select(dom => new Domino(dom[0], dom[1])).ToList();

    return (board, constraints, dominos);
  }
}

record JsonResponseDto(string PrintDate, GameDataDto Easy, GameDataDto Medium, GameDataDto Hard);

record GameDataDto(List<List<int>> Dominoes, List<RegionDto> Regions);

record RegionDto(List<List<int>> Indices, string Type, int? Target);
