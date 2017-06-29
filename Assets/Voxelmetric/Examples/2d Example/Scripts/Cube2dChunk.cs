﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class Cube2dChunk : Chunk
{
    public Block[,] blocks;

    protected MeshFilter filter;
    protected MeshCollider col;

    protected bool rendering;
    protected bool meshDataReady;

    public override void VmStart(Pos position, ChunkController chunkController)
    {
        Utils.ProfileCall(() =>
        {
            Pos = position;
            transform.position = Pos;
        }, "Set position");

        this.ChunkController = chunkController;
        Utils.ProfileCall(() =>
        {
            blocks = new Block[ChunkSize, ChunkSize];
        }, "Create blocks array");

        Utils.ProfileCall(() =>
        {
            chunkController.vm.components.chunkFiller.FillChunk(this);
        }, "Fill Chunk");

        Utils.ProfileCall(() =>
        {
            if (filter == null)
            {
                col = gameObject.AddComponent<MeshCollider>();
                filter = gameObject.AddComponent<MeshFilter>();
                var renderer = gameObject.AddComponent<MeshRenderer>();
                renderer.material = chunkController.vm.components.textureLoader.material;
            }
        }, "Adding components");

        Utils.ProfileCall(() =>
        {
            transform.parent = chunkController.transform;
        }, "Setting parent");

        Utils.ProfileCall(() =>
        {
            gameObject.SetActive(true);
        }, "Activating game object");
    }

    /// <summary>
    /// Clears all data out of the chunk so that it can be returned to the chunk pool
    /// </summary>
    public override void Clear()
    {
        meshData.Clear();
        if (filter.mesh != null) filter.mesh.Clear();
        if (col.sharedMesh != null) col.sharedMesh.Clear();
        blocks = new Block[0, 0];
        filter.mesh = null;
        Rendered = false;
    }

    public override void Render()
    {
        if (rendering) return;
        rendering = true;

        if (!Rendered)
        {
            CreateChunkNeighbors();
        }

        // Set the rendered flag to true even though the mesh isn't rendered yet because all
        // the work for it is done and does not need to be called again.
        Rendered = true;

        //ThreadPool.QueueUserWorkItem(new WaitCallback(CreateChunkMeshDelegate), meshData);
        CreateChunkMesh(meshData);
    }

    public override void LateUpdate()
    {
        if (meshDataReady)
        {
            Utils.ProfileCall(() =>
            {
                AssignMesh(meshData);
            }, "Assign mesh");

            Utils.ProfileCall(() =>
            {
                meshData.Clear();
            }, "Clear mesh");

            rendering = false;
            meshDataReady = false;
        }
    }

    protected void CreateChunkNeighbors()
    {
        Utils.ProfileCall(() =>
        {
            // First time this chunk is rendered - make sure that all neighboring
            // chunks are filled before this chunk can be rendered
            foreach (var dir in DirectionUtils.Directions)
            {
                if (dir != Direction.north && dir != Direction.south)
                {
                    ChunkController.CreateChunk(Pos + (ChunkController.chunkSize * (Pos)dir));
                }
            }
        }, "Create neighbors");
    }

    void CreateChunkMeshDelegate(System.Object meshData)
    {
        CreateChunkMesh((MeshData)meshData);
    }

    /// <summary>
    /// Creates a chunk mesh from the blocks the mesh contains. The mesh should use the
    /// chunk's position as it's origin point.
    /// </summary>
    /// <param name="meshData">The meshdata to create</param>
    protected override void CreateChunkMesh(MeshData meshData)
    {
        lock (meshData)
        {
            for (int x = 0; x < blocks.GetLength(0); x++)
            {
                for (int y = 0; y < blocks.GetLength(1); y++)
                {
                    blocks[x, y].GetBlockType(ChunkController.vm).Render(this, new Pos(x, y) + Pos, blocks[x, y], meshData);
                }
            }

            meshDataReady = true;
        }

    }

    /// <summary>
    /// Takes the mesh data passed to it and assigns it to the chunks renderer replacing the original contents
    /// </summary>
    /// <param name="meshData">The meshdata to assign</param>
    protected override void AssignMesh(MeshData meshData)
    {
        filter.mesh = new Mesh
        {
            name = "Mesh for " + name,
            vertices = meshData.verts.ToArray(),
            triangles = meshData.tris.ToArray(),
            uv = meshData.uvs.ToArray()
        };

        filter.mesh.RecalculateNormals();

        col.sharedMesh = new Mesh
        {
            name = "Collision mesh for " + name,
            vertices = meshData.verts.ToArray(),
            triangles = meshData.tris.ToArray()
        };

        meshData.Clear();
    }

    /// <summary>
    /// Returns the block at the specified position
    /// </summary>
    /// <param name="blockPos">Position</param>
    public override Block GetBlock(Pos blockPos)
    {
        if (blockPos.x > Pos.x + ChunkSize - 1 || blockPos.y > Pos.y + ChunkSize - 1 ||
            blockPos.z > Pos.z + ChunkSize - 1 || blockPos.x < Pos.x ||
            blockPos.y < Pos.y || blockPos.z < Pos.z)
        {
            return ChunkController.GetBlock(blockPos);
        }

        return blocks[
            blockPos.x - Pos.x,
            blockPos.y - Pos.y
        ];
    }

    /// <summary>
    /// Replaces the block at the given location with the newBlock
    /// </summary>
    /// <param name="newBlock">The block to place at the target location</param>
    /// <param name="blockPos">position to place the new block</param>
    /// <param name="updateRender">todo: describe updateRender parameter on SetBlock</param>
    /// <returns>Returns the block that was replaced</returns>
    public override Block SetBlock(Block newBlock, Pos blockPos, bool updateRender = true)
    {
        var oldBlock = GetBlock(Pos);
        oldBlock.GetBlockType(ChunkController.vm).OnDestroy(this, Pos, oldBlock, 0);

        blocks[
            blockPos.x - Pos.x,
            blockPos.y - Pos.y
        ] = newBlock.GetBlockType(ChunkController.vm).OnCreate(this, Pos, newBlock);

        if (updateRender) RenderSoon();

        return oldBlock;
    }

    #region Save and load
    public override void AddUnsavedBlock(Pos pos) { }
    public override void ClearUnsavedBlocks() { }
    public override bool HasUnsavedBlocks() { throw new NotImplementedException(); }
    public override void DeserializeChunk(List<byte> data) { }
    public override List<byte> SerializeChunk(byte storeMode) { throw new NotImplementedException(); }
    #endregion
}
