using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;

using static msg.Algorithm;

namespace msg
{
    // Einstiegspunkt und Konsolenein-/ausgabe
    class Program
    {
        // Um beim nächsten Mal wieder die gleichen Standorte benutzen zu können, werden sie am Ende gespeichert
        static Standort[] oldStandorte = null;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;


            // Laden der Standorte - entweder kann die csv-Datei mit den msg-Standorten geladen werden, oder man sucht nach eigenen Orten
            Standort[] standorte = null;
            while (standorte == null)
            {
                Console.Clear();
                MyConsole.WriteLine("Hallo :),");
                MyConsole.Write("dies ist die Einsendung von *red*Leo Decking** zur Coding-Challenge 2020 von *|white**black*get in *green*{*black*IT*green*}** und *|white**dRed*.*dGray*msg**.");
                MyConsole.WriteLine("\nMach diese Konsole am besten in den *red*Vollbildmodus** und stell die *red*Schriftgröße** etwas kleiner.\n");

                MyConsole.WriteLine("Wie willst du die Standorte, für die der beste Rundweg gefunden werden soll, laden?");
                MyConsole.WriteLine("*green*1** - csv-Datei mit *|white**dRed*.*dGray*msg**-Standorten");
                MyConsole.WriteLine("*green*2** - Suche nach eigenen Adressen/Städten");
                if (oldStandorte != null) MyConsole.WriteLine("*green*3** - Suche nach eigenen Adressen/Städten - *yellow* vorherige Standorte übernehmen**");
                MyConsole.Write(oldStandorte == null ? "(*green*1** oder *green*2**): > " : "(*green*1**, *green*2** oder *green*3**): > ");

                string input = Console.ReadLine().Trim();
                if (input == "1")
                {
                    Console.Clear();
                    MyConsole.WriteLine("*green*1** - csv-Datei mit *|white**dRed*.*dGray*msg**-Standorten laden\n");

                    MyConsole.WriteLine("Bitte füge den Inhalt der .csv-Datei *dGray*(in einem Texteditor öffnen und kopieren)** hier ein:");
                    MyConsole.Write("Die Überschriften müssen übereinstimmen: *dMagenta*Nummer,msg Standort,Straße,Hausnummer,PLZ,Ort,Breitengrad,Längengrad\n*cyan*");

                    while (Console.KeyAvailable) Console.ReadKey();

                    List<Standort> standorteList = new List<Standort>();
                    // Zeilen mit falschen Eingaben werden solange übersprungen bis die richtigen Spaltenüberschriften eingefügt werden
                    while (Console.ReadLine() != "Nummer,msg Standort,Straße,Hausnummer,PLZ,Ort,Breitengrad,Längengrad") Console.CursorTop--;

                    string line;
                    while (!string.IsNullOrEmpty(line = Console.ReadLine())) standorteList.Add(new Standort(line));
                    standorte = standorteList.ToArray();
                }
                else if (input == "2" || (oldStandorte != null && input == "3"))
                {
                    List<Standort> standorteList = new List<Standort>();
                    if (input == "3") standorteList.AddRange(oldStandorte);
                    while (standorte == null)
                    {
                        Console.Clear();
                        MyConsole.WriteLine(input == "2" ? "*green*2** - Suche nach eigenen Adressen/Städten" : "*green*3** - Suche nach eigenen Adressen/Städten - *yellow* vorherige Standorte übernehmen**");

                        MyConsole.WriteLine("\n\nDie Standorten werden mit Hilfe der *blue*Openrouteservice API** (*cyan*https://openrouteservice.org/**) gesucht.");
                        MyConsole.WriteLine("Es wird eine Internetverbindung benötigt.");
                        MyConsole.WriteLine("Weitere Informationen zu den Begrifflichkeiten findest du hier: *blue*https://github.com/pelias/documentation/blob/master/search.md#filter-by-data-type**");

                        if (standorteList.Count > 0)
                        {
                            MyConsole.WriteLine("\n**Es wurden bisher folgende *green*" + standorteList.Count + "** Orte geladen:\n");
                            for (int i = 0; i < standorteList.Count; i++)
                            {
                                Standort s = standorteList[i];
                                MyConsole.WriteLine("*green*" + (new string(' ', (standorteList.Count + " ").Length - ((i + 1) + "").Length) + (i + 1)) + "**: " + (i % 2 == 0 ? "*magenta*" : "*dYellow*") + s.name + "*dGray*: " + s.text + " *dRed*" + s.lon + " " + s.lat);
                            };
                        }
                        MyConsole.WriteLine("**");

                        MyConsole.Write("Suchbegriff: (*red*a** zum Abbrechen, *dYellow*e** zum Entfernen, *green*w** für weiter) > ");
                        string consoleInput = Console.ReadLine().Trim().ToLower();
                        Console.CursorTop--;
                        MyConsole.Write(new string(' ', Console.WindowWidth) + "\r");
                        Console.CursorTop--;
                        if (consoleInput == "a") // Abbrechen
                        {
                            bool abbrechen;
                            while (true)
                            {
                                MyConsole.Write("Willst du die Suche wirklich abbrechen / von vorne beginnen? (*red*ja** oder *yellow*nein**): > ");
                                string read = Console.ReadLine().Trim().ToLower();
                                if (read == "ja")
                                {
                                    abbrechen = true;
                                    break;
                                }
                                else if (read == "nein")
                                {
                                    abbrechen = false;
                                    break;
                                }
                                else
                                {
                                    Console.CursorTop--;
                                    MyConsole.Write(new string(' ', Console.WindowWidth) + "\r");
                                    Console.CursorTop--;

                                }
                            }
                            if (abbrechen) break;
                        }
                        else if (consoleInput == "e") // Entfernen eines Standorts
                        {
                            if (standorteList.Count == 0)
                            {
                                MyConsole.Write("Es wurden noch *red*keine** Standorte hinzugefügt, welche entfernt werden können. *dGray*[**OK*dGray*]**");
                                Console.ReadLine();
                            }
                            else
                            {
                                while (true)
                                {
                                    MyConsole.Write("Welchen Standort willst du entfernen? (*red*0** zum Abbrechen) (*red*0**-*green*" + standorteList.Count + "**)> ");
                                    int index;
                                    if (int.TryParse(Console.ReadLine().Trim(), out index) && index >= 0 && index <= standorteList.Count)
                                    {
                                        if (index > 0) standorteList.RemoveAt(index - 1);
                                        break;
                                    }
                                    else
                                    {
                                        Console.CursorTop--;
                                        MyConsole.Write(new string(' ', Console.WindowWidth) + "\r");
                                        Console.CursorTop--;
                                    }
                                }
                            }
                        }
                        else if (consoleInput == "w") // Weiter zur Routen-Berechnung
                        {
                            if (standorteList.Count < 3)
                            {
                                MyConsole.Write("Es werden mindestens *red*3** Standorte benötigt. Füge weitere hinzu. *dGray*[**OK*dGray*]**");
                                Console.ReadLine();
                            }
                            else
                            {
                                while (true)
                                {
                                    MyConsole.Write("Willst du mit *green*" + standorteList.Count + "** Standorten fortfahren? (*green*ja** oder *red*nein**): > ");
                                    string read = Console.ReadLine().Trim().ToLower();
                                    if (read == "ja")
                                    {
                                        standorte = standorteList.ToArray();
                                        break;
                                    }
                                    else if (read == "nein") break;
                                    else
                                    {
                                        Console.CursorTop--;
                                        MyConsole.Write(new string(' ', Console.WindowWidth) + "\r");
                                        Console.CursorTop--;

                                    }
                                }
                            }

                        }
                        else // Sucheingabe
                        {
                            Standort[] newStandorte = await OpenRouteService.SearchLocations(consoleInput);
                            if (newStandorte.Length == 0)
                            {
                                MyConsole.Write("Es wurden leider *red*keine** passenden Orte gefunden, versuche es erneut mit einem anderen Suchbegriff. *dGray*[**OK*dGray*]**");
                                Console.ReadLine();
                            }
                            else if (newStandorte.Length == 1)
                            {
                                MyConsole.WriteLine("Willst du folgenden Ort hinzufügen:");
                                Standort s = newStandorte[0];
                                MyConsole.WriteLine("*blue*" + s.name + "*dGray*: " + s.text + " *dRed*" + s.lon + " " + s.lat);
                                MyConsole.WriteLine("**");
                                while (true)
                                {
                                    MyConsole.Write("(*green*ja** oder *red*nein**): > ");
                                    string read = Console.ReadLine().Trim().ToLower();
                                    if (read == "ja")
                                    {
                                        standorteList.Add(s);
                                        break;
                                    }
                                    else if (read == "nein")
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        Console.CursorTop--;
                                        MyConsole.Write(new string(' ', Console.WindowWidth) + "\r");
                                        Console.CursorTop--;

                                    }
                                }
                            }
                            else
                            {
                                MyConsole.WriteLine("Es wurden folgende *yellow*" + newStandorte.Length + "** Orte gefunden, welchen willst du hinzufügen?\n");
                                MyConsole.WriteLine("*yellow*" + new string(' ', (newStandorte.Length + " ").Length - 1) + "0**: *red*Keinen** dieser Orte hinzufügen");
                                for (int i = 0; i < newStandorte.Length; i++)
                                {
                                    Standort s = newStandorte[i];
                                    MyConsole.WriteLine("*yellow*" + new string(' ', (newStandorte.Length + " ").Length - ((i + 1) + "").Length) + (i + 1) + "**: " + (i % 2 == 0 ? "*blue*" : "*dGreen*") + s.name + "*dGray*: " + s.text + " *dRed*" + s.lon + " " + s.lat);
                                };
                                MyConsole.WriteLine("**");
                                while (true)
                                {
                                    MyConsole.Write("Eingabe: > ");
                                    int index;
                                    if (int.TryParse(Console.ReadLine().Trim(), out index) && index >= 0 && index <= newStandorte.Length)
                                    {
                                        if (index > 0) standorteList.Add(newStandorte[index - 1]);
                                        break;
                                    }
                                    else
                                    {
                                        Console.CursorTop--;
                                        MyConsole.Write(new string(' ', Console.WindowWidth) + "\r");
                                        Console.CursorTop--;
                                    }
                                }
                            }
                        }
                    }

                }
            }


