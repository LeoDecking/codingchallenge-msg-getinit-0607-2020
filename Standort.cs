using System.Globalization;
using System.Text.Json;

namespace msg
{
    // Diese Objekte stellen einzelne Standorte mit Namen, näherer Beschreibung und am wichtigsten den geographischen Koordinaten dar
    class Standort
    {
        public string name;

        public string text;

        public double lat;
        public double lon;

        // Laden eines Standorts aus der csv-Datei mit msg-Standorten
        public Standort(string line)
        {
            string[] parts = line.Split(',');

            this.name = parts[1];

            this.text = parts[2] + " " + parts[3] + ", " + parts[4] + " " + parts[5];

            this.lat = double.Parse(parts[6], NumberStyles.Any, CultureInfo.InvariantCulture);
            this.lon = double.Parse(parts[7], NumberStyles.Any, CultureInfo.InvariantCulture);
        }
        
        // Laden eines Standorts aus der JSON-Antwort von openrouteservice.org
        public Standort(JsonElement feature)
        {
            this.name = feature.GetProperty("properties").GetProperty("label").GetString();

            this.text = feature.GetProperty("properties").GetProperty("layer").GetString();

            this.lat = feature.GetProperty("geometry").GetProperty("coordinates")[1].GetDouble();
            this.lon = feature.GetProperty("geometry").GetProperty("coordinates")[0].GetDouble();
        }
    }
}

// Dies ist Teil der Einsendung von Leo Decking, Paderborn, zur Coding Challenge von get in IT und msg im Juni/Juli 2020