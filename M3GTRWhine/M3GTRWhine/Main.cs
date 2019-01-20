using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using IrrKlang;
using IrrKlangPreloadedSounds;

namespace M3GTRWhine
{
    public class Main : Script
    {
        PreloadedSound sound;
        PreloadedSound reverseSound;
        bool started;

        Vehicle[] vehicles;
        IList<Vehicle> rx7 = new List<Vehicle>();
        IList<PreloadedSound[]> sounds = new List<PreloadedSound[]>();

        public Main()
        {
            sound = new PreloadedSound(@"scripts\sounds\car_whine.wav");
            reverseSound = new PreloadedSound(@"scripts\sounds\car_whine_reverse.wav");
            started = false;

            Tick += OnTick;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
            Aborted += OnAbort;
               
        }

        /*void OnTick(object sender, EventArgs e)
        {
            vehicles = World.GetAllVehicles();
            for(int i = 0; i < vehicles.Length; i++)
            {
                if (vehicles[i].Model == "RX7TUNABLE")
                {
                    if (!rx7.Contains(vehicles[i]))
                    {
                        rx7.Add(vehicles[i]);
                        sound.Add(null);
                    }
                }
            }
            if (rx7.Count > 0)
            {
                for (int i = 0; i < rx7.Count; i++)
                {
                    if (sound[i] == null)
                    {
                        PreloadedSound whine = new PreloadedSound(@"scripts\sounds\car_whine.wav");
                        PreloadedSound reverseWhine = new PreloadedSound(@"scripts\sounds\car_whine_reverse.wav");
                        int engineBone = Function.Call<int>(Hash.GET_ENTITY_BONE_INDEX_BY_NAME, rx7[i], "engine");
                        Vector3 engineBonePos = Function.Call<Vector3>(Hash.GET_WORLD_POSITION_OF_ENTITY_BONE, rx7[i], engineBone);
                        whine.Play3DSound(engineBonePos, true);
                        reverseWhine.Play3DSound(engineBonePos, true);
                        float pitchValue = HandlePitchValue(rx7[i]);
                        bool isReverse = rx7[i].CurrentGear == 0;
                        if (!isReverse)
                        {
                            whine.SetDistances(10f, 1f);
                            whine.ProcessSound(engineBonePos, pitchValue);
                        }
                        else
                        {
                            reverseWhine.SetDistances(10f, 1f);
                            reverseWhine.ProcessSound(engineBonePos, pitchValue);
                        }
                        PreloadedSound.ManageSoundEngine();
                        PreloadedSound[] sounds = { whine, reverseWhine };
                        sound[i] = sounds;
                    }
                }
            }
        }*/

        void OnTick(object sender, EventArgs e)
        {
            if(Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Model.IsCar)
            {
                Vehicle vehicle = Game.Player.Character.CurrentVehicle;
                if(vehicle.Model == "M3E46")
                {
                    GenerateWhine(vehicle);
                }
                else
                {
                    StopAllSounds();
                }
            }
            else
            {
                StopAllSounds();

            }
        }

        void GenerateWhine(Vehicle vehicle)
        {
            if (!Game.IsPaused || !Game.IsLoading)
            {
                if (vehicle.IsToggleModOn(VehicleToggleMod.Turbo))
                {
                    int engineBone = Function.Call<int>(Hash.GET_ENTITY_BONE_INDEX_BY_NAME, vehicle, "engine");
                    Vector3 engineBonePos = Function.Call<Vector3>(Hash.GET_WORLD_POSITION_OF_ENTITY_BONE, vehicle, engineBone);
                    if (!started)
                    {
                        if (vehicle.EngineRunning == true)
                        {
                            sound.Play3DSound(engineBonePos, true);
                            reverseSound.Play3DSound(engineBonePos, true);
                            sound.Sound.Volume = 0f;
                            reverseSound.Sound.Volume = 0f;
                            started = true;
                        }
                    }
                    else
                    {
                        if (vehicle.EngineRunning == false)
                            StopAllSounds();
                        sound.SetDistances(0f, 1f);
                        reverseSound.SetDistances(0f, 1f);
                        if (vehicle.Acceleration == 0 && vehicle.Speed > 0.1f && Game.IsControlPressed(2, GTA.Control.VehicleAccelerate) && vehicle.CurrentGear != 0)
                        {
                            if (sound.Sound.Volume > 0f)
                                sound.Sound.Volume -= 0.1f;

                            if(reverseSound.Sound.Volume > 0f)
                                reverseSound.Sound.Volume -= 0.1f;
                        }
                        else
                        {
                            if (sound.Sound.Volume < 1f)
                                sound.Sound.Volume += 0.1f;

                            if (reverseSound.Sound.Volume < 1f)
                                reverseSound.Sound.Volume += 0.1f;
                        }
                        sound.ProcessSound(engineBonePos, HandlePitch(0.24f, 1.82f, vehicle));
                        reverseSound.ProcessSound(engineBonePos, HandleReversePitch(0.01f, 0.65f, vehicle));
                        PreloadedSound.ManageSoundEngine();
                    }
                }
                else
                {
                    StopAllSounds();
                }
            }
            else
            {
                StopAllSounds();
            }
        }

