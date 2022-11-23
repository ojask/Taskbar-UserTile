using System.Net;
using AppWeather.Model;
using Newtonsoft.Json;

namespace AppWeather.Api
{
    class WeatherApi
    {

        public static WeatherModel.RootObject getOneDayWeather(string latitude, string longitude)
        {
            string str1 = "https://api.open-meteo.com/v1/forecast?latitude=" + latitude + "&longitude=" + longitude + "&current_weather=true&timezone=auto";
            string url = string.Format(str1);
            WebClient client = new WebClient();
            try
            {
                var json = client.DownloadString(url);
                var result = JsonConvert.DeserializeObject<WeatherModel.RootObject>(json);
                WeatherModel.RootObject weatherData = result;
                return weatherData;
            }
            catch (WebException e)
            {
                return null;
            }


        }

    }

    class LocationApi
    {

        public static LocationModel.RootObject getCityCoords(string cityname)
        {
            string str1 = "https://geocoding-api.open-meteo.com/v1/search?name=" + cityname + "&count=1";
            string url = string.Format(str1);
            WebClient client = new WebClient();
            try
            {
                var json = client.DownloadString(url);
                json = json.Replace("[", "");
                json = json.Replace("]", "");
                int q = json.IndexOf("\"elevation\"") - 1;
                int h = json.LastIndexOf("}");
                json = json.Remove(q, h - q).Insert(q, "}");
                var result = JsonConvert.DeserializeObject<LocationModel.RootObject>(json);
                LocationModel.RootObject LocationData = result;
                return LocationData;
            }
            catch (WebException e)
            {
                return null;
            }


        }

    }
}
