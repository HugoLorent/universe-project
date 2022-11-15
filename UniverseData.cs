using System;
using System.Collections.Generic;
using System.Text;

namespace universe_project
{
    class Universe
    {
        public List<System> systems;
    }

    class System
    {
        public string name;
        public List<Planet> planets;
    }

    class Planet
    {
        public int size;
        public int usability;
        public int orbit;
        public string name;
        public event EventHandler finishDeserialize;

        public void isDeserialized()
        {
            finishDeserialize?.Invoke(this, EventArgs.Empty);
        }
    }
}
