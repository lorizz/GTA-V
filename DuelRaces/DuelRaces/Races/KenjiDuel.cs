using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Math;
using GTA.Native;
using NAudio.Wave;

namespace DuelRaces.Races
{
    public class KenjiDuel
    {
        public string name;
        public bool isPlaying;
        public int round;
        public int points;

        /* CUTSCENE */
        // Player
        public VectorHeading startPlayerPos;
        public Vehicle playerVehicle;

        // NPC
        public VectorHeading startKenjiPos;
        public Vehicle kenjiVehicle;
        public float counter;
        public int currentFrame;

        // Audio
        public WaveStream outputStream;
        public WaveChannel32 channel;
        public DirectSoundOut soundOut;

        // Camera
        public Camera cam;      

        public KenjiDuel()
        {
            Main.logger.Info("Loading Kenji Duel");
            this.name = "Kenji Duel";
            World.RenderingCamera = null;
            World.DestroyAllCameras();

            isPlaying = false;
            round = 1;
            points = 0;
        }

        public void Start()
        {
            if (outputStream != null)
            {
                outputStream.Dispose();
                channel.Dispose();
                soundOut.Dispose();
            }
            outputStream = new WaveFileReader("scripts/duelraces/sounds/kenji.wav");
            channel = new WaveChannel32(outputStream);
            soundOut = new DirectSoundOut();
            Vehicle[] vehicleList = World.GetAllVehicles();
            for(int i = 0; i < vehicleList.Length; i++)
            {
                if(vehicleList[i] != kenjiVehicle && vehicleList[i] != playerVehicle)
                    if(vehicleList[i] != Game.Player.Character.CurrentVehicle)
                        vehicleList[i].Delete();
            }
            Ped[] pedList = World.GetAllPeds();
            for(int i = 0; i < pedList.Length; i++)
            {
                if (pedList[i] != Game.Player.Character)
                    pedList[i].Delete();
            }
            startPlayerPos = new VectorHeading(new Vector3(-4860.342f, -941.994f, 483.782f), 282.666f);
            startKenjiPos = new VectorHeading(new Vector3(-4554.846f, -865.514f, 446.045f), 75.574f);

            if (Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Model.IsCar)
                playerVehicle = Game.Player.Character.CurrentVehicle;
            else
                playerVehicle = World.CreateVehicle(VehicleHash.Massacro, Vector3.Zero);
            playerVehicle.Position = startPlayerPos.GetPosition();
            playerVehicle.Heading = startPlayerPos.GetHeading();
            playerVehicle.EngineRunning = true;
            playerVehicle.IsInvincible = true;

            kenjiVehicle = World.CreateVehicle(VehicleHash.Elegy, Vector3.Zero);
            kenjiVehicle = World.CreateVehicle("RX7TUNABLE", Vector3.Zero);
            kenjiVehicle.InstallModKit();
            kenjiVehicle.SetMod(VehicleMod.Spoilers, 0, true);
            kenjiVehicle.SetMod(VehicleMod.FrontBumper, 2, true);
            kenjiVehicle.SetMod(VehicleMod.RearBumper, 2, true);
            kenjiVehicle.SetMod(VehicleMod.SideSkirt, 2, true);
            kenjiVehicle.SetMod(VehicleMod.Exhaust, 2, true);
            // kenjiVehicle.SetMod(VehicleMod.)
            kenjiVehicle.SetMod(VehicleMod.Hood, 2, true);
            kenjiVehicle.SetMod(VehicleMod.Engine, 4, true);
            kenjiVehicle.SetMod(VehicleMod.Brakes, 3, true);
            kenjiVehicle.SetMod(VehicleMod.Transmission, 3, true);
            kenjiVehicle.SetMod(VehicleMod.Suspension, 4, true);
            kenjiVehicle.SetMod(VehicleMod.Armor, 5, true);
            kenjiVehicle.ToggleMod(VehicleToggleMod.Turbo, true);
            kenjiVehicle.ToggleExtra(2, true);
            kenjiVehicle.PrimaryColor = VehicleColor.UtilDarkGreen;
            kenjiVehicle.IsInvincible = true;

            kenjiVehicle.Position = startKenjiPos.GetPosition();
            kenjiVehicle.Heading = startKenjiPos.GetHeading();
            kenjiVehicle.EngineRunning = true;

            // NPC
            Game.Player.Character.SetIntoVehicle(playerVehicle, VehicleSeat.Driver);
            Ped randomPed = World.CreatePed(PedHash.HaoCutscene, Vector3.Zero);
            randomPed.SetIntoVehicle(kenjiVehicle, VehicleSeat.Driver);

            // Camera
            cam = World.CreateCamera(new Vector3(-4700.114f, -902.6223f, 472.6724f), new Vector3(0f, 0f, 130f), GameplayCamera.FieldOfView);
            World.RenderingCamera = cam;

            // Time
            World.CurrentDayTime = new TimeSpan(3, 0, 0);
            World.Weather = Weather.Foggy;

            Worldf.SetWaypoint(-4718.075f, 536.934f);

            counter = 0f;
            Main.IsInCutscene = true;
            soundOut.Init(channel);
            soundOut.Play();
        }

