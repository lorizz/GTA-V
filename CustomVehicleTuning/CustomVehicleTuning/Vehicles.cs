using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomVehicleTuning
{
    public class Vehicles
    {
        public static List<VehicleSettings> vehicles;
        public static VehicleSettings JESTER3, NSX;

        public static void RegisterDefaultSettings()
        {
            vehicles = new List<VehicleSettings>();
            JESTER3 = new VehicleSettings("JESTER3");
            NSX = new VehicleSettings("NSX");
        }
    }
}
