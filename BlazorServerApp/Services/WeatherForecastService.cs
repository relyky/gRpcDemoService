using BlazorServerApp.Models;

namespace BlazorServerApp.Services;

public class WeatherForecastService
{
  static readonly string[] Summaries = new[]
  {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
  };

  public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
  {
    return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
      Date = startDate.AddDays(index),
      TemperatureC = Random.Shared.Next(-20, 55),
      Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    }).ToArray());
  }

  public async IAsyncEnumerable<WeatherForecast> GetForecastsStreamAsync() 
  {
    DateTime startDate = DateTime.Today.AddDays(-7);

    for(int index = 0; index < 21; index++)
    {
      // 模擬處理時間
      await Task.Delay(Random.Shared.Next(1, 2000));

      yield return new WeatherForecast
      {
        Date = startDate.AddDays(index),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
      };
    }
  }

}
