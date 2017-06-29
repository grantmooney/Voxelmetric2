using UnityEngine;
using System.Collections.Generic;

public class TextureSet
{
    public string name;
    protected List<Rect> textures = new List<Rect>();

    public TextureSet(string name)
    {
        this.name = name;
    }

    public virtual void AddTexture(Rect texture)
    {
        textures.Add(texture);
    }

    public virtual Rect GetTexture(Chunk chunk, Pos blockPos, Direction direction)
    {
        if (textures.Count == 0)
        {
            Debug.LogError("Block texture object is empty but something is requesting a texture!");
            return new Rect();
        }

        if (textures.Count == 1 || direction == Direction.up)
        {
            return textures[0];
        }

        var random = new System.Random(blockPos.GetHashCode());
        var randomNumber = random.Next(1, textures.Count);
        return textures[randomNumber];
    }
}
