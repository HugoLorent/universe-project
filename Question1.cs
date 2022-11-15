using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace universe_project
{
    class Question1
    {
        public static ProgressViewer progressViewer = new ProgressViewer();

        public Universe DeserializeAll(string directoryPath)
        {
            long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            Console.WriteLine("La version synchrone commence !");
            Universe universe = new Universe() { systems = new List<System>() };

            // Lecture de tous les noms de dossiers dans c:/universe
            List<string> systems = Directory.GetDirectories(directoryPath, "*", SearchOption.TopDirectoryOnly).ToList();

            // Permet d'obtenir le nombre de planètes total
            int totalPlanetFiles = 0;
            foreach (string system in systems)
            {
                totalPlanetFiles += Directory.GetFiles(system).ToList().Count;
            }
            progressViewer.totalPlanetFiles = totalPlanetFiles;

            // On parcourt chaque dossier "System xxxx"
            foreach (string systemName in systems)
            {
                // Lecture de tous les noms de fichier dans chaque dossier "System xxxx"
                List<string> planets = Directory.GetFiles(systemName, "*", SearchOption.TopDirectoryOnly).ToList();
                System system = new System() { planets = new List<Planet>(), name = systemName };

                // Désérialisation de chaque planète
                foreach (string planetName in planets)
                {
                    string path = Path.Combine(systemName, planetName);
                    Planet planet = DeserializePlanet(path);
                    system.planets.Add(planet);
                }

                universe.systems.Add(system);
            }

            long end = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long time = end - start;
            Console.WriteLine("\nLa version synchrone a fini en " + time + " millisecondes");
            return universe;
        }

        // Désérialise une planète
        public Planet DeserializePlanet(string filePath)
        {
            string data = File.ReadAllText(filePath);
            Planet result = JsonConvert.DeserializeObject<Planet>(data);
            result.finishDeserialize += progressViewer.OnReceived;
            result.isDeserialized();
            return result;
        }
    }
}
