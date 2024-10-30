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

            Generate();
        }

        void Generate()
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
                    int surfaceHeight = (int)noiseValues[x, z];

                    // Fill everything below our surface height with dirt.
                    for (int y = 0; y < surfaceHeight - 1; y++)
                    {
                        blockTypes[x, y, z] = BlockType.Dirt;
                    }
                    // Set our surface block to grass.
                    blockTypes[x,surfaceHeight-1,z]= BlockType.Grass;
                    // Fill everything above with air.
                    for(int y = surfaceHeight; y < blockTypes.GetLength(1); y++)
                    {
                        blockTypes[x, y, z] = BlockType.Air;
                    }
                }
            }
        }

        private void SetBlock( int x, int y, int z, BlockType type)
        {
            Block block = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<Block>();
            block.Type = type;
            int offsetX = (int)GlobalLoc.x; int offsetZ = (int)GlobalLoc.y;
            block.transform.position = new(x+offsetX, y * data.noiseScaleZ / SettingsData.noiseVerticalDivisor, z+offsetZ);
            block.transform.parent = transform;
            blocks[x, y, z] = block;
        }

        private void OnDestroy()
        {

        }
    }
}
