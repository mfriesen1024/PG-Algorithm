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

        Block[,,] blocks;

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
            blocks = new Block[data.chunkSize, data.chunkSize*2, data.chunkSize];

            // Set an array of blocktypes so we can ensure culling works as intended.
            BlockType[,,] blockTypes = new BlockType[data.chunkSize,data.chunkSize*2,data.chunkSize];

            // Fill each vertical "stack"
            for (int x = 0; x < noiseValues.GetLength(0); x++)
            {
                for (int z = 0; z < noiseValues.GetLength(1); z++)
                {
                    // our top block should never be fully surrounded, so always instantiate a box for it.
                    SetBlock( x, (int)noiseValues[x, z], z, BlockType.Grass);
                }
            }
        }

        private void SetBlock( int x, int y, int z, BlockType type)
        {
            Block block = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<Block>();
            block.Type = type;
            block.transform.position = new(x, y * data.noiseScaleZ / SettingsData.noiseVerticalDivisor, z);
            block.transform.parent = transform;
            blocks[x, y, z] = block;
        }

        private void OnDestroy()
        {

        }
    }
}
