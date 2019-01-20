using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Math;
using GTA.Native;
using System.Xml;

namespace CustomVehicleTuning
{
    public class VehicleSettings
    {
        private string displayName;

        // Transmission
        private Transmission transmission;
        private Engine engine;

        public VehicleSettings(string par1)
        {
            displayName = par1;
            CustomVehicleTuning.logger.Info("Loading " + displayName + " stock settings...");
            LoadStockSettings();
            Vehicles.vehicles.Add(this);
            CustomVehicleTuning.logger.Info(displayName + " stock settings loaded successfully!");
        }

        public string GetDisplayName()
        {
            return displayName;
        }

        public Transmission GetTransmission()
        {
            return transmission;
        }

        public void SetTransmission(Transmission x)
        {
            transmission = x;
        }

        public void LoadStockSettings()
        {
            Engine en = new Engine("Stock");
            Transmission tr = new Transmission("Stock");
            XmlReader reader = XmlReader.Create(@"scripts/CustomVehicleTuning/" + displayName + ".xml");
            CustomVehicleTuning.logger.Info("Initializing stock settings...");
            while (reader.Read())
            {
                if((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Engine"))
                {
                    XmlReader engineTree = reader.ReadSubtree();
                    while(engineTree.Read())
                    {
                        if((engineTree.NodeType == XmlNodeType.Element) && (engineTree.Name == "Stock"))
                        {
                            XmlReader stockTree = engineTree.ReadSubtree();
                            while(stockTree.Read())
                            {
                                if(stockTree.NodeType == XmlNodeType.Element)
                                {
                                    switch(stockTree.Name)
                                    {
                                        case "MaxSpeed":
                                            en.SetMaxSpeed(stockTree.ReadElementContentAsFloat() / 4f);
                                            break;
                                        case "EnginePower":
                                            en.SetEnginePower(stockTree.ReadElementContentAsFloat());
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
                else if((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Transmission"))
                {
                    XmlReader transmissionTree = reader.ReadSubtree();
                    while(transmissionTree.Read())
                    {
                        if((transmissionTree.NodeType == XmlNodeType.Element) && (transmissionTree.Name == "Stock"))
                        {
                            XmlReader stockTree = transmissionTree.ReadSubtree();
                            while(stockTree.Read())
                            {
                                if(stockTree.NodeType == XmlNodeType.Element)
                                {                             
                                    switch(stockTree.Name)
                                    {
                                        case "Gears":
                                            tr.SetTotalGears(stockTree.ReadElementContentAsInt());
                                            break;
                                        case "FinalDrive":
                                            tr.SetFinalDrive(stockTree.ReadElementContentAsFloat());
                                            break;
                                        case "First":
                                            tr.SetGearRatio(1, stockTree.ReadElementContentAsFloat() / tr.GetFinalDrive());
                                            break;
                                        case "Second":
                                            tr.SetGearRatio(2, stockTree.ReadElementContentAsFloat() / tr.GetFinalDrive());
                                            break;
                                        case "Third":
                                            tr.SetGearRatio(3, stockTree.ReadElementContentAsFloat() / tr.GetFinalDrive());
                                            break;
                                        case "Fourth":
                                            tr.SetGearRatio(4, stockTree.ReadElementContentAsFloat() / tr.GetFinalDrive());
                                            break;
                                        case "Fifth":
                                            tr.SetGearRatio(5, stockTree.ReadElementContentAsFloat() / tr.GetFinalDrive());
                                            break;
                                        case "Sixth":
                                            tr.SetGearRatio(6, stockTree.ReadElementContentAsFloat() / tr.GetFinalDrive());
                                            break;
                                        case "Seventh":
                                            tr.SetGearRatio(7, stockTree.ReadElementContentAsFloat() / tr.GetFinalDrive());
                                            break;
                                        case "Clutch":
                                            tr.SetClutch(stockTree.ReadElementContentAsFloat());
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            engine = en;
            transmission = tr;
            CustomVehicleTuning.logger.Info("Stock settings loaded!");
        }

        public void ApplySettings(Vehicle vehicle)
        {
            Engine en = engine;
            vehicle.MaxSpeed = en.GetMaxSpeed();
            VehicleOffset.SetDriveMaxFlatVel(vehicle, en.GetEnginePower());

            Transmission tr = transmission;
            vehicle.HighGear = tr.GetTotalGears();
            VehicleOffset.SetGearRatios(vehicle, tr.GetGearRatios());
            VehicleOffset.SetFinalDrive(vehicle, tr.GetFinalDrive());
            VehicleOffset.SetClutchShiftUp(vehicle, tr.GetClutch());
            VehicleOffset.SetClutchShiftDown(vehicle, tr.GetClutch());
        }
    }
}
