using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    SettingsData data;
    // Start is called before the first frame update
    void Start()
    {
        Load();
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
