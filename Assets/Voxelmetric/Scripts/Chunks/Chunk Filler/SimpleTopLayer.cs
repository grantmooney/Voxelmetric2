public class SimpleTopLayer : SimpleLayer
{
    public int topBlockId;
    public override int ApplyLayerCol(int x, int z, int head)
    {
        var add = Noise(x, z, offset, frequency, amplitude) + baseHeight;

        var top = add;
        if (!absolute) {
            top += head;
        }
        if (absolute && head > add) {
            return head;
        }

        if (head > 0)
        {
            Utils.ProfileCall(() =>
            {
                filler.SetBlocks(x, z, head, top - 1, new Block(blockId));
                // "Top"/"Ground" layer
                filler.SetBlocks(x, z, top - 1, top, new Block(topBlockId));
            }, "Set Blocks");
        }
        return top;
    }
}
