Dies ist meine Einsendung zur Coding-Challenge Juni/Juli 2020 von get-in-it und msg.
Ich bin Leo Decking, 19 Jahre alt, und wohne in Paderborn. Zur Zeit mache ich noch ein FSJ im Diözesanbüro der Deutschen Pfadfinderschaft St. Georg (DPSG) Paderborn. Im Oktober/November fange ich mit meinem Informatik-Studium an.

---------------------------Algorithmus---------------------------
Als ich über Instagram von dem Wettbewerb erfahren habe, fand ich die Aufgabe direkt sehr interessant und hab angefangen ein wenig auszuprobieren:
Als Programmiersprache habe ich C# gewählt, da ich dort - zusammen mit JavaScript/TypeScript - die meiste Erfahrung habe, und es von der Performance besser ist als JavaScript.
Schnell habe ich gemerkt, dass das ausprobieren aller möglichen Wege zwischen zufälligen zwar leicht zu programmieren ist, aber auch nach vielen kleinen Optierungen immer noch ewig (zuletzt ~30min) in der Berechnung dauert.

Ich wollte, das mein Programm auf jeden Fall immer die beste Lösung ausgibt und nicht nur eine Näherung.
Ich habe mich also im Internet umgeschaut und bin auf den Ansatz der dynamischen Programmierung gestoßen (Held-Karp-Algorithmus): Die Zwischenergebnisse werden gespeichert, sodass sie nicht ímmer neu berechnet werden müssen.
Der Grundansatz ist, dass auf dem bestmöglichen Weg ja auch die Teilstrecken bestmögliche Wege sein müssen:
Wenn es die 7 Punkte A, B, C, D, E und F gibt und der beste Weg von A nach D über B und C lautet "A-C-B-D", dann kann in dem besten Weg mit allen Punkten nicht "A-B-C-D" auftauchen.
Es werden zu Beginn jeweils nur die Länge der Wege und die "abgelaufenen" Punkte gespeichert, nicht die Reihenfolge, da der Speicherverbrauch so schon schnell wächst. Die Reihenfolge wird am Ende berechnet.

Begonnen wird also damit, dass alle Wege vom 1. (da es ein Rundweg ist, kann man einen beliebigen der Punkte als Start festlegen) zu jedem der anderen Punkte berechnet und die Entfernung gespeichert wird.
Anschließend werden alle möglichen Wege zwischen 3 Punkten (der erste davon der Startpunkt) berechnet. Dafür wird die Entfernung zwischen den ersten beiden Punkten aus den gespeicherten genommen und die Entfernung vom letzten der ersten beiden zum 3. angefügt.
Bei den Punkten A-B-C wird natürlich sowohl A-B-C als auch A-C-B berechnet.
Das geht so weiter mit allen Wegen zwischen 4, dann alle Wege zwischen 5 Punkten, usw.
Entscheidend für einen Weg ist für die weitere Berechnung nur, welche Punkte besucht wurden, die Entfernung und der letzte Punkt, von dem es später weiter geht. Die Reihenfolge der Punkte zwischen Start- und Endpunkt ist egal.
Sobald ein Weg mit gleichem Endpunkt und gleichen besuchten Punkten dazwischen gefunden wird, dessen Entfernung kleiner ist, wird die alte Entfernung mit der besseren überschrieben.
Es wird also für jede Kombination an besuchten Punkten mit jedem Punkt als Endpunkt die kürzeste Entfernung gespeichert.

----------------------Laufzeit und Speicher----------------------
Die hierfür benötigte Speichermenge wächst also ungefähr Exponentiell mit der Anzahl der Punkte N: N (Anzahl der Möglichen Endpunkte pro Kombination) * 2^N (Bei einem Punkt mehr gibt es immer doppelt so viele Möglichkeiten - er wurde besucht oder er wurde nicht besucht).
Die benötigte Zeit wächst auch in etwa exponentiell zur Basis 2.
Damit ist die Berechnung deutlich schneller als die Brute-Force-Methode, bei der die Zeit faktoriell wächst.
Mein Laptop (i7-8565U, 16GB DDR4 RAM) benötigte folgende Zeit und Arbeitsspeicher:
21 Punkte -   2s - 0.2GB
22 Punkte -   5s - 0.4GB
23 Punkte -  12s - 0.7GB
24 Punkte -  32s - 1.5GB
25 Punkte -  82s - 3GB
26 Punkte - 206s - 7GB
27 Punkte - 485s - 12GB

