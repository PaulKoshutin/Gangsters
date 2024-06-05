using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

[Serializable]
public class Char : OrgComponent
{
    public string type;
    public string name;
    public Sprite image;
    public District district;
    public Org org;

    public int mental;
    public int social;
    public int physical;

    public int hiring_price;

    public int wealth;

    public Char() { }
    public void SetData(string type, District district, Org org, string name, Sprite image, int mental, int social, int physical, int hiring_price, int wealth)
    {
        this.type = type;
        this.district = district;
        this.org = org;
        this.name = name;
        this.image = image;
        this.mental = mental;
        this.social = social;
        this.physical = physical;

        this.hiring_price = hiring_price;
        this.wealth = wealth;
    }


    public override void Add(OrgComponent comp)
    {
        throw new NotImplementedException();
    }

    public override void Remove(OrgComponent comp)
    {
        throw new NotImplementedException();
    }
    public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
    {
        Texture2D SpriteTexture = LoadTexture(FilePath);
        //Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), 16f, 0, SpriteMeshType.FullRect);

        return NewSprite;
    }
    public Texture2D LoadTexture(string FilePath)
    {
        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);
            if (Tex2D.LoadImage(FileData))
                return Tex2D;
        }
        return null;
    }
    public void OnAfterDeserialize()
    {
        this.LoadNewSprite("Images/" + name + ".png");
    }
}