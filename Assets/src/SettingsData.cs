using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class SettingsData
    {
        public float noiseScaleXY = 1;
        public float noiseScaleZ = 8;
        public int chunkLoadDist = 3; // How many chunks from the player
        public int chunkSize=12;
        public int seed1;
        public const float noiseVerticalDivisor = 256; // Divide vertical height of noise by this.

        public int blocksPerTick=8;

        public SettingsData()
        {
        }

        public SettingsData(string[] fileData)
        {
            noiseScaleXY = float.Parse(fileData[0]);
            noiseScaleZ = float.Parse(fileData[1]);
            chunkLoadDist = int.Parse(fileData[2]);
            chunkSize = int.Parse(fileData[3]);
            seed1 = int.Parse(fileData[4]);
            blocksPerTick = int.Parse(fileData[6]);
        }

        internal string[] ToLines()
        {
            string[] data = {
            noiseScaleXY.ToString(),
            noiseScaleZ.ToString(),
            chunkLoadDist.ToString(),
            chunkSize.ToString(),
            seed1.ToString(),
            blocksPerTick.ToString()
            };
            return data;
        }
    }
}