        private void CreateTask()
        {
            TaskSequence npcSequence = new TaskSequence();
            npcSequence.AddTask.DriveTo(kenjiVehicle, new Vector3(-4649.946f, -900.463f, 457.723f), 2f, 30f);
            npcSequence.Close();
            kenjiVehicle.GetPedOnSeat(VehicleSeat.Driver).Task.PerformSequence(npcSequence);

            TaskSequence playerSequence = new TaskSequence();
            playerSequence.AddTask.DriveTo(playerVehicle, new Vector3(-4649.946f, -900.463f, 457.723f), 2f, 30f);
            playerSequence.Close();
            playerVehicle.GetPedOnSeat(VehicleSeat.Driver).Task.PerformSequence(playerSequence);
        }

        private void KenjiLookAtTask()
        {
            TaskSequence kenjiSequence = new TaskSequence();
            kenjiSequence.AddTask.LookAt(playerVehicle.Driver);
            kenjiSequence.Close(true);
            kenjiVehicle.Driver.Task.PerformSequence(kenjiSequence);
        }

        private void KenjiBurnout()
        {
            Function.Call(Hash.SET_VEHICLE_BURNOUT, kenjiVehicle, true);
            TaskSequence ts = new TaskSequence();
            ts.AddTask.DriveTo(kenjiVehicle, kenjiVehicle.Position + kenjiVehicle.ForwardVector * 3, 1f, 1000f);
            ts.Close();
            kenjiVehicle.Driver.Task.PerformSequence(ts);
        }

        // Local variables
        bool j1 = false;
        float j3 = 0f;
        float j4 = 0f;
        bool j5 = false;
        float j6 = 10f; // 10f
        float j7 = 0f;

