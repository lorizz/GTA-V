using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomVehicleTuning
{
    public class Transmission
    {
        private string name;
        private int totalGears;
        private float gear1, gear2, gear3, gear4, gear5, gear6, gear7;
        private float finalDrive;
        private float clutch;
        private float driveForce;

        public Transmission(string par1)
        {
            name = par1;
            totalGears = 6;
            gear1 = 3.3F;
            gear2 = 1.9F;
            gear3 = 1.5F;
            gear4 = 1.2F;
            gear5 = 1.0F;
            gear6 = 0.9F;
            gear7 = 0.6f;
            clutch = 2f;
        }

        public string GetName()
        {
            return name;
        }

        public void SetName(string x)
        {
            name = x;
        }

        public int GetTotalGears()
        {
            return totalGears;
        }

        public void SetTotalGears(int x)
        {
            totalGears = x;
        }

        public float GetGearRatio(int number)
        {
            switch(number)
            {
                case 1:
                    return gear1;
                case 2:
                    return gear2;
                case 3:
                    return gear3;
                case 4:
                    return gear4;
                case 5:
                    return gear5;
                case 6:
                    return gear6;
                case 7:
                    return gear7;
                default:
                    return -3.3F;
            }
        }

        public void SetGearRatio(int number, float value)
        {
            switch(number)
            {
                case 1:
                    gear1 = value;
                    break;
                case 2:
                    gear2 = value;
                    break;
                case 3:
                    gear3 = value;
                    break;
                case 4:
                    gear4 = value;
                    break;
                case 5:
                    gear5 = value;
                    break;
                case 6:
                    gear6 = value;
                    break;
                case 7:
                    gear7 = value;
                    break;
                default:
                    return;
            }
        }

        public float GetFinalDrive()
        {
            return finalDrive;
        }

        public void SetFinalDrive(float x)
        {
            finalDrive = x;
        }

        public float[] GetGearRatios()
        {
            float[] ratios = new float[8];
            ratios[0] = -3.3F;
            ratios[1] = gear1;
            ratios[2] = gear2;
            ratios[3] = gear3;
            ratios[4] = gear4;
            ratios[5] = gear5;
            ratios[6] = gear6;
            ratios[7] = gear7;
            return ratios;
        }

        public float GetClutch()
        {
            return clutch;
        }

        public void SetClutch(float x)
        {
            clutch = x;
        }

        public float GetDriveForce()
        {
            return driveForce;
        }

        public void SetDriveForce(float x)
        {
            driveForce = x;
        }
    }
}
