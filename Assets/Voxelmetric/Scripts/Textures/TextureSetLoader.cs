using UnityEngine;
using System.Collections.Generic;

public class TextureSetLoader: MonoBehaviour
{
    public string pathToTextureResources = "textures";
    public Material material;
    [HideInInspector]
    public TextureAtlas mainAtlas;

    protected Dictionary<string, TextureSet> textureSets = new Dictionary<string, TextureSet>();

    public virtual void LoadTextures(Voxelmetric vm)
    {
        var resourceTextures = Resources.LoadAll<Texture2D>(pathToTextureResources);
        var atlas = new ImmutableTextureAtlas(resourceTextures, 8192);

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
                    // TODO: Direction-based texture naming, like this:
                    // var dir = textureName.Substring(openIndex + 1, textureName.Length - (openIndex + 2));
                    rootTextureName = textureName.Substring(0, openIndex);
                }
                TextureSet tex;
                if (textureSets.ContainsKey(rootTextureName))
                {
                    tex = textureSets[rootTextureName];
                    tex.AddTexture(atlas.GetTextureRect(rootTextureName)); ;
                }
                else
                {
                    tex = new TextureSet(rootTextureName);
                    tex.AddTexture(atlas.GetTextureRect(rootTextureName));
                    AddTexture(tex);
                }
            }
        }


        mainAtlas = atlas;
        material.mainTexture = (Texture2D)atlas;
        #if UNITY_EDITOR
            Debug.Log(string.Format("Voxelmetric Texture Set Loader loaded {0} textures into {1} texture sets.", resourceTextures.Length, textureSets.Keys.Count));
        #endif
    }

    public virtual void AddTexture(TextureSet textureSet)
    {
        if (textureSets.ContainsKey(textureSet.name))
        {
            Debug.LogError("TEXTURE ALREADY EXISTS WITH NAME: " + textureSet.name);
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
            #if UNITY_EDITOR
                Debug.LogError("There is no loaded texture by the name " + name);
            #endif
            return null;
        }

        return textureSets[name];
    }
}