Für 28 oder mehr Punkte war der Arbeitsspeicher meines Laptops leider zu klein, da hierfür über 20GB benötigt würden.


----------------------openrouteservice.org-----------------------
Um die Abstände zwischen den einzelnen Standorten zu "berechnen", habe ich die kostenlose openrouteservice.org API der Universität Heidelberg benutzt: https://openrouteservice.org/dev/#/api-docs/v2/matrix/{profile}/post
Es wird jeweils die Entfernung für den schnellsten, nicht unbedingt kürzezsten Weg genommen, da man ja möglichst effizient reisen möchte.
Es gibt dort die Möglichkeit zwischen mehreren Fortbewegungsprofilen zu wählen - z.B. Fahrrad oder Auto. Das habe ich übernommen. Zusätzlich habe ich aber auch die Berechnung der Luftlinie eingebaut.
Da man dort auch nach Orten suchen kann, habe ich dies in das Programm eingebaut. Man kann so auch Routen zwischen beliebigen (auf dem Landweg erreichbaren :D) berechnen.
Darüber hinaus bietet openrouteservice auch eine Karte, bei der man über GET-Parameter auch Routenpunkte vorgeben kann: https://maps.openrouteservice.org/
Dadurch war es ganz leicht, dass der berechnete Weg einem nun auch angezeigt werden kann.


---------------------Benutzung des Programms---------------------
Um das Programm auszuführen, benötigt man das dotnet core SDK: https://dotnet.microsoft.com/download
Wenn man sich mit einer Konsole in diesem Ornder befindet, startet man das Programm mit "dotnet run -c release"
Anschließend wird man nach den Standorten gefragt - entweder man fügt den Inhalt der msg-Standorte-csv-Datei ein - oder man sucht nach eigenen Orten.
Nachdem man sein Fortbewegungsprofil auswählt, werden die Entfernungen berechnet/geladen und der schnellste Weg, den man danach auch im Browser betrachten kann, wird berechnet.


----------------------------Der Code-----------------------------
Program.cs:             Hier wird der ganze Ablauf und die Konsolenaus- und -eingaben gestartet und gesteuert
MyConsole.cs:           Ermöglicht, einfach Farben in der Konsole zu nutzen
Standort.cs:            Diese Objekte stellen einzelne Standorte mit Namen, näherer Beschreibung und am wichtigsten den geographischen Koordinaten dar
OpenRouteService.cs:    Suche nach Orten, Laden der Entfernungs-Matrix und Berechnung von Luftlinien-Entfernungen
Profile.cs:             Objekte dieser Klasse repräsentieren die unterschiedlichen Fortbewegungsprofile, die es bei openrouteservice.org gibt
Algorithm.cs:           Hier passiert das Wichtigste: Der kürzeste Rundweg wird mit Hilfe eines dynamischen Programmieransatzes berechnet.


-----------Bestmögliche Wege nach Fortbewegungsprofil------------

Fortbewegungsprofil: 100km/h Luftlinie (berechnet, nicht von openrouteservice)
Gesamtdauer: 23:21
Gesamtdistanz: 2335,43km

