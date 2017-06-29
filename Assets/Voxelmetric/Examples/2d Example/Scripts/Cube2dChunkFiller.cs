using System.Collections.Generic;
using SimplexNoise;

public class Cube2dChunkFiller : ChunkFiller
{
    public string seed;

    public TerrainLayer[] layers;
    private readonly Dictionary<Pos, Block[,]> stored = new Dictionary<Pos, Block[,]>();
    private int chunkSize;

    public int minChunkY = -32;
    public int maxChunkY = 32;

    public override void Initialize(Voxelmetric vm)
    {
        base.Initialize(vm);
        noise = new Noise(seed);

        layers = FindObjectOfType<LayerStore>().GetLayers(vm);

        Utils.ProfileCall(() =>
        {
            foreach (var layer in layers)
            {
                layer.VmStart(this);
            }
        }, "Initialize layers");
    }

    public override void FillChunk(Chunk chunk)
    {
        if (chunkSize == 0) {
            chunkSize = chunk.ChunkSize;
        }

        var pos = chunk.Pos;
        var chunk2d = (Cube2dChunk)chunk;

        Block[,] storedValuesForChunk;
        if (stored.TryGetValue(pos, out storedValuesForChunk))
        {
            chunk2d.blocks = storedValuesForChunk;
            stored.Remove(pos);

            return;
        }

        for (pos.y = minChunkY; pos.y <= maxChunkY; pos.y += chunk.ChunkSize)
        {
            stored.Add(pos, new Block[chunk.ChunkSize, chunk.ChunkSize]);
        }

        FillChunkColumn(pos, chunk.ChunkSize);

        chunk2d.blocks = stored[chunk2d.Pos];
        stored.Remove(chunk2d.Pos);
    }

    private void FillChunkColumn(Pos columnPos, int chunkSize)
    {
        var pos = columnPos;
        for (pos.x = columnPos.x; pos.x < columnPos.x + chunkSize; pos.x++)
        {
            FillColumn(pos.x);
        }
    }

    private void FillColumn(int x)
    {
        var head = minChunkY;
        foreach (var layer in layers)
        {
            Utils.ProfileCall(() =>
            {
                head = layer.ApplyLayerCol(x, 0, head);
            }, "Applying layer");
        }
    }

    /// <summary>
    /// Sets a column of chunks starting at startPlaceHeight and ending at endPlaceHeight.
    /// Usually faster than setting blocks one at a time from the layer.
    /// </summary>
    /// <param name="x">Column global x position</param>
    /// <param name="z">Column global z position</param>
    /// <param name="startPlaceHeight">First block's global y position</param>
    /// <param name="endPlaceHeight">Last block's global y position</param>
    /// <param name="blockToPlace">The block to fill this column with</param>
    public override void SetBlocks(int x, int z, int startPlaceHeight, int endPlaceHeight, Block blockToPlace)
    {
        // Ignore z-value
        SetBlocks(x, startPlaceHeight, endPlaceHeight, blockToPlace);
    }

    public void SetBlocks(int x, int startPlaceHeight, int endPlaceHeight, Block blockToPlace)
    {
        var chunkPos = vm.components.chunks.GetChunkPos(new Pos(x, 0));

        // Loop through each chunk in the column
        for (chunkPos.y = minChunkY; chunkPos.y <= maxChunkY; chunkPos.y += chunkSize)
        {
            // And for each one loop through its height
            for (int y = 0; y < chunkSize; y++)
            {
                //and if this is above the starting height
                if (chunkPos.y + y >= startPlaceHeight)
                {
                    // And below or equal to the end height place the block using 
                    // localSetBlock which is faster than the non-local pos set block
                    if (chunkPos.y + y < endPlaceHeight)
                    {
                        //if (y + 1 < chunkSize)
                        //{
                        //    UnityEngine.Debug.Log(y.ToString() + " / " + stored[chunkPos][x - chunkPos.x, y].ToString());
                        //}
                        stored[chunkPos][x - chunkPos.x, y] = blockToPlace;
                    }
                    else
                    {
                        // Return early, we've reached the end of the blocks to add
                        return;
                    }
                }
            }
        }
    }
}
