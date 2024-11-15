﻿using SimplexNoise;
using System;
using System.Linq;
using UnityEngine;

namespace Assets
{
    /// <summary>
    /// Represents a region of terrain, so we don't load too much at once.
    /// </summary>
    internal class ChunkData : MonoBehaviour
    {
        static BlockType[] renderIfAdjacent { get; } = { BlockType.Air, BlockType.Water };
        public Vector2 Location { get; private set; }
        SettingsData data;

        float[,] noiseValues;
        Block[,,] blocks;
        BlockType[,,] blockTypes;

        // Use this to generate by frame.
        int x, y, z;
        private bool ready = false;
        int framesToGenerate = 0;

        public Vector2 GlobalLoc { get => GetGlobalLoc(); }

        private Vector2 GetGlobalLoc()
        {
            try
            {
                return Location * data.chunkSize;
            }
            catch (NullReferenceException e)
            {
                Debug.LogException(e);
                return Vector2.zero;
            }
        }

        public void Init(SettingsData data, Vector2 location)
        {
            this.Location = location;
            this.data = data;

            Generate();
        }

        void Generate()
        {
            x = 0; y = 0; z = 0;

            // do proper octave generation later.
            noiseValues = new float[data.chunkSize, data.chunkSize];
            try { Noise.Seed = data.seed1; } catch { }
            for (int x = 0; x < noiseValues.GetLength(0); x++)
            {
                for (int z = 0; z < noiseValues.GetLength(1); z++)
                {
                    noiseValues[x, z] = Noise.CalcPixel2D((int)(x + GlobalLoc.x), (int)(z + GlobalLoc.y), data.noiseScaleXY);
                }
            }
            blocks = new Block[data.chunkSize, data.chunkSize * 2, data.chunkSize];

            // Set an array of blocktypes so we can ensure culling works as intended.
            blockTypes = new BlockType[data.chunkSize, data.chunkSize * 2, data.chunkSize];

            // Fill each vertical "stack"
            for (int x = 0; x < noiseValues.GetLength(0); x++)
            {
                for (int z = 0; z < noiseValues.GetLength(1); z++)
                {
                    int surfaceHeight = (int)(noiseValues[x, z] * data.noiseScaleZ / SettingsData.noiseVerticalDivisor);

                    // Fill everything below our surface height with dirt.
                    for (int y = 0; y < surfaceHeight; y++)
                    {
                        blockTypes[x, y, z] = BlockType.Dirt;
                    }
                    // Set our surface block to grass.
                    blockTypes[x, surfaceHeight, z] = BlockType.Grass;
                    // Fill everything above with air.
                    for (int y = surfaceHeight + 1; y < blockTypes.GetLength(1); y++)
                    {
                        blockTypes[x, y, z] = BlockType.Air;
                    }
                    // If water level is above surface, overwrite whatever was there and fill water up to surface.
                    if (data.waterLvl > surfaceHeight)
                    {
                        for(int y = surfaceHeight + 1; y < data.waterLvl; y++)
                        {
                            blockTypes[x, y, z] = BlockType.Water;
                        }
                    }
                }
            }

            ready = true;
        }

        private void Update()
        {
            int count = 0;

            if (ready)
            {
                GenerateStuff();
            }

            void GenerateStuff()
            {
                bool b = false;
                // Now, check for adjacent blocks other than air. If we get 6 directly adjacent, or the block is air, don't render the block. Otherwise, place it.
                while (x < blockTypes.GetLength(2)&&y<blockTypes.GetLength(1)&&z<blockTypes.GetLength(0))
                {
                    if (b) { return; }

                    // bools for negative/positive in each direction.
                    bool xn, xp, yn, yp, zn, zp;
                    // Check if there's air, and if index out of range, set false.
                    try { xn = RenderCheck(x-1,y,z); } catch { xn = false; }
                    try { xp = RenderCheck(x + 1, y, z); } catch { xp = false; }
                    try { yn = RenderCheck(x, y - 1, z); } catch { yn = false; }
                    try { yp = RenderCheck(x, y + 1, z); } catch { yp = false; }
                    try { zn = RenderCheck(x, y, z - 1); } catch { zn = false; }
                    try { zp = RenderCheck(x, y, z + 1); } catch { zp = false; }

                    // Now compare everything.
                    bool adjacencyCheck = !(xn && xp && yn && yp && zn && zp);
                    BlockType blockType = blockTypes[x, y, z];
                    if (blockType != BlockType.Air && adjacencyCheck)
                    {
                        PlaceBlock(x, y, z, blockTypes[x, y, z]); count++; if (count >= data.blocksPerTick) { b = true; }
                    }
                    UpdateXYZ();
                }
                Debug.Log($"4 {x},{y},{z}");
                if (b) return;
                Debug.Log($"Finished generation in {framesToGenerate} frames."); ready = false;
            }

            void UpdateXYZ()
            {
                z++;
                if (z >= blockTypes.GetLength(2)) { z = 0; x++; }
                if (x >= blockTypes.GetLength(0)) { x = 0; y++; }
                Console.WriteLine("" + x + y + z);
            }
        }

        private bool RenderCheck(int x,int y,int z)
        {
            // Checks if the force render list contains the blocktype at the given coordinates.
            var typeList = renderIfAdjacent.ToList();
            return !typeList.Contains(blockTypes[x, y, z]);
        }

        private void PlaceBlock(int x, int y, int z, BlockType type)
        {
            Block block = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<Block>();
            block.Type = type;
            int offsetX = (int)GlobalLoc.x; int offsetZ = (int)GlobalLoc.y;
            block.transform.position = new(x + offsetX, y, z + offsetZ);
            block.transform.parent = transform;
            blocks[x, y, z] = block;
        }

        private void OnDestroy()
        {
            foreach (Block block in blocks)
            {
                if (block != null) { Destroy(block.gameObject); }
            }
            Destroy(gameObject);
        }
    }
}
