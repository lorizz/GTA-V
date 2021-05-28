using GTA;
using GTA.Math;
using IrrKlang;

namespace IrrKlangPreloadedSounds
{
    class PreloadedSound
    {
        public string FilePath;
        public ISound Sound;
        public ISoundEffectControl SoundEffect;

        public float MaximumDistance = 20f;
        public float MinimumDistance = 1f;

        public PreloadedSound(/*ISoundEngine engine,*/ string filepath)
        {
            //Source = engine.AddSoundSourceFromFile(filepath, StreamMode.AutoDetect, true);
            FilePath = filepath;
        }

        public void Play3DSound(/*ISoundEngine engine,*/ Vector3 sourcePosition, bool playLooped, bool allowMultipleInstances = false, bool allowSoundEffects = true)
        {
            if (allowMultipleInstances || (!allowMultipleInstances && (Sound == null || Sound != null && Sound.Finished)))
            {
                Vector3D sourcePos = SoundHelperIK.Vector3ToVector3D(GameplayCamera.GetOffsetFromWorldCoords(sourcePosition));

                //Sound = engine.Play3D(Source, sourcePos.X, sourcePos.Y, sourcePos.Z, playLooped, false, false);
                Sound = SoundEngine.Play3D(FilePath, sourcePos.X, sourcePos.Y, sourcePos.Z, playLooped, false, StreamMode.AutoDetect, allowSoundEffects);
                if (allowSoundEffects)
                {
                    SoundEffect = Sound.SoundEffectControl;
                }
            }
        }

        public void StopSound()
        {
            if (Sound == null || Sound.Finished) return;
            Sound.Stop();
        }

        public bool IsPlaying()
        {
            return Sound != null && !Sound.Finished;
        }

        public void ProcessSound(Vector3 sourcePosition, float speed)
        {
            if (Sound != null && !Sound.Finished)
            {
                Sound.MaxDistance = MaximumDistance;
                Sound.MinDistance = MinimumDistance;
                Sound.PlaybackSpeed = speed;
                Vector3D sourcePos = SoundHelperIK.Vector3ToVector3D(GameplayCamera.GetOffsetFromWorldCoords(sourcePosition));
                Sound.Position = sourcePos;
            }
        }

        public void SetDistances(float max, float min)
        {
            MaximumDistance = max;
            MinimumDistance = min;
        }

        public void Dispose()
        {
            Sound.Stop();
            Sound.Dispose();
            //Source.Dispose();
        }

        public static ISoundEngine SoundEngine = new ISoundEngine();

        public static void ManageSoundEngine()
        {
            SoundEngine.SetListenerPosition(new Vector3D(0, 0, 0), new Vector3D(0, 0, 1), new Vector3D(0, 0, 0), new Vector3D(0, 1, 0));
            SoundEngine.Update();
        }
    }

    static class SoundHelperIK
    {
        public static Vector3D Vector3ToVector3D(Vector3 vec)
        {
            return new Vector3D(vec.X, vec.Z, vec.Y);
        }
    }
}