using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    [Serializable]
    public class SettingsData
    {
        public float noiseScaleXY = 1;
        public float noiseScaleZ = 8;
        public int chunkLoadDist = 3; // How many chunks from the player
        const float noiseVerticalDivisor = 256; // Divide vertical height of noise by this.

        public SettingsData()
        {
        }

        public SettingsData(string[] fileData)
        {
            noiseScaleXY = float.Parse(fileData[0]);
            noiseScaleZ = float.Parse(fileData[1]);
            chunkLoadDist = int.Parse(fileData[2]);
        }

        internal string[] ToLines()
        {
            string[] data = {
            noiseScaleXY.ToString(),
            noiseScaleZ.ToString(),
            chunkLoadDist.ToString()
            };
            return data;
        }
    }
}
