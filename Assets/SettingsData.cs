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
        const float noiseVerticalDivisor = 256; // Divide vertical height of noise by this.

        public SettingsData()
        {
        }

        public SettingsData(string[] fileData)
        {
        }

        internal string[] ToLines()
        {
            throw new NotImplementedException();
        }
    }
}