            // Auswahl des Fortbewegungsprofils
            MyConsole.WriteLine("**Für welche Fortbewegungsprofil soll der Weg berechnet werden?:\n");
            for (int i = 0; i < Profile.Profiles.Length; i++)
            {
                MyConsole.WriteLine("*yellow*" + (i + 1) + ": " + Profile.Profiles[i].Color + Profile.Profiles[i].Name);
            };
            MyConsole.WriteLine();
            Profile profile = new Profile();
            while (true)
            {
                MyConsole.Write("**(*yellow*1**-*yellow*" + (Profile.Profiles.Length) + "**): > ");
                int input;
                if (int.TryParse(Console.ReadLine(), out input) && input >= 1 && input <= Profile.Profiles.Length)
                {
                    profile = Profile.Profiles[input - 1];
                    break;
                }
                Console.CursorTop--;
                MyConsole.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
                Console.CursorTop--;
            }

            Console.Clear();

            // Anzeige der geladenen Standorte
            MyConsole.WriteLine("**Es wurden folgende *green*" + standorte.Length + "** Standorte geladen:\n");
            for (int i = 0; i < standorte.Length; i++)
            {
                Standort s = standorte[i];
                MyConsole.WriteLine((i % 2 == 0 ? "*magenta*" : "*dYellow*") + s.name + "*dGray*: " + s.text + " *dRed*" + s.lon + " " + s.lat);
            };


