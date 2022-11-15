using System;
using System.Collections.Generic;
using System.Text;

namespace universe_project
{
    class ProgressViewer
    {
        public int totalPlanetFiles;
        public int actualPlanetNumberFiles = 0;
        static private readonly object counterLock = new object();

        // Gestion de l'avancement de la désérialisation via un event
        public void OnReceived(object sender, EventArgs e)
        {
            lock (counterLock)
            {
                actualPlanetNumberFiles++;
            }
           
            int progress = Convert.ToInt32(((double)actualPlanetNumberFiles / totalPlanetFiles) * 100);
            Console.Write("\rProgress : " + progress + "%");
        }
    }
}
