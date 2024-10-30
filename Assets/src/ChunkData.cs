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
                    int surfaceHeight = (int)(noiseValues[x, z] * data.noiseScaleZ / SettingsData.noiseVerticalDivisor);

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

            // Now, check for adjacent blocks other than air. If we get 6 directly adjacent, or the block is air, don't render the block. Otherwise, place it.
            for (int x = 0; x < blockTypes.GetLength(0); x++)
            {
                for(int y = 0; y < blockTypes.GetLength(1); y++)
                {
                    for (int z = 0; z < blockTypes.GetLength(2); z++)
                    {
                        // bools for negative/positive in each direction.
                        bool nx, px, ny, py, nz, pz;
                        // Check if there's air, and if index out of range, set false.
                        try { nx = blockTypes[x - 1, y, z] != BlockType.Air; } catch { nx = false; }
                        try { px = blockTypes[x - 1, y, z] != BlockType.Air; } catch { px = false; }
                        try { ny = blockTypes[x - 1, y, z] != BlockType.Air; } catch { ny = false; }
                        try { py = blockTypes[x - 1, y, z] != BlockType.Air; } catch { py = false; }
                        try { nz = blockTypes[x - 1, y, z] != BlockType.Air; } catch { nz = false; }
                        try { pz = blockTypes[x - 1, y, z] != BlockType.Air; } catch { pz = false; }

                        // Now throw everything in a 7 way and gate, and if successful, render the block.
                        if (blockTypes[x,y,z] != BlockType.Air && (nx&&px&&ny&&py&&nz&&pz))
                        {
                            SetBlock(x,y,z, blockTypes[x,y,z]);    
                        }
                    }
                }
            }
        }

        private void SetBlock( int x, int y, int z, BlockType type)
        {
            Block block = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<Block>();
            block.Type = type;
            int offsetX = (int)GlobalLoc.x; int offsetZ = (int)GlobalLoc.y;
            block.transform.position = new(x+offsetX, y, z+offsetZ);
            block.transform.parent = transform;
            blocks[x, y, z] = block;
        }

        private void OnDestroy()
        {

        }
    }
}
