using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SimplexNoise;

namespace Assets
{
    /// <summary>
    /// Represents a region of terrain, so we don't load too much at once.
    /// </summary>
    internal class ChunkData : MonoBehaviour
    {
        Vector2 location;
        SettingsData data;

        Block[,] blocks;

        public Vector2 GlobalLoc { get => GetGlobalLoc(); }

        private Vector2 GetGlobalLoc()
        {
            try
            {
                return location * data.chunkSize;
            }
            catch (NullReferenceException e)
            {
                Debug.LogException(e);
                return Vector2.zero;                
            }
        }

        public void Init(SettingsData data, Vector2 location)
        {
            this.location = location;
            this.data = data;

            Generate(GlobalLoc);
        }

        void Generate(Vector2 offset)
        {
            // do proper octave generation later.
            float[,] noiseValues = Noise.Calc2D(data.chunkSize, data.chunkSize, data.noiseScaleXY);
            blocks = new Block[data.chunkSize, data.chunkSize];

            for (int x = 0; x < noiseValues.GetLength(0); x++)
            {
                for (int z = 0; z < noiseValues.GetLength(1); z++)
                {
                    Block b = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<Block>();
                    b.Type = BlockType.Grass;
                    b.transform.position = new(x, (int)(noiseValues[x, z] * data.noiseScaleZ / SettingsData.noiseVerticalDivisor), z);
                    b.transform.parent = transform;
                }
            }
        }

        private void OnDestroy()
        {

        }
    }
}
