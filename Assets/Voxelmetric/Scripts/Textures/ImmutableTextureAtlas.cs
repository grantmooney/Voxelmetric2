﻿using UnityEngine;
using System.Collections.Generic;
using System;

public class ImmutableTextureAtlas : TextureAtlas
{
    readonly Dictionary<string, Rect> textureRects = new Dictionary<string, Rect>();

    public ImmutableTextureAtlas(Texture2D[] textures, int maximumSize)
    {
        texture = new Texture2D(64, 64) { filterMode = FilterMode.Point };
        var rects = texture.PackTextures(textures, 0, maximumSize, false);
        for (int i = 0; i < textures.Length; i++)
        {
            textureRects[textures[i].name] = (rects[i]);
        }
    }

    public override List<String> GetTextureNames()
    {
        return new List<String>(textureRects.Keys);
    }

    public override Rect GetTextureRect(string name)
    {
        if (!textureRects.ContainsKey(name))
        {
            Debug.LogError("There is no loaded texture by the name " + name);
            return new Rect();
        }

        return textureRects[name];
    }
}
