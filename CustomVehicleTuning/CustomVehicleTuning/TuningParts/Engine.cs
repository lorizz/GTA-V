using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomVehicleTuning
{
    public class Engine
    {
        private string name;
        private float maxSpeed, enginePower;

        public Engine(string par1)
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

        public float GetMaxSpeed()
        {
            return maxSpeed;
        }

        public void SetMaxSpeed(float x)
        {
            maxSpeed = x;
        }

        public float GetEnginePower()
        {
            return enginePower;
        }

        public void SetEnginePower(float x)
        {
            enginePower = x;
        }
    }
}
