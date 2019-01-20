using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Math;
using GTA.Native;

namespace DuelRaces.Races
{
    public class TestCanyon : RaceTypeCanyon
    {
        private TaskSequence playerSequence;
        private TaskSequence npcSequence;

        private int npcCounterTaskSequence;

        public TestCanyon()
        {
            this.raceName = "Test Canyon 1";
            this.npcPath = new SpeedVector[]
            {
                
            };
            npcCounterTaskSequence = 2;
        }

        public override void CreateFirstTaskSequence()
        {
            if (Ped.Exists(Game.Player.Character))
            {
                Vehicle veh = Game.Player.Character.CurrentVehicle;
                playerSequence = new TaskSequence();
                for (int i = 0; i < 4; i++)
                {
                    playerSequence.AddTask.DriveTo(veh, npcPath[i].GetPosition(), 3f, 30f, (int)DrivingStyle.Rushed);
                }
                playerSequence.Close();
                Game.Player.Character.Task.PerformSequence(playerSequence);
            }
            if (Ped.Exists(this.npc.GetPedOnSeat(VehicleSeat.Driver)))
            {
                npcSequence = new TaskSequence();
                for (int i = npcCounterTaskSequence; i < 10; i++)
                {
                    float speed = i > 4 ? npcPath[i].GetSpeed() : 30f;
                    npcSequence.AddTask.DriveTo(this.npc, npcPath[i].GetPosition(), 3f, speed, (int)DrivingStyle.Rushed);
                    npcCounterTaskSequence++;
                }
                npcSequence.Close();
                this.npc.GetPedOnSeat(VehicleSeat.Driver).Task.PerformSequence(npcSequence);
            }
        }

        public override void PreloadRace()
        {
            Vehicle[] vehicles = World.GetAllVehicles();
            Ped[] peds = World.GetAllPeds();
            for(int i = 0; i < vehicles.Length; i++)
            {
                vehicles[i].Delete();
            }
            for(int i = 0; i < peds.Length; i++)
            {
                if(peds[i] != Game.Player.Character)
                {
                    peds[i].Delete();
                }
            }
            this.player = World.CreateVehicle(VehicleHash.Infernus, npcPath[0].GetPosition());
            this.player.Heading = 150f;
            this.npc = World.CreateVehicle(VehicleHash.Infernus, npcPath[2].GetPosition());
            this.npc.Heading = 150f;
            Game.Player.Character.SetIntoVehicle(this.player, VehicleSeat.Driver);
            Ped ped = World.CreatePed(PedHash.Michelle, this.npc.Position);
            Wait(10);
            ped.SetIntoVehicle(this.npc, VehicleSeat.Driver);
            CreateFirstTaskSequence();
        }

        int currentCp = 2;
        public override void Update()
        {
            // Update NPC
            // Add new TaskSequence after finishing the previous one
            float distanceToNextTaskPos = Vector3.Distance(this.npc.Position, npcPath[npcCounterTaskSequence + 1].GetPosition());
            if(distanceToNextTaskPos < 3.0f)
            {
                currentCp++;
            }
            if(currentCp == 9 || currentCp == 19 || currentCp == 29 || currentCp == 39 || currentCp == 49 || currentCp == 59)
            {
                npcSequence.Dispose();
                npcSequence = new TaskSequence();
                for(int i = 0; i < npcCounterTaskSequence + 10; i++)
                {
                    npcSequence.AddTask.DriveTo(this.npc, npcPath[i].GetPosition(), 3f, npcPath[i].GetSpeed(), (int)DrivingStyle.Rushed);
                    npcCounterTaskSequence++;
                }
                npcSequence.Close();
                this.npc.GetPedOnSeat(VehicleSeat.Driver).Task.PerformSequence(npcSequence);
            }
        }

        public override void HandleEnding()
        {

        }
    }
}