Ismaning/München (Hauptsitz) → 1:21 (136km) → Passau → 2:32 (254,2km) → Chemnitz → 1:28 (148,2km) → Görlitz → 1:58 (196,9km) → Berlin → 1:54 (190,9km) → Braunschweig → 0:31 (51,8km) → Hannover → 1:21 (136,5km) → Hamburg → 1:21 (135,5km) → Schortens/Wilhelmshaven → 1:12 (120,6km) → Lingen (Ems) → 0:38 (64,3km) → Münster → 0:42 (71,2km) → Essen → 0:14 (24,6km) → Düsseldorf → 0:26 (44km) → Köln/Hürth → 1:26 (144km) → Frankfurt → 0:56 (93,8km) → Walldorf → 0:17 (29,4km) → Bretten → 1:02 (104,5km) → St. Georgen → 0:53 (88,4km) → Stuttgart → 1:34 (158,1km) → Nürnberg → 0:46 (77km) → Ingolstadt → 0:39 (65,3km) → Ismaning/München (Hauptsitz)

https://maps.openrouteservice.org/directions?a=48.229035,11.686153,48.571989,13.453256,50.829383,12.914737,51.145511,14.970028,52.580911,13.293884,52.278748,10.524797,52.337987,9.769706,53.557577,9.986065,53.537779,7.936809,52.519154,7.322185,51.969304,7.61428,51.450577,7.008871,51.274774,6.794912,50.886726,6.913119,50.136479,8.570963,49.295011,8.649036,49.032767,8.698372,48.126258,8.325873,48.694648,9.161239,49.429596,11.017404,48.784417,11.399106,48.229035,11.686153&b=2&c=0&k1=de-DE&k2=km
Achtung: Da die openrouteservice-Karte leider keine Luftlinien-Routen unterstützt, wird der Weg stattdessen für Fußgänger angezeigt, die dort angegebenen Routen-Informationen weichen also etwas ab.


Fortbewegungsprofil: Auto
Gesamtdauer: 32:38
Gesamtdistanz: 3192,84km

Ismaning/München (Hauptsitz) → 2:03 (222,5km) → Stuttgart → 1:21 (113,5km) → St. Georgen → 1:50 (155,4km) → Bretten → 0:47 (41,6km) → Walldorf → 0:51 (100,4km) → Frankfurt → 1:48 (192,8km) → Köln/Hürth → 0:51 (68,2km) → Düsseldorf → 0:22 (28km) → Essen → 1:02 (102,9km) → Münster → 1:06 (94,7km) → Lingen (Ems) → 1:52 (160,4km) → Schortens/Wilhelmshaven → 2:12 (217,9km) → Hamburg → 1:36 (162km) → Hannover → 0:44 (63km) → Braunschweig → 2:19 (233,6km) → Berlin → 3:03 (320,9km) → Görlitz → 1:44 (177,6km) → Chemnitz → 2:15 (247,6km) → Nürnberg → 0:57 (95,1km) → Ingolstadt → 2:03 (215,4km) → Passau → 1:42 (179,4km) → Ismaning/München (Hauptsitz)
https://maps.openrouteservice.org/directions?a=48.229035,11.686153,48.694648,9.161239,48.126258,8.325873,49.032767,8.698372,49.295011,8.649036,50.136479,8.570963,50.886726,6.913119,51.274774,6.794912,51.450577,7.008871,51.969304,7.61428,52.519154,7.322185,53.537779,7.936809,53.557577,9.986065,52.337987,9.769706,52.278748,10.524797,52.580911,13.293884,51.145511,14.970028,50.829383,12.914737,49.429596,11.017404,48.784417,11.399106,48.571989,13.453256,48.229035,11.686153&b=0&c=0&k1=de-DE&k2=km



Fortbewegungsprofil: Schwerkraftwagen
Gesamtdauer: 55:47
Gesamtdistanz: 3025,27km