            if (profile.Id == "linear")
                MyConsole.WriteLine("\n**Die Entfernungen zwischen den einzelnen Standorten für *dGray*'" + profile.Color + profile.Name + "*dGray*'** werden jetzt berechnet. [OK]");
            else
            {
                MyConsole.WriteLine("\n**Die Entfernungen zwischen den einzelnen Standorten für *dGray*'" + profile.Color + profile.Name + "*dGray*'** werden jetzt mit Hilfe der *blue*Openrouteservice API** geladen.");
                MyConsole.WriteLine("Da die Wege möglichst effizient sein sollen, wird nicht der *red*kürzeste**, sondern der *green*schnellste** Weg genommen.");
                MyConsole.Write("Hierfür wird eine Internetverbindung benötigt. [OK]");
            }
            while (Console.KeyAvailable) Console.ReadKey();
            Console.ReadLine();
            // Die Entfernungen zwischen den einzelnen Standorten werden als Matrix mit der von https://openrouteservice.org/ bereitgestellten API geladen
            // Es wird für den jeweils schnellsten Weg die Entfernung und die Zeit angegeben
            int[][][] matrixes = profile.Id == "linear" ? OpenRouteService.GetLinearDistances(standorte) : await OpenRouteService.GetDistances(standorte, profile);
            int[][] distanceMatrix = matrixes[0];
            int[][] durationMatrix = matrixes[1];


            // Anzeige der Entfernungen
            Console.Clear();
            MyConsole.WriteLine("Die Entfernungen für *dGray*'" + profile.Color + profile.Name + "*dGray*'** wurden geladen.");
            MyConsole.WriteLine("Es wird jeweils von den angebgebenen Koordinaten, bzw. der nächsten Straße ausgegangen.");
            MyConsole.WriteLine("Alle Werte sind in Stunden:Minuten angegeben.");
            MyConsole.WriteLine("Die Zeilen geben den jeweiligen Start- und die Spalten den Zielstandort an:");
            MyConsole.WriteLine("\tVon *dRed*" + standorte[1].name + "** nach *dRed*" + standorte[2].name + "** dauert es zum Beispiel *red*" + DurationToString(durationMatrix[1][2]) + "**.\n");
            
