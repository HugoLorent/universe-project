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
            List<string> systems = Directory.GetDirectories(directoryPath, "*", SearchOption.TopDirectoryOnly).ToList();
            List<Task<System>> systemsTask = new List<Task<System>>();

            int totalPlanetFiles = 0;

            foreach (string system in systems)
            {
                totalPlanetFiles += Directory.GetFiles(system).ToList().Count;
            }
            progressViewer.totalPlanetFiles = totalPlanetFiles;

            foreach (string systemName in systems)
            {
                Task<System> systemTask = Task.Run(() =>
                {
                    List<string> planets = Directory.GetFiles(systemName, "*", SearchOption.TopDirectoryOnly).ToList();
                    System system = new System() { planets = new List<Planet>(), name = systemName };

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
