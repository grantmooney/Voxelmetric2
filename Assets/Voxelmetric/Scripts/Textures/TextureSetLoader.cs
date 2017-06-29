using UnityEngine;
using System.Collections.Generic;

public class TextureSetLoader: MonoBehaviour
{
    public string pathToTextureResources = "textures";
    public Material material;
    [HideInInspector]
    public Texture2D tilesheet;

    protected Dictionary<string, TextureSet> textureSets = new Dictionary<string, TextureSet>();

    public virtual void LoadTextures(Voxelmetric vm)
    {
        var resourceTextures = Resources.LoadAll<Texture2D>(pathToTextureResources);
        var packedTextures = new Texture2D(64, 64) { filterMode = FilterMode.Point };
        var rects = packedTextures.PackTextures(resourceTextures, 0, 8192, false);

        for (int i = 0; i < resourceTextures.Length; i++)
        {
            var textureName = resourceTextures[i].name;
            var nameLength = textureName.Length;
            if (!string.IsNullOrEmpty(textureName))
            {
                var rootTextureName = textureName;
                var openIndex = textureName.IndexOf('[');
                if (openIndex > 0 && openIndex < textureName.Length - 2 && textureName.IndexOf(']') == textureName.Length - 1)
                {
                    var dir = textureName.Substring(openIndex + 1, textureName.Length - (openIndex + 2));
                    rootTextureName = textureName.Substring(0, openIndex);
                    // Debug.Log(openIndex + ", " + dir + ", " + rootTextureName);
                }
                TextureSet tex;
                if (textureSets.ContainsKey(rootTextureName))
                {
                    tex = textureSets[rootTextureName];
                    tex.AddTexture(rects[i]);
                }
                else
                {
                    tex = new TextureSet(rootTextureName);
                    tex.AddTexture(rects[i]);
                    AddTexture(tex);
                }
            }
        }

        tilesheet = packedTextures;
        material.mainTexture = packedTextures;

        Debug.Log(string.Format("Voxelmetric Texture Set Loader loaded {0} textures into {1} texture sets.", rects.Length, textureSets.Keys.Count));
    }

    public virtual void AddTexture(TextureSet textureSet)
    {
        if (textureSets.ContainsKey(textureSet.name))
        {
            Debug.LogError("TEXTURE ALREADY EXISTS WITH NAME: "+textureSet.name);
        }
        else
        {
            textureSets.Add(textureSet.name, textureSet);
        }
    }

    public virtual TextureSet GetByName(string name)
    {
        if (!textureSets.ContainsKey(name))
        {
            Debug.LogError("There is no loaded texture by the name " + name);
            return null;
        }

        return textureSets[name];
    }
}
