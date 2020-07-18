using System;
using System.Collections.Generic;

namespace msg
{
    // Diese Klasse ermöglicht es in der Konsole auf leichte Weisen Farben zu benutzen
    // Mit Sternchen (*) kann man die Vorder- mit zusätzlichen senkrechten Strichen (|) die Hintergrundfarbe festlegen
    // Ein Doppelstern (**) hebt die Farbeinstellungen wieder auf
    // "Hallo *red*das hier ist rot**, hier ist es wieder normal"
    // "Und *|blue*hier ist der Hintergrund blau** hier nicht mehr"
    class MyConsole
    {
        public static void Write(string text = "")
        {
            string[] parts = text.Split("*");
            for (int i = 0; i < parts.Length; i++)
            {
                if (i % 2 == 0) Console.Write(parts[i]);
                else if (parts[i].Length == 0) Console.ResetColor();
                else if (Colors.ContainsKey(parts[i])) Console.ForegroundColor = Colors[parts[i]];
                else if (Colors.ContainsKey(parts[i].TrimStart('|'))) Console.BackgroundColor = Colors[parts[i].TrimStart('|')];
                else Console.Write("FALSCHE FARBE");
            }
        }
        public static void WriteLine(string text = "")
        {
            Write(text + "\n");
        }

        static Dictionary<string, ConsoleColor> Colors = new Dictionary<string, ConsoleColor> {
            { "black", ConsoleColor.Black },
            { "blue", ConsoleColor.Blue },
            { "cyan", ConsoleColor.Cyan },
            { "dBlue", ConsoleColor.DarkBlue },
            { "dCyan", ConsoleColor.DarkCyan },
            { "dGray", ConsoleColor.DarkGray },
            { "dGreen", ConsoleColor.DarkGreen },
            { "dMagenta", ConsoleColor.DarkMagenta },
            { "dRed", ConsoleColor.DarkRed },
            { "dYellow", ConsoleColor.DarkYellow },
            { "gray", ConsoleColor.Gray },
            { "green", ConsoleColor.Green },
            { "magenta", ConsoleColor.Magenta },
            { "red", ConsoleColor.Red },
            { "white", ConsoleColor.White },
            { "yellow", ConsoleColor.Yellow }
            };
    }
}
// Dies ist Teil der Einsendung von Leo Decking, Paderborn, zur Coding Challenge von get in IT und msg im Juni/Juli 2020