        void StopAllSounds()
        {
            while (sound.Sound.Volume > 0f && reverseSound.Sound.Volume > 0f)
            {
                if (sound.Sound.Volume > 0f)
                    sound.Sound.Volume -= 0.1f;
                if (reverseSound.Sound.Volume > 0f)
                    reverseSound.Sound.Volume -= 0.1f;
            }
            sound.StopSound();
            reverseSound.StopSound();
            started = false;
        }

        // X = Y^2
        float HandlePitch(float min, float max, Vehicle vehicle)
        {
            float currentGear = vehicle.CurrentGear;
            float speed = vehicle.Speed + min;
            float newPitch = (speed / 13f); // / decreaser

            // Global check
            if (newPitch < min)
            {
                newPitch = min;
            }
            else if(newPitch > max)
            {
                newPitch = max;
            }

            // Finetuning check
            if (speed > 15f && speed <= 30f)
            {
                newPitch = (speed / 40f) + 0.80f;
            }
            else if(speed > 30f && speed <= 40f)
            {
                newPitch = (speed / 75f) + 1.13f;
            }
            else if(speed > 40f)
            {
                newPitch = (speed / 100f) + 1.26f;
            }

            return newPitch;
        }

        float HandleReversePitch(float min, float max, Vehicle vehicle)
        {
            float currentGear = vehicle.CurrentGear;
            float speed = vehicle.Speed;
            float newPitch = speed / 20;

            if (currentGear == 0)
            {
                // Global check
                if (newPitch < min)
                {
                    newPitch = min;
                }
                else if (newPitch > max)
                {
                    newPitch = max;
                }
            }
            else
            {
                newPitch = min;
            }

            return newPitch;
        }

        /*void OnTick(object sender, EventArgs e)
        {
            Vehicle[] vehicles = World.GetAllVehicles();
            Vehicle veh = null;
            if (Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Model.IsCar)
            {
                for (int i = 0; i < vehicles.Length; i++)
                {
                    if (vehicles[i].Model == "RX7TUNABLE")
                    {
                        veh = vehicles[i];
                    }
                }
                //Vehicle vehicle = Game.Player.Character.CurrentVehicle;
                if (Function.Call<bool>(Hash._0x557E43C447E700A8, Game.GenerateHash("carwhinereload")))
                {
                    PreloadedSound.SoundEngine.StopAllSounds();
                    PreloadedSound.SoundEngine.Dispose();
                    PreloadedSound.SoundEngine = new ISoundEngine();
                    sound = new PreloadedSound(@"scripts\sounds\car_whine.wav");
                    reverseSound = new PreloadedSound(@"scripts\sounds\car_whine_reverse.wav");

                    UI.Notify("Car Whine ~s~Reloaded");
                    Wait(150);
                }

                if (veh != null)
                {
                    int engineBone = Function.Call<int>(Hash.GET_ENTITY_BONE_INDEX_BY_NAME, veh, "engine");
                    Vector3 engineBonePos = Function.Call<Vector3>(Hash.GET_WORLD_POSITION_OF_ENTITY_BONE, veh, engineBone);
                    float pitchValue = HandlePitchValue(veh);
                    bool isReverse = veh.CurrentGear == 0;
                    float clutchValue = veh.Acceleration;
                    if (!started)
                    {
                        sound.Play3DSound(engineBonePos, true);
                        reverseSound.Play3DSound(engineBonePos, true);
                        started = true;
                    }
                    if(clutchValue == 0 && Game.IsControlPressed(2, GTA.Control.VehicleAccelerate))
                    {
                        if(sound.Sound.Volume > 0f)
                            sound.Sound.Volume = 0f;
                    }
                    else
                    {
                        if(sound.Sound.Volume < 2f)
                            sound.Sound.Volume += 0.2f;
                    }
                    UI.ShowSubtitle(sound.Sound.Volume.ToString());
                    if (!isReverse)
                    {
                        if (reverseSound.IsPlaying())
                        {
                            reverseSound.StopSound();
                            sound.Play3DSound(engineBonePos, true);
                        }
                        sound.SetDistances(2f, 1f);
                        sound.ProcessSound(engineBonePos, pitchValue);
                    }
                    else
                    {
                        if (sound.IsPlaying())
                        {
                            sound.StopSound();
                            reverseSound.Play3DSound(engineBonePos, true);
                        }
                        reverseSound.SetDistances(2f, 1f);
                        reverseSound.ProcessSound(engineBonePos, pitchValue);
                    }
                    PreloadedSound.ManageSoundEngine();
                }
            }
        }*/

        void OnKeyDown(object sender, KeyEventArgs e)
        {

        }

        void OnKeyUp(object sender, KeyEventArgs e)
        {

        }

        void OnAbort(object sender, EventArgs e)
        {
            PreloadedSound.SoundEngine.StopAllSounds();
            PreloadedSound.SoundEngine.Dispose();
        }

        float HandlePitchValue(Vehicle vehicle)
        {
            float pitchValue = (vehicle.Speed / (20f * vehicle.CurrentGear - 0.2f)) + (vehicle.CurrentGear * 0.2f);
            if(pitchValue <= 0.3f)
            {
                pitchValue = 0.3f;
            }

            return pitchValue;
        }
    }
}
