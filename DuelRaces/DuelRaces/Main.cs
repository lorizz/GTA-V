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

namespace DuelRaces
{
    public class Main : Script
    {
        private const string MOD_ID = "thelorizz_dualraces";
        private const string MOD_NAME = "Duel Races";
        private const string MOD_VERSION = "Beta 1.0";

        private static bool isInCutscene;
        private static bool recording;

        // Races
        private bool isInKenjiRace;

        public static SimpleLogger logger;

        public Main()
        {
            logger = new SimpleLogger(false);
            logger.Info("Initializing " + MOD_NAME + " " + MOD_VERSION);
            this.Tick += OnTick;
            this.KeyDown += OnKeyDown;
            isInCutscene = false;
            recording = false;

            // Initializes all races
            Races.RaceRegistry.RegisterRaces();
        }

        int counter = 0;
        public void OnTick(object sender, EventArgs e)
        {
            // DEBUG ONLY
            Function.Call(Hash.SET_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, 0f);
            if (recording)
            {
                counter++;
                UI.ShowSubtitle("Counter: " + counter);
                if (counter > 100)
                {
                    Vehicle veh = Game.Player.Character.CurrentVehicle;
                    string speed = veh.Speed.ToString();
                    Vector3 pos = veh.Position;
                    logger.SpeedVector("new SpeedVector(" + 
                        pos.X.ToString().Replace(',', '.') + "f, " 
                        + pos.Y.ToString().Replace(',', '.') + "f, " 
                        + pos.Z.ToString().Replace(',', '.') + "f, " 
                        + speed.Replace(',', '.') + "f, "
                        + "),");
                    counter = 0;
                }
            }
            if(isInKenjiRace)
                Races.RaceRegistry.kenjiDuel.OnTick(sender, e);
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.K)
            {
                /*if(Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Model.IsCar)
                {
                    Vehicle veh = Game.Player.Character.CurrentVehicle;
                    Races.RaceRegistry.kenjiDuel.Start();
                }*/
                Races.RaceRegistry.kenjiDuel.Start();
                isInKenjiRace = true;
            }
            if (e.KeyCode == Keys.I)
            {
                if (!recording)
                {
                    if (Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Model.IsCar)
                        recording = true;
                }
                else
                {
                    recording = false;
                }
            }
        }

        public static bool IsInCutscene
        {
            get
            {
                return isInCutscene;
            }
            set
            {
                isInCutscene = value;
            }
        }
    }

    public class Utils
    {
        public static void ToolTip(string text)
        {
            Function.Call(Hash._SET_TEXT_COMPONENT_FORMAT, "STRING");
            Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, text);
            Function.Call(Hash._0x238FFE5C7B0498A6, 0, 0, 1, -1);
        }

        public static float deltaTime
        {
            get
            {
                return Function.Call<float>(Hash.TIMESTEP);
            }
        }
    }

    public class Mathf
    {
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static float CubicEaseIn(float p)
        {
            return p * p * p;
        }
    }
   
    public class Effects
    {
        public static void SmokeEffect(Vehicle veh)
        {
            if (veh != null)
            {
                if (Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, "scr_carsteal4"))
                {
                    Function.Call(Hash.REQUEST_PTFX_ASSET, "scr_carsteal4");
                    Function.Call<int>(Hash.START_PARTICLE_FX_NON_LOOPED_ON_ENTITY, "scr_carsteal4_wheel_burnout", veh, 0.0f, -1f, 0.85f, 0, 0, 0, 1f, 0, 1, 0);
                    Function.Call(Hash.SET_PARTICLE_FX_NON_LOOPED_COLOUR, 255.0f, 255.0f, 255.0f);
                }
            }
        }
    }

    public class Worldf
    {
        public static void SetWaypoint(float x, float y)
        {
            Function.Call(Hash.SET_NEW_WAYPOINT, x, y);
        }
    }
}
