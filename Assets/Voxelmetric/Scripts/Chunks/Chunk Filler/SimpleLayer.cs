public class SimpleLayer : TerrainLayer {
    /// <summary>
    /// Whether the layer is absolute or added on to the previous layer
    /// </summary>
    public bool absolute;
    public int baseHeight;
    public float frequency;
    public float amplitude;
    public int offset;

    public int blockId;

    public override int ApplyLayerCol(int x, int z, int head)
    {
        var add = Noise(x, z, offset, frequency, amplitude) + baseHeight;

        var top = add;
        if (!absolute) top += head;
        if (absolute && head > add) return head;

        Utils.ProfileCall(() =>
        {
            filler.SetBlocks(x, z, head, top, new Block(blockId));
        }, "Set Blocks");
        return top;
    }
}
