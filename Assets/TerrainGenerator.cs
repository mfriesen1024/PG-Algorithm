using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private SettingsData data;
    public SettingsData Data { get => data; private set => data = value; }

    List<ChunkData> chunks = new();

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
        if (File.Exists(fName)) { data = new(File.ReadAllLines(fName)); }
        else { data = new(); File.WriteAllLines(fName, data.ToLines()); }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
