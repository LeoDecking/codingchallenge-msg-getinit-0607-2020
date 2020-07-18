using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace msg
{
    // Suche nach Orten, Laden der Entfernungs-Matrix und Berechnung von Luftlinien-Entfernungen
    class OpenRouteService
    {
        // https://openrouteservice.org/dev/#/api-docs/geocode/search/get
        // Returns a JSON formatted list of objects corresponding to the search input.
        public static async Task<Standort[]> SearchLocations(string query, string apiTokenKey = "5b3ce3597851110001cf62482a3dd38109654fe6a5bc6b31ac037d30")
        {
            using (var httpClient = new HttpClient { BaseAddress = new Uri("https://api.openrouteservice.org") })
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json, application/geo+json, application/gpx+xml, img/png; charset=utf-8");

                using (var response = await httpClient.GetAsync("/geocode/search?api_key=" + HttpUtility.UrlEncode(apiTokenKey) + "&size=25&text=" + HttpUtility.UrlEncode(query)))
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    JsonElement data = JsonSerializer.Deserialize<JsonElement>(responseData);

                    Standort[] standorte = new Standort[data.GetProperty("features").GetArrayLength()];
                    for (int i = 0; i < standorte.Length; i++)
                    {
                        standorte[i] = new Standort(data.GetProperty("features")[i]);
                    }
                    return standorte;
                }
            }
        }


        // https://openrouteservice.org/dev/#/api-docs/v2/matrix/{profile}/post
        // Returns duration or distance matrix for mutliple source and destination points.
        // By default a symmetric duration matrix is returned where every point in locations is paired with each other. The result is null if a value can’t be determined.
        public static async Task<int[][][]> GetDistances(Standort[] standorte, Profile profile, string apiTokenKey = "5b3ce3597851110001cf62482a3dd38109654fe6a5bc6b31ac037d30")
        {
            using (var httpClient = new HttpClient { BaseAddress = new Uri("https://api.openrouteservice.org") })
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json, application/geo+json, application/gpx+xml, img/png; charset=utf-8");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiTokenKey);
                using (var content = new StringContent("{\"locations\":" + JsonSerializer.Serialize(standorte.Select(s => new double[] { s.lon, s.lat })) + ",\"metrics\":[\"distance\",\"duration\"],\"units\":\"m\"}", Encoding.UTF8, "application/json"))
                {
                    using (var response = await httpClient.PostAsync("/v2/matrix/" + profile.Id, content))
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        JsonElement data = JsonSerializer.Deserialize<JsonElement>(responseData);

                        int[][] distanceMatrix = new int[standorte.Length][];
                        int[][] durationMatrix = new int[standorte.Length][];

                        for (int i = 0; i < distanceMatrix.Length; i++)
                        {
                            distanceMatrix[i] = new int[distanceMatrix.Length];
                            durationMatrix[i] = new int[distanceMatrix.Length];
                            for (int j = 0; j < distanceMatrix.Length; j++)
                            {
                                // Runden auf Meter-Genauigkeit
                                distanceMatrix[i][j] = (int)Math.Round(data.GetProperty("distances")[i][j].GetDouble());
                                // Runden auf Sekunden-Genauigkeit
                                durationMatrix[i][j] = (int)Math.Round(data.GetProperty("durations")[i][j].GetDouble());
                            }
                        }

                        return new[] { distanceMatrix, durationMatrix };

                    }
                }
            }
        }

        // Berechnung der Luftlinien-Entfernung, Geschwindigkeit 100km/h = 27.77777m/s
        public static int[][][] GetLinearDistances(Standort[] standorte)
        {
            int[][] distanceMatrix = new int[standorte.Length][];
            int[][] durationMatrix = new int[standorte.Length][];

            for (int i = 0; i < distanceMatrix.Length; i++)
            {
                distanceMatrix[i] = new int[distanceMatrix.Length];
                durationMatrix[i] = new int[distanceMatrix.Length];
                for (int j = 0; j < distanceMatrix.Length; j++)
                {
                    // Runden auf Meter-Genauigkeit
                    distanceMatrix[i][j] = (int)Math.Round(GetLinearDistance(standorte[i].lon, standorte[i].lat, standorte[j].lon, standorte[j].lat));
                    // Runden auf Sekunden-Genauigkeit
                    durationMatrix[i][j] = (int)Math.Round(distanceMatrix[i][j] * 3.6 / 100);
                }
            }

            return new[] { distanceMatrix, durationMatrix };
        }

        // https://stackoverflow.com/a/51839058/6004362
        // Entfernung in Metern zwischen zwei Koordinaten
        static double GetLinearDistance(double longitude, double latitude, double otherLongitude, double otherLatitude)
        {
            var d1 = latitude * (Math.PI / 180.0);
            var num1 = longitude * (Math.PI / 180.0);
            var d2 = otherLatitude * (Math.PI / 180.0);
            var num2 = otherLongitude * (Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }
    }
}
// Dies ist Teil der Einsendung von Leo Decking, Paderborn, zur Coding Challenge von get in IT und msg im Juni/Juli 2020