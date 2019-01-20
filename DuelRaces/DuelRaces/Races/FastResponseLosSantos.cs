using System;
using System.Drawing;
using GTA;
using GTA.Math;
using GTA.Native;

namespace DuelRaces.Races
{
    public class FastResponseLosSantos : FastResponse
    {
        public FastResponseLosSantos()
        {
            this.raceName = "Fast Response 1";
            this.cutsceneVectors = new VectorHeading[]
            {
                new VectorHeading(new Vector3(-2634.995f, 2763.161f, 16.242f), 349.390f),
                new VectorHeading(new Vector3(-2609.516f, 2937.542f, 16.228f), 352.637f),
                new VectorHeading(new Vector3(-2594.219f, 3054.833f, 15.464f), 353.453f),
                new VectorHeading(new Vector3(-2588.841f, 3160.775f, 13.938f), 2.780f),
                new VectorHeading(new Vector3(-2578.900f, 3293.115f, 12.932f), 350.723f),
                new VectorHeading(new Vector3(-2551.778f, 3418.279f, 12.772f), 344.061f)
            };
            this.startRaceMarker = cutsceneVectors[0];
        }

        public override void Update()
        {
            /*World.DrawMarker(MarkerType.VerticalCylinder, cutsceneVectors[0].GetPosition(), Vector3.WorldNorth, Vector3.Zero, new Vector3(10f, 10f, 6f), Color.White);
            World.DrawMarker(MarkerType.CheckeredFlagRect, new Vector3(cutsceneVectors[0].GetPosition().X, cutsceneVectors[0].GetPosition().Y, cutsceneVectors[0].GetPosition().Z + 7f), GameplayCamera.Direction, Vector3.Zero, new Vector3(2f, 2f, 2f), Color.White);
            var tmpSF = new Scaleform("PLAYER_NAME_01");
            tmpSF.CallFunction("SET_PLAYER_NAME", this.raceName);
            tmpSF.Render3D(cutsceneVectors[0].GetPosition() + new Vector3(0f, 0f, 6f), GameplayCamera.Rotation, new Vector3(12, 6, 2));
            Cutscene();*/
        }

        public override Vehicle CreateVehicle()
        {
            return World.CreateVehicle(new Model("POLREVENT"), cutsceneVectors[0].GetPosition(), cutsceneVectors[0].GetHeading());
        }

        public override void CreateTaskSequence()
        {
            if(Ped.Exists(Game.Player.Character))
            {
                Vehicle veh = Game.Player.Character.CurrentVehicle;
                TaskSequence sequence = new TaskSequence();
                for(int i = 0; i < cutsceneVectors.Length; i++)
                {
                    float speed = i >= 3 ? 50f : 30f;
                    sequence.AddTask.DriveTo(veh, cutsceneVectors[i].GetPosition(), 10f, speed, (int)DrivingStyle.Rushed);
                }
                sequence.Close();
                Game.Player.Character.Task.PerformSequence(sequence);
                Game.Player.Character.Task.PerformSequence(new TaskSequence());
            }
        }

        float speedPercentage = 0f;
        public override void Cutscene()
        {
            if(Main.IsInCutscene)
            {
                Vehicle veh = Game.Player.Character.CurrentVehicle;
                UI.ShowSubtitle("Time " + Math.Ceiling(timeCounter).ToString() + " Speed: " + Math.Ceiling(veh.Speed).ToString());
                timeCounter += Utils.deltaTime * 1;
                veh.Speed = 20f;
                if (timeCounter > 11)
                {
                    if (veh.SirenActive == false)
                        veh.SirenActive = true;
                    veh.Speed = Mathf.Lerp(veh.Speed, 50f, speedPercentage);
                    if(veh.Speed < 50f)
                        speedPercentage += 0.3f * Utils.deltaTime;
                }
                if (timeCounter > 20)
                {
                    Game.Player.Character.Task.PerformSequence(new TaskSequence());
                    Main.IsInCutscene = false;
                }
            }
        }

        public override Vehicle GetVehicle(string id)
        {
            switch (id)
            {
                case "PLAYER":
                    return Game.Player.Character.CurrentVehicle;
                default:
                    return null;
            }
        }
    }
}
