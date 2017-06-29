using System.Collections.Generic;
using UnityEngine;

public class Load2dChunks : LoadChunks
{
    private Pos objectPos;
    
    Pos[] chunkPositions;

    readonly List<Pos> chunksToRender = new List<Pos>();
    readonly List<Pos> chunksToGenerate = new List<Pos>();



    void Start()
    {
        chunks = vm.components.chunks;
        chunkSize = chunks.chunkSize;
        chunkPositions = ChunkLoadOrder.ChunkPositions(chunkLoadRadius);
        distanceToDeleteInUnitsSquared = (int)(DistanceToDeleteChunks * chunkSize * chunks.blockSize);
        distanceToDeleteInUnitsSquared *= distanceToDeleteInUnitsSquared;
    }

    // Update is called once per frame
    void Update()
    {
        objectPos = transform.position;

        if (deleteTimer == WaitBetweenDeletes)
        {
            DeleteChunks();

            deleteTimer = 0;
            return;
        }
        else
        {
            deleteTimer++;
        }

        if (chunksToRender.Count != 0)
        {
            for (int i = 0; i < ChunksToLoadPerFrame; i++)
            {
                if (chunksToRender.Count == 0)
                {
                    break;
                }

                var pos = chunksToRender[0];
                var chunk = chunks.GetChunk(pos);
                if (chunk != null) {
                    chunk.Render();
                }
                chunksToRender.RemoveAt(0);
            }

            if (RenderChunksInSeparateFrame)
            {
                return;
            }
        }

        if (chunksToGenerate.Count == 0)
        {
            FindChunksAndLoad();
        }

        for (int i = 0; i < ChunksToLoadPerFrame; i++)
        {
            if (chunksToGenerate.Count == 0)
            {
                break;
            }

            var pos = chunksToGenerate[0];
            chunks.CreateChunk(pos);

            chunksToGenerate.RemoveAt(0);
            chunksToRender.Add(pos);
        }
    }

    protected override void DeleteChunks()
    {
        var posX = objectPos.x;
        var chunksToDelete = new List<Pos>();
        foreach (var chunk in chunks.GetChunks())
        {
            var xd = posX - chunk.Pos.x;
            if ((xd * xd) > distanceToDeleteInUnitsSquared)
            {
                chunksToDelete.Add(chunk.Pos);
            }
        }

        for (int i = 0; i < chunksToDelete.Count; i++)// (var chunk in chunksToDelete)
        {
            chunks.Destroy(chunksToDelete[i]);
        }
    }

    bool FindChunksAndLoad()
    {
        // Cycle through the array of positions
        for (int i = 0; i < chunkPositions.Length; i++)
        {
            // Get the position of this gameobject to generate around
            var playerPos = chunks.GetChunkPos(transform.position);

            // translate the player position and array position into chunk position
            var newChunkPos = new Pos(chunkPositions[i].x * chunkSize + playerPos.x, 0);

            if (chunksToGenerate.Contains(newChunkPos) || chunksToRender.Contains(newChunkPos))
            {
                continue;
            }

            // Get the chunk in the defined position
            var newChunk = chunks.GetChunk(newChunkPos);

            // If the chunk already exists and it's already
            // rendered or in queue to be rendered continue
            if (newChunk == null)
            {
                for (int y = minChunkY; y <= maxChunkY; y += chunkSize)
                {
                    chunksToGenerate.Add(new Pos(newChunkPos.x, y));
                }
                return true;
            }
            else if (!newChunk.Rendered)
            {
                for (int y = minChunkY; y <= maxChunkY; y += chunkSize)
                {
                    chunksToRender.Add(new Pos(newChunkPos.x, y));
                }
                return true;
            }
        }
        return false;
    }
}