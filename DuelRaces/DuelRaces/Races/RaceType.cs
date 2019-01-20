using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Math;
using GTA.Native;
using HeiswayiNrird.SimpleLogger;

namespace DuelRaces.Races
{
    public abstract class FastResponse : Script
    {
        public string raceName;
        public long time;
        public float timeCounter;
        public VectorHeading startRaceMarker;
        public VectorHeading[] cutsceneVectors;
        public VectorHeading endLine;
        public TaskSequence ts;
        
        public FastResponse()
        {
            this.Tick += OnTick;
        }

        public void OnTick(object sender, EventArgs e)
        {
            //Update();
            if (Main.IsInCutscene)
                //Cutscene();
            if (World.GetDistance(Game.Player.Character.Position, startRaceMarker.GetPosition()) < 8f)
            {
                Utils.ToolTip("Press ENTER to enter!");
                if (Game.IsKeyPressed(System.Windows.Forms.Keys.Enter))
                {
                    //PrepareRace();
                }
            }
        }

        public void PrepareRace()
        {
            Main.logger.Info("Preparing Fast Response 1");
            this.timeCounter = 0f;
            Vehicle veh = CreateVehicle();
            Main.logger.Info("Vehicle Created");
            Wait(10);
            veh.EngineRunning = true;
            Ped player = Game.Player.Character;
            if(player.CurrentVehicle != null)
            {
                player.CurrentVehicle.Delete();
            }
            player.SetIntoVehicle(veh, VehicleSeat.Driver);
            Main.logger.Info("Creating new Task Sequence");
            CreateTaskSequence();
            Main.IsInCutscene = true;
        }

        abstract public Vehicle CreateVehicle();
        abstract public void Update();
        abstract public void Cutscene();
        abstract public void CreateTaskSequence();
        abstract public Vehicle GetVehicle(String id);
    }

    public abstract class RaceTypeCanyon : Script
    {
        public string raceName;
        public VectorHeading startRaceMarker;
        public VectorHeading[] startingPosition;
        public VectorHeading endLine;
        public Vehicle player, npc;
        public SpeedVector[] npcPath;

        public RaceTypeCanyon()
        {
            this.Tick += OnTick;
        }

        public void OnTick(object sender, EventArgs e)
        {
            //Update();
        }

        public abstract void CreateFirstTaskSequence();
        public abstract void PreloadRace();
        public abstract void Update();
        public abstract void HandleEnding();
    }

    /*public abstract class Race : Script
    {
        public string raceName;
        public VectorHeading startRaceMarker;
        public VectorHeading[] startingPositions;
        public VectorHeading endLine;
        public bool isLoading = false;
        public bool inCountdown = false;

        public Race()
        {
            this.Tick += OnTick;
        }

        public void OnTick(object sender, EventArgs e)
        {
            Update();
            if (World.GetDistance(Game.Player.Character.Position, startRaceMarker.GetPosition()) < 8f)
            {
                Utils.ToolTip("Press ENTER to enter!");
                if (Game.IsKeyPressed(System.Windows.Forms.Keys.Enter))
                {
                    PrepareRace();
                }
            }
            Cutscene();
        }

        abstract public void Update();
        abstract public void PrepareRace();
        abstract public void Cutscene();
        abstract public Vehicle GetVehicle(String id);
    }*/

    public class RaceRegistry
    {
        //public static FastResponse fr1;
        //public static TestCanyon tc;
        public static KenjiDuel kenjiDuel;

        public static void RegisterRaces()
        {
            //fr1 = new FastResponseLosSantos();
            //tc = new TestCanyon();
            kenjiDuel = new KenjiDuel();
        }
    }

    public class VectorHeading
    {
        public Vector3 pos;
        public float heading;

        public VectorHeading(Vector3 par1, float par2)
        {
            this.pos = par1;
            this.heading = par2;
        }

        public Vector3 GetPosition()
        {
            return this.pos;
        }

        public float GetHeading()
        {
            return this.heading;
        }
    }

    public class SpeedVector
    {
        private Vector3 pos;
        private float speed;

        public SpeedVector(float par1, float par2, float par3, float par4)
        {
            this.pos = new Vector3(par1, par2, par3);
            this.speed = par4;
        }

        public Vector3 GetPosition()
        {
            return pos;
        }

        public float GetSpeed()
        {
            return speed;
        }
    }
}
