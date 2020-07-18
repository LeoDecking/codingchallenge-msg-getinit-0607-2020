using System;

namespace msg
{
    // Hier passiert das Wichtigste: Der kürzeste Rundweg wird mit Hilfe eines dynamischen Programmieransatzes berechnet.
    class Algorithm
    {

        // Die Methode "GetShortestPath" gibt ein "Path"-Object zurück. Es enthält alle wichtigen Informationen über den errechneten Weg.
        public struct Path
        {
            // Array mit Ort-Indizes in der richtigen Reihenfolge
            public int[] Places;
            // Gesamtdauer
            public int Duration;
            // Array mit den Zeit-Abständen zwischen den einzelnen Orten
            public int[] Durations;
            // Gesamtentfernung
            public int Distance;
            // Array mit den Entfernungen zwischen den einzelnen Orten
            public int[] Distances;
            // Zeit, die für die Berechnung benötigt wurde in ms
            public double ComputationTime;
        }

        // Berechnung des Binomial-Koeffizienten: Anzahl der Möglichkeiten von N unterschiedlichen Werten K Werte auszuwählen
        // https://stackoverflow.com/a/12992171/6004362
        static int GetBinCoeff(int N, int K)
        {
            // This function gets the total number of unique combinations based upon N and K.
            // N is the total number of items.
            // K is the size of the group.
            // Total number of unique combinations = N! / ( K! (N - K)! ).
            // This function is less efficient, but is more likely to not overflow when N and K are large.
            // Taken from:  http://blog.plover.com/math/choose.html
            //
            long r = 1;
            int d;
            if (K > N) return 0;
            for (d = 1; d <= K; d++)
            {
                r *= N--;
                r /= d;
            }
            return (int)r;
        }


