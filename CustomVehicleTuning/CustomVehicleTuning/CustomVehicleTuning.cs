using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using HeiswayiNrird.SimpleLogger;

namespace CustomVehicleTuning
{
    public class CustomVehicleTuning : Script
    {
        public static SimpleLogger logger;

        private bool vehicleApplied;
        private Vehicle vehicle;
        private Vehicle oldVehicle;

        public CustomVehicleTuning()
        {
            logger = new SimpleLogger(false);
            vehicleApplied = false;
            Vehicles.RegisterDefaultSettings();

            Tick += OnTick;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
        }

        public void OnTick(object sender, EventArgs e)
        {
            Player player = Game.Player;
            if(player.Character.IsInVehicle())
            {
                Vehicle vehicle = player.Character.CurrentVehicle;
                if (oldVehicle == null)
                {
                    oldVehicle = vehicle;
                }
                else if (oldVehicle != vehicle)
                {
                    vehicleApplied = false;
                    oldVehicle = vehicle;
                }
                if(vehicle.Model.IsCar)
                {
                    for(int i = 0; i < Vehicles.vehicles.Count; i++)
                    {
                        VehicleSettings stockVehicle = Vehicles.vehicles[i];
                        if(vehicle.DisplayName.ToUpper() == stockVehicle.GetDisplayName().ToUpper())
                        {
                            if (!vehicleApplied)
                            {
                                stockVehicle.ApplySettings(vehicle);
                                vehicleApplied = true;
                            }
                        }
                        // Rev Limiter
                        if (vehicle.CurrentGear == vehicle.HighGear)
                        {
                            if (vehicle.CurrentRPM >= 1f)
                            {
                                vehicle.CurrentRPM = 0.95f;
                            }
                        }
                    }
                    Utils.ShowText(0.7f, 0.4f, "Transmission Type: Stock");
                    Utils.ShowText(0.7f, 0.43f, "Total Gears: " + vehicle.HighGear);
                    Utils.ShowText(0.7f, 0.46f, "Gear 1 Ratio: " + VehicleOffset.GetGearRatio(vehicle, 1));
                    Utils.ShowText(0.7f, 0.49f, "Gear 2 Ratio: " + VehicleOffset.GetGearRatio(vehicle, 2));
                    Utils.ShowText(0.7f, 0.52f, "Gear 3 Ratio: " + VehicleOffset.GetGearRatio(vehicle, 3));
                    Utils.ShowText(0.7f, 0.55f, "Gear 4 Ratio: " + VehicleOffset.GetGearRatio(vehicle, 4));
                    Utils.ShowText(0.7f, 0.58f, "Gear 5 Ratio: " + VehicleOffset.GetGearRatio(vehicle, 5));
                    Utils.ShowText(0.7f, 0.61f, "Gear 6 Ratio: " + VehicleOffset.GetGearRatio(vehicle, 6));
                    Utils.ShowText(0.7f, 0.64f, "Gear 7 Ratio: " + VehicleOffset.GetGearRatio(vehicle, 7));
                    Utils.ShowText(0.7f, 0.67f, "Final Drive: " + VehicleOffset.GetFinalDrive(vehicle));                
                    Utils.ShowText(0.85f, 0.4f, "Grip: " + VehicleOffset.GetTractionCurveMax(vehicle));
                    Utils.ShowText(0.85f, 0.43f, "Clutch Shift Down: " + VehicleOffset.GetClutchShiftDown(vehicle));
                    Utils.ShowText(0.85f, 0.46f, "Drive Force: " + VehicleOffset.GetDriveForce(vehicle));
                    Utils.ShowText(0.85f, 0.49f, "Drive Max Flat Vel: " + VehicleOffset.GetDriveMaxFlatVel(vehicle));
                    Utils.ShowText(0.85f, 0.52f, "Initial Drive Max Flat Vel: " + VehicleOffset.GetInitialDriveMaxFlatVel(vehicle));
                }
            }
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {

        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {

        }
    }

    public class Utils
    {
        public static void ShowText(float x, float y, string text, float size = 0.3f)
        {
            Function.Call(Hash.SET_TEXT_FONT, 0);
            Function.Call(Hash.SET_TEXT_SCALE, size, size);
            Function.Call(Hash.SET_TEXT_COLOUR, 255, 255, 255, 255);
            Function.Call(Hash.SET_TEXT_WRAP, 0.0, 1.0);
            Function.Call(Hash.SET_TEXT_CENTRE, 0);
            Function.Call(Hash.SET_TEXT_OUTLINE, true);
            Function.Call(Hash._SET_TEXT_ENTRY, "STRING");
            Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, text);
            Function.Call(Hash._DRAW_TEXT, x, y);
        }
    }
}
