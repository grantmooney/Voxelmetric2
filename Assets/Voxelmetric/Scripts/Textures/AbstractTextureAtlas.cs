using UnityEngine;
using System.Collections.Generic;

public abstract class TextureAtlas
{
    protected Texture2D texture;

    public int Width
    {
        get
        {
            return texture.width;
        }
    }

    public int Height
    {
        get
        {
            return texture.height;
        }
    }

    public static explicit operator Texture2D(TextureAtlas d)
    {
        return d.texture;
    }

    public abstract Rect GetTextureRect(string name);

    public abstract List<string> GetTextureNames();
}