Ismaning/München (Hauptsitz) → 3:03 (173,2km) → Passau → 3:33 (188,4km) → Ingolstadt → 1:40 (93,8km) → Nürnberg → 4:28 (248,7km) → Chemnitz → 3:23 (178,1km) → Görlitz → 4:29 (238km) → Berlin → 3:59 (229,1km) → Braunschweig → 1:08 (66,6km) → Hannover → 2:56 (161,8km) → Hamburg → 3:55 (216,1km) → Schortens/Wilhelmshaven → 2:56 (151,6km) → Lingen (Ems) → 1:32 (76,8km) → Münster → 1:42 (89,6km) → Essen → 0:34 (25,9km) → Düsseldorf → 1:14 (67,8km) → Köln/Hürth → 3:21 (186,9km) → Frankfurt → 1:54 (103,4km) → Walldorf → 1:05 (42,1km) → Bretten → 2:58 (155km) → St. Georgen → 2:06 (113,1km) → Stuttgart → 3:44 (219,3km) → Ismaning/München (Hauptsitz)
https://maps.openrouteservice.org/directions?a=48.229035,11.686153,48.571989,13.453256,48.784417,11.399106,49.429596,11.017404,50.829383,12.914737,51.145511,14.970028,52.580911,13.293884,52.278748,10.524797,52.337987,9.769706,53.557577,9.986065,53.537779,7.936809,52.519154,7.322185,51.969304,7.61428,51.450577,7.008871,51.274774,6.794912,50.886726,6.913119,50.136479,8.570963,49.295011,8.649036,49.032767,8.698372,48.126258,8.325873,48.694648,9.161239,48.229035,11.686153&b=4a&c=0&k1=de-DE&k2=km



Fortbewegungsprofil: Fahrrad
Gesamtdauer: 165:30
Gesamtdistanz: 2887,88km

Ismaning/München (Hauptsitz) → 4:35 (80,7km) → Ingolstadt → 5:21 (91,9km) → Nürnberg → 11:16 (198,4km) → Stuttgart → 6:20 (111,6km) → St. Georgen → 7:42 (133,7km) → Bretten → 2:11 (38,8km) → Walldorf → 6:18 (111,7km) → Frankfurt → 10:45 (185,8km) → Köln/Hürth → 2:57 (52,3km) → Düsseldorf → 1:43 (30,8km) → Essen → 4:50 (85,5km) → Münster → 4:04 (72,1km) → Lingen (Ems) → 7:58 (140,1km) → Schortens/Wilhelmshaven → 10:26 (172,4km) → Hamburg → 9:15 (162,7km) → Hannover → 3:32 (61,3km) → Braunschweig → 13:14 (228,3km) → Berlin → 13:52 (242,8km) → Görlitz → 10:19 (181,7km) → Chemnitz → 19:14 (339,7km) → Passau → 9:28 (165,5km) → Ismaning/München (Hauptsitz)
https://maps.openrouteservice.org/directions?a=48.229035,11.686153,48.784417,11.399106,49.429596,11.017404,48.694648,9.161239,48.126258,8.325873,49.032767,8.698372,49.295011,8.649036,50.136479,8.570963,50.886726,6.913119,51.274774,6.794912,51.450577,7.008871,51.969304,7.61428,52.519154,7.322185,53.537779,7.936809,53.557577,9.986065,52.337987,9.769706,52.278748,10.524797,52.580911,13.293884,51.145511,14.970028,50.829383,12.914737,48.571989,13.453256,48.229035,11.686153&b=1a&c=0&k1=de-DE&k2=km



Fortbewegungsprofil: Rennrad
Gesamtdauer: 123:15
Gesamtdistanz: 2936,79km

Ismaning/München (Hauptsitz) → 3:47 (83,9km) → Ingolstadt → 4:01 (96km) → Nürnberg → 7:57 (193,2km) → Stuttgart → 4:32 (114,3km) → St. Georgen → 5:42 (138,4km) → Bretten → 1:42 (38,7km) → Walldorf → 4:56 (117,8km) → Frankfurt → 7:32 (179,9km) → Köln/Hürth → 2:23 (53,2km) → Düsseldorf → 1:17 (30,9km) → Essen → 3:29 (86,2km) → Münster → 3:10 (77,1km) → Lingen (Ems) → 6:10 (151km) → Schortens/Wilhelmshaven → 8:12 (183,1km) → Hamburg → 6:47 (163,8km) → Hannover → 2:46 (67,1km) → Braunschweig → 10:05 (238km) → Berlin → 10:33 (246,7km) → Görlitz → 7:31 (180,3km) → Chemnitz → 13:52 (337,4km) → Passau → 6:41 (159,7km) → Ismaning/München (Hauptsitz)
https://maps.openrouteservice.org/directions?a=48.229035,11.686153,48.784417,11.399106,49.429596,11.017404,48.694648,9.161239,48.126258,8.325873,49.032767,8.698372,49.295011,8.649036,50.136479,8.570963,50.886726,6.913119,51.274774,6.794912,51.450577,7.008871,51.969304,7.61428,52.519154,7.322185,53.537779,7.936809,53.557577,9.986065,52.337987,9.769706,52.278748,10.524797,52.580911,13.293884,51.145511,14.970028,50.829383,12.914737,48.571989,13.453256,48.229035,11.686153&b=1c&c=0&k1=de-DE&k2=km



