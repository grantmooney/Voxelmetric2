using UnityEngine;
using SimplexNoise;

public class TerrainLayer {
    protected ChunkFiller filler;
    protected Noise noise;

    public virtual void VmStart(ChunkFiller chunkFiller)
    {
        filler = chunkFiller;
        noise = chunkFiller.noise;
    }

    public virtual int ApplyLayerCol(int x, int z, int head)
    {
        return head;
    }

    public int Noise(int x, int z, int offset, float frequency, float amplitude)
    {
        return Mathf.RoundToInt((noise.Generate(x * frequency, offset, z * frequency) * amplitude) + offset);
    }
}
