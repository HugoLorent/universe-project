using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace universe_project
{
    class Question2
    {
        public static ProgressViewer progressViewer = new ProgressViewer();

        public async Task<Universe> DeserializeAll(string directoryPath)
        {
            long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            Console.WriteLine("La version asynchrone commence !");
            Universe universe = new Universe() { systems = new List<System>() };

            // Lecture de tous les noms de dossiers dans c:/universe
            List<string> systems = Directory.GetDirectories(directoryPath, "*", SearchOption.TopDirectoryOnly).ToList();

            // Création de la liste de toutes les taches qui seront lancés
            List<Task<System>> systemsTask = new List<Task<System>>();

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
                // Lancement des taches en parallèle
                Task<System> systemTask = Task.Run(() =>
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

                    return system;
                });
                systemsTask.Add(systemTask);
            }

            // On attend que toutes les taches soient finies
            await Task.WhenAll(systemsTask);

            foreach (Task<System> systemTask in systemsTask)
            {
                universe.systems.Add(systemTask.Result);
            }

            long end = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long time = end - start;
            Console.WriteLine("\nLa version asynchrone a fini en " + time + " millisecondes");
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
