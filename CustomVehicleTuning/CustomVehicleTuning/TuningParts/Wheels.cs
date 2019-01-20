using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomVehicleTuning.TuningParts
{
    public class Wheels
    {
        private string name;
        private float grip;

        public Wheels(string par1)
        {
            name = par1;
        }

        public string GetName()
        {
            return name;
        }

        public void SetName(string x)
        {
            name = x;
        }

        public float GetGrip()
        {
            return grip;
        }

        public void SetGrip(float x)
        {
            grip = x;
        }
    }
}