        // Berechnung des kürzesten Weges, der alle Punkte beinhaltet, weitere Erklärung in README.txt
        // Zur Berechnung wird nur die durationMatrix genutzt, da der Weg möglichst Zeit-effizient sein soll.
        // Wenn in den Kommentaren von "Entfernung" gesprochen wird, ist damit die zeitliche Entfernung gemeint.
        // int[i][j] -> Entfernung von Punkt i zu Punkt j
        public static Path GetShortestPath(int[][] durationMatrix, int[][] distanceMatrix)
        {
            // Um die Laufzeit zu vergleichen, wird die Zeit gemessen
            DateTime startTime = DateTime.Now;

            // Da es um einen Rundweg geht, ist es egal, bei welchem Punkt gestartet wird - um es übersichtlich zu halten, wird Punkt 0 als Startpunkt festgelegt.

            // In diesem Array werden die möglichen Punkt-Kombinationen gespeichert                          876543210
            // int[i][l][cIndex] -> jedes 1-er Bit des Integers zeigt, dass der Punkt besucht wurde ->  z.B. 000100101 -> Der Startpunkt und die Punkte 2 und 5 wurden besucht
            // i -> Anzahl der besuchten Punkte
            // l -> Der Index des zuletzt besuchten Punkts, von dem aus es weiter geht
            // cIndex -> Der Index der Kombination (Sobald der Weg für eine neue Kombination berechnet wurde, wird diese hinten angefügt)
            int[][][] allCombinations = new int[durationMatrix.Length + 1][][];

            // In diesem Array werden alle bisher errechneten kürzesten Entfernungen gespeichert
            // int[l][c]
            // l -> Die Nummer des letzten Punktes
            // c -> Die Punkt-Kombination (mit i 1-er Bits, eins davon ist Nr. l), z.B. 000100101
            // z.B. memo[2][00110101] = 23 -> Der kürzeste Weg, der am Startpunkt startet und über die Punkte 4 und 5 zum Punkt 2 führt, ist 23 lang
            int[][] memo = new int[durationMatrix.Length][];


            // Durchgehen der Kombinationen mit 2 Punkten: Alle starten beim Startpunkt und führen jeweils zu einem anderen
            allCombinations[2] = new int[durationMatrix.Length][];
            for (int n = 1; n < durationMatrix.Length; n++) // n -> Der Punkt, zu dem es vom Startpunkt aus geht
            {
                allCombinations[2][n] = new int[1]; // Es gibt nur eine Kombination mit 2 Punkten, die am Startpunkt startet und an Punkt n endet
                allCombinations[2][n][0] = 1 << n | 1; // Die Bits 0 (Der Startpunkt) und n werden gesetzt
                memo[n] = new int[(int)Math.Pow(2, durationMatrix.Length)]; // Initialisierung von memo: Für jeden Endpunkt n gibt es jeweils 2^(Anzahl Punkte) Kombinations-Möglichkeiten
                memo[n][1 << n | 1] = durationMatrix[0][n]; // Die Länge der Kombination vom Startpunkt zu Punkt n wird gespeichert
            }

            // Durchgehen der Kombinationen mit i besuchten Punkten
            for (int i = 3; i <= durationMatrix.Length; i++) // i -> Anzahl der besuchten Orte
            {
                allCombinations[i] = new int[durationMatrix.Length][];
                int[][] combinations = allCombinations[i];
                int[][] lastCombinations = allCombinations[i - 1];

                int binCoeff = GetBinCoeff(durationMatrix.Length - 2, i - 2); // Bei jedem Punkt, an dem der Weg enden kann, stehen 2 Punkte, die besucht worden sind fest, der Startpunkt und der Endpunkt. Von den restlichen (Anzahl der Punkte) - 2 Punkte wurden i - 2 Punkte besucht. Es gibt ((Anzahl der Punkte) - 2) über (i - 2) Möglichkeiten.

                // Es wird jeweils ein neuer Endpunkt n genommen und an die bestehenden Kombinationen mit (i - 1) besuchten Punkten angehangen
                for (int n = 1; n < durationMatrix.Length; n++) // n -> Der Punkt, der als nächstes besucht werden soll
                {
                    combinations[n] = new int[binCoeff]; // Die Kombinationen mit i besuchten Punkten, die am Startpunkt starten und an Punkt n enden
                    int[] cmb = combinations[n];

                    int cIndex = 0; // Der Index, unter der die nächste neue Kombination gespeichert wird

                    // Alle bisherigen Kombinationen mit (i - 1) besuchten Punkten werden durchgegangen
                    for (int l = 1; l < durationMatrix.Length; l++)// l -> Der letzte Punkt, von dem aus es weiter zu Punkt n geht
                    {
                        if (l == n) continue; // Der Punkt darf natürlich nicht mit dem Punkt übereinstimmen, bei dem es weitergehen soll

                        for (int cOldIndex = 0; cOldIndex < lastCombinations[1].Length; cOldIndex++)// cOldIndex -> Index der Kombination mit (i - 1) besuchten Punkten, die am Startpunkt startet und am Punkt l endet
                        {
                            int last = lastCombinations[l][cOldIndex]; // Die Kombination, an der der neue Punkt n angehangen wird
                            int combination = last | 1 << n; // Bei der neuen Kombination wird zusätzlich zu den zuvor besuchten Punkten auch Punkt n gesetzt

                            if (last == combination) continue; // Wenn das Bit von Punkt n auch vorher schon gesetzt war, der Punkt also schon besucht wurde, wird übersprungen

                            int duration = memo[l][last] + durationMatrix[l][n]; // Die neue Entfernung entspricht der bisherigen Entfernung + der Entfernung von Punk l zu Punkt n

                            if (memo[n][combination] == 0)
                            {
                                memo[n][combination] = duration; // Wenn für die Kombination noch keine Entfernung berechnet wurde, wird sie gespeichert
                                cmb[cIndex++] = combination; // Die Punkt-Kombination wird gespeichert, damit man beim Durchgang mit größerem i anschließend einfach über cOldIndex darauf zugreifen kann
                            }
                            else if (duration < memo[n][combination]) // Wenn für die KOmbination schon eine Entfernung gespeichert ist, die aber größer ist, wird die neue, kleinere, Entfernung gespeichert
                                memo[n][combination] = duration;
                        }
                    }
                }
            }
            // In dem memo-Array befinden sich nun die kürzesten Entfernungen für alle 2^(Anzahl der Punkte) Punkt-Kombinationen. Sie starten jeweils beim Startpunkt und enden an Punkt n
            // Nun muss noch die Reihenfolge der Punkte ermittelt werden, da die ja vorher nicht gespeichert wurde. Zusätzlich muss noch die Entfernung von Punkt n zum Startpunkt addiert werden, da es ja ein Rundweg sein soll

            int[] path = new int[durationMatrix.Length + 1]; // Hier wird die schnellste Reihenfolge der Punkte gespeichert
            int[] durations = new int[durationMatrix.Length]; // Hier die zeitlichen Entfernungen zwischen den einzelnen Punkten des besten Weges
            int[] distances = new int[durationMatrix.Length]; // Hier die "normalen" Entfernungen zwischen den einzelnen Punkten des besten Weges
            int minDistance = 0; // Hier werden die einzelnen "normalen" Entfernungen zwischen den Punkten des besten Weges zusammen addiert
            // Die Wegberechnung fängt hinten an und verfolgt den Weg sozusagen zurück, es wird bei jedem Schritt geguckt, wo eine Entfernung mit einem Punkt weniger so viel niedriger ist, wie die Verbindung in distanceMatrix zum entsprechenden Punkt hin lang ist.
            int currentCombination = (1 << durationMatrix.Length) - 1; // Hier wird gespeichert, wo man sich gerade in der Wegberechnung befindet, zuerst (man geht ja von hinten aus) sind alle Bits gesetzt
            path[0] = 0; // Der Startpunkt ist Punkt 0
            path[durationMatrix.Length] = 0; // Der Endpunkt auch

            // Berechnung der kürzesten Weg-Länge, es werden aus dem memo-Array die Werte genommen, bei denen alle Bits gesetzt sind, und der Weg zum Startpunkt hinzugefügt, da es ein Rundweg sein soll
            int minDuration = int.MaxValue; // Hier wird die Länge des im Moment kürzesten, berechneten Weges gespeichert
            for (int l = 1; l < durationMatrix.Length; l++) // l -> Der Knoten, der zuletzt besucht wurde
            {
                // Die Entfernung entspricht der in memo gespeicherten kürzesten Entfernung, mit der alle Punkte (i = Anzahl der Punkte) besucht werden und die an Punkt l endet, addiert mit der Entfernung von Punkt l zum Startpunkt
                int duration = memo[l][allCombinations[durationMatrix.Length][l][0]] + durationMatrix[l][0];
                if (duration < minDuration) // Dies wird nur ausgeführt, wenn bisher noch keine kürzere Entfernung gefunden wurde
                {
                    // Die Eigenschaften des aktuell gefundenen Weges werden gespeichert
                    minDuration = duration;
                    path[durationMatrix.Length - 1] = l; // Der letzte besuchte Punkt ist Punkt l
                    durations[durationMatrix.Length - 1] = durationMatrix[l][0]; // Der letzte zeitliche Abstand ist der von Punkt l zum Startpunkt
                    distances[durationMatrix.Length - 1] = distanceMatrix[l][0];// Der letzte "normale" Abstand ist der von Punkt l zum Startpunkt
                    minDistance = distanceMatrix[l][0]; // Hier werden die einzelnen "normalen" Abstände addiert, sodass man am Ende auch die gesamt-räumliche Entfernung hat (bisher wurde ja nur die zeitliche Berechnet)
                }
            }



            int d = minDuration - durationMatrix[path[durationMatrix.Length - 1]][0]; // Der Weg, der noch übrig ist; Die Gesamtentfernung abzüglich der Entfernung vom letzten Punkt zum Startpunkt. Hiervon werden immer die berechneten "Wegstücke" abgezogen
            currentCombination ^= 1 << path[durationMatrix.Length - 1]; // Bei der aktuelle Kombination wird das Bit vom letzten Punkt entfernt

            for (int i = durationMatrix.Length - 2; i >= 0; i--) // Von hinten ausgehend wird der Weg zurück verfolgt, immmer mit einem Punkt weniger besucht
            {
                for (int l = 1; l < durationMatrix.Length; l++)  // Der Punkt, der vielleicht, zuletzt besucht wurde
                {
                    if ((currentCombination & 1 << l) == 0) continue; // Wenn der Punkt l, nicht mehr in der aktuellen Kombination vorhanden ist, kann übersprungen werden

                    int duration = memo[l][currentCombination]; // Die gespeicherte Entfernung, die mit der aktuellen Kombination an Punkt l endet
                    
                    if (duration + durationMatrix[l][path[i + 1]] == d) // Wenn gespeicherte Entfernung zu Punkt l addiert mit der Entfernung von Punkt l zu dem nächstem, schon herausgefundenen Punkt des Weges übereinstimmt, mit der Entfernung d, die noch übrig ist, ist Punkt l der nächste Punkt des Weges
                    {
                        // Die Eigenschaften des Pfades werden aktualisiert, ausgehend davon, dass Punkt l der nächste (da ja rückwärts vorgegangen wird eigentlich der nächst letzte) Punkt ist
                        d = duration;
                        path[i] = l;
                        durations[i] = durationMatrix[l][path[i + 1]];
                        distances[i] = distanceMatrix[l][path[i + 1]];
                        minDistance += distances[i];
                        currentCombination ^= 1 << l; // Das Bit von Punkt l wird entfernt
                        break;
                    }
                }
            }
            durations[0] = durationMatrix[0][path[1]]; // Die zeitliche Entfernung vom Startpunkt zum ersten Punkt des Weges wird addiert
            distances[0] = distanceMatrix[0][path[1]]; // Die "normale Entfernung vom Startpunkt zum ersten Punkt des Weges wird addiert
            minDistance += distances[0]; // Auch die räumliche Entfernung wird aktualisiert
            //Nun wird noch überpüft, ob das übrig gebliebene Wegstück, von der Entfernung her passend ist:
            if (durationMatrix[0][path[1]] != d) MyConsole.WriteLine("Es gab leider einen Fehler beim Überprüfen der Route, ist eine Landverbindung zwischen allen Standorten möglich? Versuche es nochmal. " + durationMatrix[0][path[1]] + " != " + d + ")");

            // Der Endzeitpunkt wird gespeichert, um die Berechnungsdauer zu ermitteln
            DateTime endTime = DateTime.Now;
            return new Path { Duration = minDuration, Distance = minDistance, Durations = durations, Distances = distances, Places = path, ComputationTime = (endTime - startTime).TotalMilliseconds };
        }


    }
}
// Dies ist Teil der Einsendung von Leo Decking, Paderborn, zur Coding Challenge von get in IT und msg im Juni/Juli 2020