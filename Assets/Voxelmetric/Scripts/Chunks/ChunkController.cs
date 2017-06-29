using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class ChunkController : MonoBehaviour {

    internal Voxelmetric vm;

    [SerializeField]
    private int _chunkPower = 4;

    [HideInInspector]
    public int chunkSize;
    [HideInInspector]
    public int chunkPower;

    [SerializeField]
    private float _blockSize = 1;
    [HideInInspector]
    public float blockSize;

    public virtual void Initialize(Voxelmetric vm)
    {
        this.vm = vm;
        chunkPower = _chunkPower;
        chunkSize = 1 << chunkPower;
        blockSize = _blockSize;
    }

    public virtual void Destroy(Pos chunkPos)
    {
        var chunk = GetChunk(chunkPos);
        if (chunk != null) Destroy(chunk);
    }

    public abstract void Destroy(Chunk chunk);

    public abstract Chunk GetChunk(Pos chunkPos);

    public abstract Block GetBlock(Pos blockPos);

    public virtual void SetBlock(Pos blockPos, Block block)
    {
        var chunk = GetChunk(blockPos);

        if (chunk != null)
        {
            chunk.SetBlock(block, blockPos);
        }
    }

    public virtual Chunk CreateChunk(Pos pos)
    {
        // Convert block position to chunk position
        pos = GetChunkPos(pos);

        var chunkGO = new GameObject("Chunk at " + pos, new Type[] {
            vm.components.chunkType.GetType()
        });

        var chunk = chunkGO.GetComponent<Chunk>();
        chunk.VmStart(pos, this);

        return chunk;
    }

    public virtual Pos GetBlockPos(Vector3 point)
    {
        point -= transform.position;
        point = Quaternion.Inverse(transform.rotation) * point;

        point /= vm.components.chunks.blockSize;

        return new Pos(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y), Mathf.RoundToInt(point.z));
    }

    public virtual Pos GetBlockPos(Vector2 point)
    {
        point -= (Vector2)transform.position;
        point = Quaternion.Inverse(transform.rotation) * point;

        point /= vm.components.chunks.blockSize;

        return new Pos(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y));
    }

    public virtual Vector3 GetWorldPos(Pos blockPos)
    {
        return (transform.rotation * blockPos) + transform.position;
    }

    public virtual Pos GetChunkPos(Pos blockPos)
    {
        return new Pos((blockPos.x >> chunkPower) << chunkPower,
                               (blockPos.y >> chunkPower) << chunkPower,
                               (blockPos.z >> chunkPower) << chunkPower);
    }

    public virtual List<Chunk> GetChunks()
    {
        return new List<Chunk>();
    }

    public virtual byte[] Serialize(byte storeMode)
    {
        return new byte[0];
    }

    public virtual void Deserialize(byte[] data) { }

    public void ClearAllStaleBlocks()
    {
        foreach (var chunk in GetChunks())
        {
            chunk.ClearStaleBlocks();
        }
    }
}
