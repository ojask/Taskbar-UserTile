namespace AppWeather.Model
{
    class WeatherModel
    {
        public class current_weather
        {
            public float temperature { get; set; }
            public int weathercode { get; set; }
            public float windspeed { get; set; }
            public string time { get; set; }
        }

        public class RootObject
        {
            public current_weather current_weather { get; set; }
        }
    }

    class LocationModel
    {
        public class results
        {
            public float latitude { get; set; }
            public float longitude { get; set; }
        }

        public class RootObject
        {
            public results results { get; set; }
        }
    }
}
