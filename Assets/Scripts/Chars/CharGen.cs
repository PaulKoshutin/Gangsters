using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Debug = UnityEngine.Debug;

public class CharGen : MonoBehaviour
{
    private static readonly CharGen instance = new CharGen();

    public static CharGen Instance
    {
        get { return instance; }
    }

    public int x;
    public string name;

    public Char Generate()
    {
        return null;
    }


    private async void Cmd()
    {
        Task task = new Task(StartWorking);
        task.Start();
        Debug.Log("My exe file is running right now");
        await task;
    }

    public void StartWorking()
    {
        Process p = new Process();
        p.StartInfo = new ProcessStartInfo("python.exe", "t2i.py poor African-American male gangster red")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        p.Start();

        string name = p.StandardOutput.ReadLine();
        string output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();

        Debug.Log(name);
        Debug.Log(output);

        this.name = name;
        
        
    }

    private void Awake()
    {
        x = 1;
    }

    private void Update()
    {
        
        if (x == 1) { Cmd(); }
        x = 0;
        if (name != "")
        {
            GameObject.Find("Icon").GetComponent<Image>().sprite = LoadNewSprite("Images/" + name + ".png");
            name = "";
            x = 1;
        }
            
    }

    public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
    {

        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

        Texture2D SpriteTexture = LoadTexture(FilePath);
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);

        return NewSprite;
    }
    public Texture2D LoadTexture(string FilePath)
    {

        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails

        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }
}