        // Camera var
        bool cam2b = false;
        bool cam3b = false;
        bool cam4b = false;
        bool cam5b = false;
        bool cam6b = false;
        bool cam7b = false;
        bool cam8b = false;
        bool cam9b = false;
        bool r2 = false;
        public void OnTick(object sender, EventArgs e)
        {
            HandleRace();
            if (Main.IsInCutscene)
            {
                if (round == 1)
                {
                    Function.Call(Hash.HIDE_HUD_AND_RADAR_THIS_FRAME);
                    Game.DisableAllControlsThisFrame(2);
                    // Init
                    if (counter >= 0f && counter < 1.702f)
                    {
                        cam.Position = Vector3.Lerp(cam.Position, new Vector3(-4710.670f, -905.827f, 472.176f), Utils.deltaTime / 10);
                    }

                    // Zoom 1
                    else if (counter >= 1.702f && counter < 3.403f)
                    {
                        if (!cam2b)
                        {
                            cam.Position = new Vector3(-4830.393f, -932.050f, 480.365f);
                            cam2b = true;
                        }
                        else
                        {
                            cam.Position = Vector3.Lerp(cam.Position, new Vector3(-4832.225f, -933.4307f, 480.527f), Utils.deltaTime / 20);
                            cam.Rotation = Vector3.Lerp(cam.Rotation, new Vector3(cam.Rotation.X, cam.Rotation.Y, 125f), Utils.deltaTime / 20);
                        }
                    }

                    // Start PointAt Kenji - Start Drift
                    else if (counter >= 3.403f && counter < 4.838f)
                    {
                        // Camera
                        if (!cam3b)
                        {
                            cam.Position = new Vector3(-4726.853f, -911.668f, 467.178f);
                            cam.FieldOfView = 45f;
                            cam3b = true;
                        }
                        else
                        {
                            cam.Position = Vector3.Lerp(cam.Position, new Vector3(-4726.853f, -921.668f, 467.178f), Utils.deltaTime / 40);
                            cam.Rotation = new Vector3(-5f, cam.Rotation.Y, 285f);
                        }
                        kenjiVehicle.Velocity = new Vector3(-25f, -13f, kenjiVehicle.Velocity.Z);
                        kenjiVehicle.Rotation = new Vector3(kenjiVehicle.Rotation.X, kenjiVehicle.Rotation.Y, Mathf.Lerp(kenjiVehicle.Rotation.Z, -50f, Utils.deltaTime / 5));
                    }

                    // Start Zoom To Kenji
                    else if (counter >= 4.838f && counter < 5.239f)
                    {
                        cam.Rotation = Vector3.Lerp(cam.Rotation, new Vector3(-5f, 0f, -285f), Utils.deltaTime / 40);
                        cam.FieldOfView = Mathf.Lerp(cam.FieldOfView, 35f, Utils.deltaTime * 10);

                        kenjiVehicle.Velocity = new Vector3(-20f, -8f, kenjiVehicle.Velocity.Z);
                        kenjiVehicle.Rotation = new Vector3(kenjiVehicle.Rotation.X, kenjiVehicle.Rotation.Y, kenjiVehicle.Rotation.Z + 0.2f);
                    }

                    //  End Drift
                    else if (counter >= 5.239f && counter < 5.439f)
                    {
                        cam.Rotation = Vector3.Lerp(cam.Rotation, new Vector3(-5f, 0f, -285f), Utils.deltaTime / 40);
                    }

                    // Zoom To Kenji Face
                    else if (counter >= 5.439f && counter < 7.241f)
                    {
                        // Camera
                        if (!cam4b)
                        {
                            cam.AttachTo(kenjiVehicle.Driver, new Vector3(0f, 1.6f, 0.7f));
                            cam.PointAt(kenjiVehicle.Driver, new Vector3(0f, -8f, 0f));
                            cam.FieldOfView = 15f;
                            cam.Shake(CameraShake.Hand, 0.4f);
                            cam4b = true;
                        }
                        else
                        {
                            cam.FieldOfView = Mathf.Lerp(cam.FieldOfView, 6f, Utils.deltaTime / 4);
                            cam.StopShaking();
                            cam.Shake(CameraShake.Hand, 0.4f);
                        }

                        float counterDiff = 7.241f - counter;
                        kenjiVehicle.Speed = 30f;
                        kenjiVehicle.Rotation = new Vector3(kenjiVehicle.Rotation.X, kenjiVehicle.Rotation.Y, 104.788f - counterDiff);
                    }

                    // End Kenji View - Start Double View
                    else if (counter >= 7.241f && counter < 7.441f)
                    {
                        if (!cam5b)
                        {
                            cam.Detach();
                            cam.StopPointing();
                            cam.StopShaking();
                            cam.Position = new Vector3(-4864.116f, -941.532f, 484.034f);
                            cam.Rotation = new Vector3(-2f, 0f, 282f);
                            cam.FieldOfView = GameplayCamera.FieldOfView;
                            cam5b = true;
                        }
                        else
                        {
                            cam.Position = Vector3.Lerp(cam.Position, new Vector3(-4863.248f, -941.328f, 483.914f), Utils.deltaTime / 10);
                        }

                        if (!j1)
                        {
                            kenjiVehicle.Position = new Vector3(-4852.475f, -937.756f, 482.755f);
                            kenjiVehicle.Rotation = new Vector3(kenjiVehicle.Rotation.X, kenjiVehicle.Rotation.Y, 102.840f);
                            j1 = true;
                        }

                        kenjiVehicle.Speed = 18f;
                    }

                    // Start RX7 Braking
                    else if (counter >= 7.441f && counter < 8.709f)
                    {
                        cam.Position = Vector3.Lerp(cam.Position, new Vector3(-4863.248f, -941.328f, 483.914f), Utils.deltaTime / 10);
                        kenjiVehicle.Speed = Mathf.Lerp(kenjiVehicle.Speed, 0f, Utils.deltaTime);
                    }

                    // End RX7 Braking
                    else if (counter >= 8.709f && counter < 10.110f)
                    {
                        cam.Position = Vector3.Lerp(cam.Position, new Vector3(-4863.248f, -941.328f, 483.914f), Utils.deltaTime / 10);
                    }

                    // RX7 Burnout Reverse
                    else if (counter >= 10.110f && counter < 10.944f)
                    {
                        //KenjiBurnout(); NOT WORKING :((((
                    }

                    // Start LookAt Kenji From Player
                    else if (counter >= 10.944f && counter < 11.879f)
                    {
                        kenjiVehicle.Driver.Task.ClearAll();
                        Function.Call(Hash.SET_VEHICLE_BURNOUT, kenjiVehicle, false);
                        if (counter > 10.50)
                        {
                            cam.Position = playerVehicle.Driver.Position + new Vector3(0f, 1f, 0.6f);
                            cam.PointAt(kenjiVehicle.Driver, new Vector3(0f, -0.3f, 0.6f));
                            if (!cam6b)
                            {
                                cam.FieldOfView = 25f;
                                KenjiLookAtTask();
                                cam6b = true;
                            }
                            else
                            {
                                cam.FieldOfView = Mathf.Lerp(cam.FieldOfView, 10f, Utils.deltaTime / 10);
                            }
                        }

                        if (kenjiVehicle.Speed == 0f)
                            kenjiVehicle.Speed = -9.5f;
                        kenjiVehicle.Speed = Mathf.Lerp(-kenjiVehicle.Speed, 0f, Utils.deltaTime / 40);
                    }

                    // #approx End RX7 Reverse (speech)
                    else if (counter >= 11.879f && counter < 15.616f)
                    {
                        cam.FieldOfView = Mathf.Lerp(cam.FieldOfView, 10f, Utils.deltaTime / 10);
                    }

                    // RX7 Back View - Aggressive Start
                    else if (counter >= 15.616f && counter < 16.350f)
                    {
                        if (!cam7b)
                        {
                            cam.StopPointing();
                            cam.Position = new Vector3(-4856.728f, -940.230f, 483.284f);
                            cam.Rotation = new Vector3(3f, 10f, 92.100f);
                            cam.FieldOfView = 45f;
                            cam7b = true;
                        }
                        else
                        {
                            cam.Position = Vector3.Lerp(cam.Position, new Vector3(-4857.120f, -940.423f, 483.308f), Utils.deltaTime / 20);
                        }

                        kenjiVehicle.Driver.Task.ClearAll();
                        kenjiVehicle.Speed = 6f;
                    }

                    // RX7 Back View Offset - Start RX7 Drift Turn Back
                    else if (counter >= 16.350f && counter < 16.948f)
                    {
                        cam.Position = Vector3.Lerp(cam.Position, new Vector3(-4857.061f, -942.219f, 483.308f), Utils.deltaTime * 3);
                        kenjiVehicle.Speed = 6f;
                    }

                    // Slow Down Offset
                    else if (counter >= 16.948f && counter < 17.351f)
                    {
                        cam.Position = Vector3.Lerp(cam.Position, new Vector3(-4857.838f, -943.128f, 483.378f), Utils.deltaTime * 1.5f);
                        kenjiVehicle.Speed = 6f;
                    }

                    // Start Intense Drift
                    else if (counter >= 17.351f && counter < 19.653f)
                    {
                        cam.Rotation = Vector3.Lerp(cam.Rotation, new Vector3(cam.Rotation.X, cam.Rotation.Y, 280f), Utils.deltaTime / 35);

                        j3 += Utils.deltaTime * 1;
                        if (kenjiVehicle.Speed != 0f)
                            kenjiVehicle.Speed = 0f;
                        kenjiVehicle.Velocity = new Vector3(j3 * 2, -2.5f + j3 / 3, -0.1f);
                        kenjiVehicle.Rotation = new Vector3(kenjiVehicle.Rotation.X - 0.1f, kenjiVehicle.Rotation.Y + 0.1f, kenjiVehicle.Rotation.Z + 2f - j3 / 2);
                    }

                    // End Intense Drift - Zoom To Kenji
                    else if (counter >= 19.653f && counter < 19.853f)
                    {
                        cam.Rotation = Vector3.Lerp(cam.Rotation, new Vector3(cam.Rotation.X, cam.Rotation.Y, 280f), Utils.deltaTime / 35);
                        kenjiVehicle.Rotation = Vector3.Lerp(kenjiVehicle.Rotation, new Vector3(kenjiVehicle.Rotation.X, kenjiVehicle.Rotation.Y, -100f), Utils.deltaTime);
                        cam.FieldOfView = Mathf.Lerp(cam.FieldOfView, 20f, Utils.deltaTime);


                        j4 += Utils.deltaTime * 1;
                        kenjiVehicle.Rotation = new Vector3(kenjiVehicle.Rotation.X - 0.1f, kenjiVehicle.Rotation.Y - 0.1f, kenjiVehicle.Rotation.Z - 0.4f);
                        kenjiVehicle.Velocity = new Vector3(kenjiVehicle.Velocity.X + 0.3f, kenjiVehicle.Velocity.Y + 0.2f, kenjiVehicle.Velocity.Z);
                    }

                    // End Zoom
                    else if (counter >= 19.853f && counter < 20.621f)
                    {
                        cam.Rotation = Vector3.Lerp(cam.Rotation, new Vector3(cam.Rotation.X, cam.Rotation.Y, 280f), Utils.deltaTime / 35);

                        j4 += Utils.deltaTime * 1;
                        kenjiVehicle.Velocity = new Vector3(kenjiVehicle.Velocity.X + 0.3f, kenjiVehicle.Velocity.Y, kenjiVehicle.Velocity.Z);
                    }

                    // Second Double View
                    else if (counter >= 20.621f && counter < 22.956f)
                    {
                        if (!cam8b)
                        {
                            cam.Position = new Vector3(-4862.969f, -943.735f, 484.134f);
                            cam.Rotation = new Vector3(1f, 2f, cam.Rotation.Z - 180f);
                            cam.FieldOfView = GameplayCamera.FieldOfView;
                            cam8b = true;
                        }
                        else
                        {
                            cam.Position = Vector3.Lerp(cam.Position, new Vector3(-4818.277f, -934.292f, 484.729f), Utils.deltaTime / j6);
                            cam.Rotation = Vector3.Lerp(cam.Rotation, new Vector3(5f, 50f, cam.Rotation.Z), Utils.deltaTime);
                            if (j6 > 1f)
                                j6 -= 0.1f * Utils.deltaTime;
                            if (counter > 21f)
                            {

                            }
                        }

                        if (!j5)
                        {
                            CreateTask();
                            j5 = true;
                        }
                    }

                    // Countdown Anim
                    else if (counter >= 22.956f && counter < 23.156f)
                    {
                        if (!cam9b)
                        {
                            cam.Detach();
                            cam.StopPointing();
                            cam.StopShaking();
                            cam.Position = playerVehicle.Position + new Vector3(-30f, 3f, 5f);
                            cam.Rotation = new Vector3(1f, 3f, -56f);
                            cam9b = true;
                        }
                        else
                        {
                            Vector3 finalPos = new Vector3(playerVehicle.Position.X, playerVehicle.Position.Y, playerVehicle.Position.Z + playerVehicle.HeightAboveGround);
                            cam.AttachTo(playerVehicle, new Vector3(0f, -30f + j7, 1f - j7 / 100));
                            cam.Rotation = Vector3.Lerp(cam.Rotation, new Vector3(0f, 0f, -90f), Utils.deltaTime / 10);
                            j7 += 0.1f * Utils.deltaTime;
                        }
                    }

                    // 3
                    else if (counter >= 23.156f && counter < 24.156f)
                    {
                        UI.ShowSubtitle("3");
                        Vector3 finalPos = new Vector3(playerVehicle.Position.X, playerVehicle.Position.Y, playerVehicle.Position.Z + playerVehicle.HeightAboveGround);
                        cam.AttachTo(playerVehicle, new Vector3(0f, -30f + j7, 1f - j7 / 100));
                        cam.Rotation = Vector3.Lerp(cam.Rotation, new Vector3(0f, 0f, -90f), Utils.deltaTime / 10);
                        j7 += 0.1f * Utils.deltaTime;
                    }

                    // 2
                    else if (counter >= 24.156f && counter < 25.156f)
                    {
                        UI.ShowSubtitle("2");
                        Vector3 finalPos = new Vector3(playerVehicle.Position.X, playerVehicle.Position.Y, playerVehicle.Position.Z + playerVehicle.HeightAboveGround);
                        cam.AttachTo(playerVehicle, new Vector3(0f, -30f + j7, 1f - j7 / 100));
                        cam.Rotation = Vector3.Lerp(cam.Rotation, new Vector3(0f, 0f, -90f), Utils.deltaTime / 10);
                        j7 += 0.1f * Utils.deltaTime;
                    }

                    // 1
                    else if (counter >= 25.156f && counter < 26.156f)
                    {
                        UI.ShowSubtitle("1");
                        Vector3 finalPos = new Vector3(playerVehicle.Position.X, playerVehicle.Position.Y, playerVehicle.Position.Z + playerVehicle.HeightAboveGround);
                        cam.AttachTo(playerVehicle, new Vector3(0f, -30f + j7, 1f - j7 / 100));
                        cam.Rotation = Vector3.Lerp(cam.Rotation, new Vector3(0f, 0f, -90f), Utils.deltaTime / 10);
                        j7 += 0.1f * Utils.deltaTime;
                        float counterDiff = Mathf.Clamp(26.156f - counter, 25.156f, 26.156f) * Utils.deltaTime;
                        if (counter > 25.926f)
                        {
                            cam.Position = Vector3.Lerp(cam.Position, GameplayCamera.Position, counterDiff);
                            cam.Rotation = Vector3.Lerp(cam.Rotation, GameplayCamera.Rotation, counterDiff);
                        }
                    }

                    // Go
                    else if (counter >= 26.156f)
                    {
                        // end cutscene
                        Main.IsInCutscene = false;
                        World.RenderingCamera = null;
                        Worldf.SetWaypoint(-4718.075f, 536.934f);
                        World.DestroyAllCameras();
                        UI.ShowSubtitle("GO!");
                        Game.Player.Character.Task.ClearAll();
                        TaskSequence ts = new TaskSequence();
                        kenjiVehicle.Driver.Task.ClearAll();
                        kenjiVehicle.Driver.Task.PerformSequence(ts);
                        outputStream.Dispose();
                        channel.Dispose();
                        soundOut.Dispose();
                        outputStream = new WaveFileReader("scripts/duelraces/sounds/canyontrack1.wav");
                        channel = new WaveChannel32(outputStream);
                        soundOut = new DirectSoundOut();
                        soundOut.Init(channel);
                        soundOut.Play();

                        Blip kenjiBlip = kenjiVehicle.AddBlip();
                        kenjiBlip.Sprite = BlipSprite.Player;
                        kenjiBlip.Color = BlipColor.Yellow;
                        kenjiBlip.Name = "Kenji";
                        kenjiBlip.Scale = 0.7f;
                        isPlaying = true;
                    }
                }
                else if (round == 2)
                {
                    if(counter < 5f)
                    {
                        if(!r2)
                        {
                            Function.Call(Hash.TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE, playerVehicle.Driver, playerVehicle, World.GetWaypointPosition().X, World.GetWaypointPosition().Y, World.GetWaypointPosition().Z, 30f, (512), 1f);
                            Function.Call(Hash.SET_DRIVE_TASK_DRIVING_STYLE, kenjiVehicle.Driver, (512));
                            Function.Call(Hash.SET_DRIVER_ABILITY, kenjiVehicle.Driver, 1.0f);

                            Function.Call(Hash.TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE, playerVehicle.Driver, playerVehicle, World.GetWaypointPosition().X, World.GetWaypointPosition().Y, World.GetWaypointPosition().Z, 30f, (512), 1f);
                            Function.Call(Hash.SET_DRIVE_TASK_DRIVING_STYLE, playerVehicle.Driver, (512));
                            Function.Call(Hash.SET_DRIVER_ABILITY, playerVehicle.Driver, 1.0f);
                        }
                    }
                    else if (counter >= 5f)
                    {
                        Main.IsInCutscene = false;
                        kenjiVehicle.Driver.Task.ClearAll();
                        playerVehicle.Driver.Task.ClearAll();
                        isPlaying = true;
                    }
                }
                counter += Utils.deltaTime * 1f;
            }
        }