Fortbewegungsprofil: Mountainbike
Gesamtdauer: 160:21
Gesamtdistanz: 2768,96km

Ismaning/München (Hauptsitz) → 4:24 (76km) → Ingolstadt → 5:20 (91,7km) → Nürnberg → 10:42 (185,8km) → Stuttgart → 6:01 (105,8km) → St. Georgen → 7:27 (128,3km) → Bretten → 2:07 (36,7km) → Walldorf → 6:13 (108,4km) → Frankfurt → 9:55 (170,6km) → Köln/Hürth → 2:54 (51,2km) → Düsseldorf → 1:43 (30,3km) → Essen → 4:48 (84,3km) → Münster → 4:04 (70,9km) → Lingen (Ems) → 7:54 (139,4km) → Schortens/Wilhelmshaven → 10:13 (174,2km) → Hamburg → 8:56 (156,9km) → Hannover → 3:28 (57,2km) → Braunschweig → 13:12 (225,3km) → Berlin → 13:25 (230,6km) → Görlitz → 10:02 (172,9km) → Chemnitz → 18:12 (312,4km) → Passau → 9:12 (160,1km) → Ismaning/München (Hauptsitz)
https://maps.openrouteservice.org/directions?a=48.229035,11.686153,48.784417,11.399106,49.429596,11.017404,48.694648,9.161239,48.126258,8.325873,49.032767,8.698372,49.295011,8.649036,50.136479,8.570963,50.886726,6.913119,51.274774,6.794912,51.450577,7.008871,51.969304,7.61428,52.519154,7.322185,53.537779,7.936809,53.557577,9.986065,52.337987,9.769706,52.278748,10.524797,52.580911,13.293884,51.145511,14.970028,50.829383,12.914737,48.571989,13.453256,48.229035,11.686153&b=1b&c=0&k1=de-DE&k2=km



Fortbewegungsprofil: E-Bike
Gesamtdauer: 136:19
Gesamtdistanz: 2897,58km

Ismaning/München (Hauptsitz) → 3:45 (80,5km) → Ingolstadt → 4:26 (91,9km) → Nürnberg → 9:16 (198,4km) → Stuttgart → 5:07 (111,1km) → St. Georgen → 6:14 (133,3km) → Bretten → 1:47 (39km) → Walldorf → 5:13 (111,6km) → Frankfurt → 8:46 (185,7km) → Köln/Hürth → 2:24 (52,3km) → Düsseldorf → 1:24 (30,8km) → Essen → 3:58 (85,6km) → Münster → 3:22 (72,3km) → Lingen (Ems) → 6:31 (141,1km) → Schortens/Wilhelmshaven → 8:48 (174,3km) → Hamburg → 7:38 (162,3km) → Hannover → 2:54 (62km) → Braunschweig → 10:55 (232,9km) → Berlin → 11:28 (243,7km) → Görlitz → 8:30 (181,6km) → Chemnitz → 15:58 (341,5km) → Passau → 7:44 (165,7km) → Ismaning/München (Hauptsitz)
https://maps.openrouteservice.org/directions?a=48.229035,11.686153,48.784417,11.399106,49.429596,11.017404,48.694648,9.161239,48.126258,8.325873,49.032767,8.698372,49.295011,8.649036,50.136479,8.570963,50.886726,6.913119,51.274774,6.794912,51.450577,7.008871,51.969304,7.61428,52.519154,7.322185,53.537779,7.936809,53.557577,9.986065,52.337987,9.769706,52.278748,10.524797,52.580911,13.293884,51.145511,14.970028,50.829383,12.914737,48.571989,13.453256,48.229035,11.686153&b=1f&c=0&k1=de-DE&k2=km



