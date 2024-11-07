using Assets;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    private SettingsData data;
    public SettingsData Data { get => data; private set => data = value; }

    List<ChunkData> chunks = new();

    [SerializeField] CameraController camController;

    // Start is called before the first frame update
    void Start()
    {
        Load();
        Generate();
    }

    private void Generate()
    {
        // For now, just generate one chunk with coords 0,0.
        ChunkData cd = new GameObject().AddComponent<ChunkData>();
        cd.Init(data, new(0, 0));
        chunks.Add(cd);
    }

    private void Load()
    {
        string fName = "data\\settings.txt";
        string dirName = "data";
        Directory.CreateDirectory(dirName);
        if (File.Exists(fName))
        {
            try { data ??= new(File.ReadAllLines(fName)); }
            catch (IndexOutOfRangeException ignored) { backup(); }
        }
        else { backup(); }

        void backup()
        {
            data ??= new(); File.WriteAllLines(fName, data.ToLines());
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camPos = camController.transform.position;
        Vector2 camPos2D = new(camPos.x, camPos.z);

        // remove faraway chunks.
        Action deleteOld = () => { };
        foreach (var chunk in chunks)
        {
            if (Vector3.Distance(camPos2D, chunk.GlobalLoc) > (data.chunkLoadDist + 2) * data.chunkSize)
            {
                deleteOld += () => { Destroy(chunk); chunks.Remove(chunk); };
            }
        }
        deleteOld();
        deleteOld = null;

        // Make new positions, then spawn new chunks if we're missing any.
        int x = (int)camPos2D.x / data.chunkSize; int z = (int)camPos2D.y / data.chunkSize;
        for (int x2 = x - data.chunkLoadDist; x2 < x + data.chunkLoadDist + 1; x2++)
        {
            for (int z2 = z - data.chunkLoadDist; z2 < z + data.chunkLoadDist + 1; z2++)
            {
                Vector2 chunkPos = new Vector2(x2, z2), globalChunkPos = chunkPos * data.chunkSize;
                // if out of range, continue.
                if (Vector3.Distance(camPos2D, globalChunkPos) > data.chunkLoadDist * data.chunkSize) { continue; }

                // Check if we have the chunk already
                bool shouldMake = true;
                foreach (var chunk in chunks)
                {
                    if (Vector3.Distance(chunk.Location, chunkPos) < 0.1) { shouldMake = false; break; }
                }

                // Make the chunk if we don't have it.
                if (shouldMake)
                {
                    ChunkData chunk = new GameObject("Chunk").AddComponent<ChunkData>();
                    chunk.Init(data, chunkPos);
                    chunks.Add(chunk);
                    Debug.Log(null);
                }
            }
        }
    }
}