            MyConsole.WriteLine("  ↓von↓  nach→ " + string.Join("*dGray*|**", standorte.Select(s => s.name.Length > 9 ? s.name.Substring(0, 9) : (new string(' ', 9 - s.name.Length) + s.name))));

            for (int i = 0; i < durationMatrix.Length; i++)
            {
                Standort s = standorte[i];
                string sName = s.name.Length > 14 ? s.name.Substring(0, 14) : (new string(' ', 14 - s.name.Length) + s.name);
                MyConsole.WriteLine(sName + ": " + string.Join(" *dGray*|** ", durationMatrix[i].Select(x => new string(' ', 6 - DurationToString(x).Length) + DurationToString(x) + " ")));
            }
            MyConsole.WriteLine();
            MyConsole.WriteLine("\nEs wird nun die optimale Reihenfolge zum Anfahren der Standorte berechnet.");

            //Berechnung des effizientesten Weges
            Path path = Algorithm.GetShortestPath(durationMatrix, distanceMatrix);
            // Arbeitsspeicher bereinigen
            GC.Collect();
            Console.CursorTop--;
            MyConsole.Write(new string(' ', Console.WindowWidth) + "\r");

            // Anzeigen des schnellsten Weges
            MyConsole.WriteLine("Der schnellste Weg wurde in *red*" + Math.Round(path.ComputationTime, 2) + "ms** gefunden:\n");

            for (int i = 0; i < standorte.Length; i++)
            {
                MyConsole.Write("*green*" + standorte[path.Places[i]].name + "** → *blue*" + DurationToString(path.Durations[i]) + " (" + Math.Round((double)path.Distances[i] / 1000, 1) + "km) **→ ");
            }
            MyConsole.WriteLine("*green*" + standorte[0].name + "**");

            MyConsole.WriteLine("\nFortbewegungsprofil: *blue*" + profile.Name);
            MyConsole.WriteLine("**Gesamtdauer: *blue*" + DurationToString(path.Duration));
            MyConsole.WriteLine("**Gesamtdistanz: *blue*" + Math.Round((double)path.Distance / 1000, 2) + "km**");

            MyConsole.WriteLine("\nÖffne diesen Link in deinem Browser, um dir den Streckenverlauf genauer anzusehen:");
            MyConsole.WriteLine("*blue*https://maps.openrouteservice.org/*cyan*directions?a=" + string.Join(',', path.Places.Select(i => standorte[i].lat.ToString(CultureInfo.InvariantCulture) + "," + standorte[i].lon.ToString(CultureInfo.InvariantCulture))) + "&b=" + profile.Code + "&c=0&k1=de-DE&k2=km");
            if(profile.Id=="linear") MyConsole.WriteLine("*red*Achtung**: Da die openrouteserve-Karte leider keine Luftlinien-Routen unterstützt, wird der Weg stattdessen für *cyan*Fußgänger** angezeigt, die dort angegebenen Routen-Informationen *red*weichen** also etwas *red*ab**.");


            MyConsole.Write("**\nDieses Programm wurde im Juli 2020 von *red*Leo Decking** als Beitrag zur Coding-Challenge 2020 von *|white**black*get in *green*{*black*IT*green*}** und *|white**dRed*.*dGray*msg** programmiert.\n");
            
            // Programm neustarten
            MyConsole.WriteLine("Drücke *dGray*[*green*Enter*dGray*]**, um von vorne zu beginnen.");
            MyConsole.WriteLine("Mit *dGray*[*red*Escape*dGray*]** kannst du das Programm beenden.");

            while (Console.KeyAvailable) Console.ReadKey();
            while (true)
            {
                ConsoleKeyInfo k = Console.ReadKey();
                if (k.Key == ConsoleKey.Enter)
                {
                    oldStandorte = standorte;
                    await Main(args);
                    return;
                }
                else if (k.Key == ConsoleKey.Escape) return;
                else MyConsole.Write("\r \r");
            }
        }


        // Umwandlung von einer Zeitdauer in ms in eine Zeitangabe nach dem Format: hh:mm
        static string DurationToString(int duration)
        {
            if (duration == 0) return "-  ";
            string minutes = Math.Floor((double)duration / 60) % 60 + "";
            return ((int)duration / 60 / 60) + ":" + (minutes.Length == 1 ? "0" : "") + minutes;
        }
    }
}
// Dies ist Teil der Einsendung von Leo Decking, Paderborn, zur Coding Challenge von get in IT und msg im Juni/Juli 2020