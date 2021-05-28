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

        public Main()
        {
            sound = new PreloadedSound(@"scripts\sounds\car_whine.wav");
            reverseSound = new PreloadedSound(@"scripts\sounds\car_whine_reverse.wav");
            started = false;

            Tick += OnTick;
            Aborted += OnAbort;
            
               
        }

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
            if (!Game.IsPaused  || !Game.IsLoading)
            {
                if (vehicle.IsToggleModOn(VehicleToggleMod.Turbo))
                {
                    int engineBone = Function.Call<int>(Hash.GET_ENTITY_BONE_INDEX_BY_NAME, vehicle, "engine");
                    Vector3 engineBonePos = (Function.Call<Vector3>(Hash.GET_WORLD_POSITION_OF_ENTITY_BONE, vehicle, engineBone) + new Vector3(-2.0f * vehicle.ForwardVector.X, -2.0f * vehicle.ForwardVector.Y, 0.0f));
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
                        if (vehicle.Acceleration == 0 && vehicle.Speed > 0.1f)
                        {
                            if (Game.IsControlPressed(2, GTA.Control.VehicleAccelerate))
                            {
                                if (sound.Sound.Volume > 0f)
                                    sound.Sound.Volume -= 0.02f;

                                if (reverseSound.Sound.Volume > 0f)
                                    reverseSound.Sound.Volume -= 0.02f;
                            }
                        }
                        else
                        {
                            if (sound.Sound.Volume < 0.8f)
                                sound.Sound.Volume += 0.02f;

                            if (vehicle.Speed > 0.1f)
                            { 
                                if (vehicle.CurrentGear > 0)
                                {
                                    if (reverseSound.Sound.Volume < 0.7f)
                                        reverseSound.Sound.Volume += 0.02f;
                                }
                                else
                                {
                                    if (reverseSound.Sound.Volume < 0.9f)
                                        reverseSound.Sound.Volume += 0.02f;
                                }
                            }
                            else
                            {
                                reverseSound.Sound.Volume = 0;
                            }
                        }
                        if(vehicle.Speed > 0.1f)
                        {
                            if(!Game.IsControlPressed(2, GTA.Control.VehicleAccelerate))
                            {
                                if (sound.Sound.Volume > 0.4f)
                                    sound.Sound.Volume -= 0.02f;

                                if (reverseSound.Sound.Volume > 0.6f)
                                    reverseSound.Sound.Volume -= 0.02f;
                            }
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
                    sound.Sound.Volume -= 0.05f;
                if (reverseSound.Sound.Volume > 0f)
                    reverseSound.Sound.Volume -= 0.05f;
            }
            sound.StopSound();
            reverseSound.StopSound();
            started = false;
        }

        // X = Y^2
        float HandlePitch(float min, float max, Vehicle vehicle)
        {
            float speed = (vehicle.Speed * 3.6f) + min;
            float newPitch = speed * 0.05f; // / decreaser

            // Global check
            if (newPitch < min)
            {
                newPitch = min;
            }
            else if(newPitch > max)
            {
                newPitch = max;
            }

            if(speed > 10f && speed <= 30f)
            {
                newPitch = speed * 0.015f + 0.32f;
            }
            else if (speed > 30f && speed <= 80f)
            {
                newPitch = speed * 0.01f + 0.45f;
            }
            else if (speed > 30f && speed <= 140f)
            {
                newPitch = speed * 0.0033f + 1.00f;
            }
            else if (speed > 140f)
            {
                newPitch = speed * 0.003f + 1.01f;
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
                if (speed > 10f && speed <= 30f)
                {
                    newPitch = speed * 0.015f + 0.32f;
                }
                else if (speed > 30f && speed <= 80f)
                {
                    newPitch = speed * 0.01f + 0.45f;
                }
                else if (speed > 30f && speed <= 140f)
                {
                    newPitch = speed * 0.0033f + 1.00f;
                }
                else if (speed > 140f)
                {
                    newPitch = speed * 0.003f + 1.01f;
                }
            }

            return newPitch;
        }


        void OnAbort(object sender, EventArgs e)
        {
            PreloadedSound.SoundEngine.StopAllSounds();
            PreloadedSound.SoundEngine.Dispose();
        }
    }
}
