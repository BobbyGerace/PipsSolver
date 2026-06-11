using PipsSolver.Model;
namespace PipsSolver;

static class NytApi
{
  private static HttpClient client = new()
  {
    BaseAddress = new Uri("https://www.nytimes.com"),
  };

  public static async Task<Board> GetBoardFromDate()
  {
    using HttpResponseMessage response = await client.GetAsync("/svc/pips/v1/2026-06-10.json");

    var json = await response.Content.ReadAsStringAsync();
    
    Console.WriteLine(json);

    throw new NotImplementedException("TODO");
  }
}