Fortbewegungsprofil: Spazieren
Gesamtdauer: 543:48
Gesamtdistanz: 2798,3km

Ismaning/München (Hauptsitz) → 32:58 (164,9km) → Passau → 62:55 (314,6km) → Chemnitz → 35:11 (176km) → Görlitz → 44:58 (224,8km) → Berlin → 44:22 (221,7km) → Braunschweig → 11:26 (57,2km) → Hannover → 30:49 (155km) → Hamburg → 25:09 (204,4km) → Schortens/Wilhelmshaven → 27:50 (139,2km) → Lingen (Ems) → 14:29 (72,5km) → Münster → 16:30 (82,6km) → Essen → 5:56 (29,7km) → Düsseldorf → 10:15 (51,3km) → Köln/Hürth → 34:29 (172,4km) → Frankfurt → 21:30 (107,5km) → Walldorf → 7:02 (35,2km) → Bretten → 25:49 (129,1km) → St. Georgen → 21:06 (105,5km) → Stuttgart → 37:26 (187,2km) → Nürnberg → 17:48 (89km) → Ingolstadt → 15:42 (78,5km) → Ismaning/München (Hauptsitz)
https://maps.openrouteservice.org/directions?a=48.229035,11.686153,48.571989,13.453256,50.829383,12.914737,51.145511,14.970028,52.580911,13.293884,52.278748,10.524797,52.337987,9.769706,53.557577,9.986065,53.537779,7.936809,52.519154,7.322185,51.969304,7.61428,51.450577,7.008871,51.274774,6.794912,50.886726,6.913119,50.136479,8.570963,49.295011,8.649036,49.032767,8.698372,48.126258,8.325873,48.694648,9.161239,49.429596,11.017404,48.784417,11.399106,48.229035,11.686153&b=2&c=0&k1=de-DE&k2=km



Fortbewegungsprofil: Wandern
Gesamtdauer: 556:49
Gesamtdistanz: 2863,07km

Ismaning/München (Hauptsitz) → 34:50 (174,2km) → Passau → 64:29 (322,5km) → Chemnitz → 36:12 (181km) → Görlitz → 45:15 (226,3km) → Berlin → 44:35 (222,8km) → Braunschweig → 11:41 (58,5km) → Hannover → 31:59 (160,9km) → Hamburg → 25:32 (206,2km) → Schortens/Wilhelmshaven → 28:12 (141,1km) → Lingen (Ems) → 15:06 (75,5km) → Münster → 17:23 (86,9km) → Essen → 5:48 (29km) → Düsseldorf → 10:38 (53,2km) → Köln/Hürth → 35:05 (175,4km) → Frankfurt → 21:29 (107,5km) → Walldorf → 7:08 (35,7km) → Bretten → 26:27 (132,3km) → St. Georgen → 21:17 (106,5km) → Stuttgart → 39:43 (198,4km) → Nürnberg → 17:57 (89,8km) → Ingolstadt → 15:52 (79,4km) → Ismaning/München (Hauptsitz)
https://maps.openrouteservice.org/directions?a=48.229035,11.686153,48.571989,13.453256,50.829383,12.914737,51.145511,14.970028,52.580911,13.293884,52.278748,10.524797,52.337987,9.769706,53.557577,9.986065,53.537779,7.936809,52.519154,7.322185,51.969304,7.61428,51.450577,7.008871,51.274774,6.794912,50.886726,6.913119,50.136479,8.570963,49.295011,8.649036,49.032767,8.698372,48.126258,8.325873,48.694648,9.161239,49.429596,11.017404,48.784417,11.399106,48.229035,11.686153&b=2b&c=0&k1=de-DE&k2=km