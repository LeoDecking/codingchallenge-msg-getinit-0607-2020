namespace msg
{
    // Objekte dieser Klasse repräsentieren die unterschiedlichen Fortbewegungsprofile, die es bei openrouteservice.org gibt
    class Profile
    {
        public string Name;
        public string Color;
        public string Id;
        public string Code;

        public static Profile[] Profiles = new Profile[] {
            // Die Luftlinien-Entfernung wird selber berechnet, da die openrouteservice-Karte keine Luftlinien-Routen anzeigen kann, wird stattdessen eine "Spazieren"-Route angezeigt
            new Profile {Name="100km/h Luftlinie",Color="*red*", Id="linear",Code="2" },

            new Profile {Name="Auto",Color="*cyan*", Id="driving-car",Code="0" },
            new Profile {Name="Schwerkraftwagen",Color="*cyan*", Id="driving-hgv",Code="4a" },

            new Profile {Name="Fahrrad",Color="*green*", Id="cycling-regular",Code="1a" },
            new Profile {Name="Rennrad",Color="*green*", Id="cycling-road",Code="1c" },
            new Profile {Name="Mountainbike",Color="*green*", Id="cycling-mountain",Code="1b" },
            new Profile {Name="E-Bike",Color="*green*", Id="cycling-electric",Code="1f" },

            new Profile {Name="Spazieren",Color="*blue*", Id="foot-walking",Code="2" },
            new Profile {Name="Wandern",Color="*blue*", Id="foot-hiking",Code="2b" }
        };
    }
}
// Dies ist Teil der Einsendung von Leo Decking, Paderborn, zur Coding Challenge von get in IT und msg im Juni/Juli 2020