using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Math;
using GTA.Native;
using IrrKlang;
using IrrKlangPreloadedSounds;

namespace M3GTRWhine
{
    public class Whine
    {
        int engineBone;
        Vector3 engineBonePos;
        ISoundEngine engine;
        ISound whine;

        public Whine(Vehicle vehicle)
        {
            int engineBone = Function.Call<int>(Hash.GET_ENTITY_BONE_INDEX_BY_NAME, vehicle, "engine");
            Vector3 engineBonePos = Function.Call<Vector3>(Hash.GET_WORLD_POSITION_OF_ENTITY_BONE, vehicle, engineBone);

            ISoundEngine engine = new ISoundEngine();
            ISound whine = engine.Play3D("@sounds/car_whine.wav", 0, 0, 0, true);
            whine.MinDistance = 5.0f;
            engine.SetListenerPosition(0, 0, 0, 0, 0, 1);
        }

        public void PlayWhine()
        {                  
            whine.Position = new Vector3D(engineBonePos.X, engineBonePos.Y, engineBonePos.Z);
        }
    }
}