        bool isKenjiTasking = false;

        private void HandleRace()
        {
            if (isPlaying)
            {
                if (round == 1)
                {
                    if (!isKenjiTasking)
                    {
                        Vector3 pos = World.GetWaypointPosition();
                        Function.Call(Hash.TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE, kenjiVehicle.Driver, kenjiVehicle, pos.X, pos.Y, pos.Z, 100f, (512), 1f);
                        Function.Call(Hash.SET_DRIVE_TASK_DRIVING_STYLE, kenjiVehicle.Driver, (512));
                        Function.Call(Hash.SET_DRIVER_ABILITY, kenjiVehicle.Driver, 1.0f);
                        isKenjiTasking = true;
                    }
                    kenjiVehicle.CurrentBlip.Rotation = (int)kenjiVehicle.Heading;
                    float playerDistanceToKenji = Vector3.Distance(playerVehicle.Position, kenjiVehicle.Position);
                    if (playerDistanceToKenji < 10f)
                    {
                        //kenjiVehicle.ApplyForceRelative(new Vector3(0, 0.3f, 0));
                    }
                    if(Vector3.Distance(playerVehicle.Position, World.GetWaypointPosition()) < 70f)
                    {
                        HandleEndRound();
                    }
                    UI.ShowSubtitle("Points: " + points + " Until end: " + Vector3.Distance(playerVehicle.Position, World.GetWaypointPosition()));
                    points += (int) (1000 / playerDistanceToKenji);
                }
                else if (round == 2)
                {
                    if (!isKenjiTasking)
                    {
                        Function.Call(Hash.TASK_VEHICLE_CHASE, kenjiVehicle.Driver, playerVehicle);
                        Function.Call(Hash.SET_DRIVE_TASK_DRIVING_STYLE, kenjiVehicle.Driver, (512));
                        Function.Call(Hash.SET_DRIVER_ABILITY, kenjiVehicle.Driver, 1.0f);
                        isKenjiTasking = true;
                    }
                    float playerDistanceToKenji = Vector3.Distance(playerVehicle.Position, kenjiVehicle.Position);
                    if(Vector3.Distance(playerVehicle.Position, World.GetWaypointPosition()) < 70f || points <= 0f)
                    {
                        HandleEndRound();
                    }
                    points -= (int)(1000 * playerDistanceToKenji);
                }
            }
        }

        private void HandleEndRound()
        {
            if (round == 1)
            {
                kenjiVehicle.FreezePosition = true;
                playerVehicle.FreezePosition = true;
                UI.ShowSubtitle("Ended with " + points.ToString() + " points!");
                GoToRound2();
            }
        }

        private void GoToRound2()
        {
            kenjiVehicle.FreezePosition = false;
            playerVehicle.FreezePosition = false;
            kenjiVehicle.Position = new Vector3(-4860.083f, -941.979f, 483.864f);
            kenjiVehicle.Heading = 282.526f;
            playerVehicle.Position = new Vector3(-4787.301f, -926.747f, 475.128f);
            playerVehicle.Heading = 275.091f;
            counter = 0;
            round = 2;
            isPlaying = false;
            isKenjiTasking = false;
            Worldf.SetWaypoint(-4718.075f, 536.934f);
            Main.IsInCutscene = true;
        }
    }
}
