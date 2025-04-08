namespace FlightWeb
{
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }


    }

    public static class AirplaneInfo
    {
        public static int SmallPlaneCount { get; set; } = 22;
        public static int BigPlaneCount { get; set; } = 52;
    }
}
