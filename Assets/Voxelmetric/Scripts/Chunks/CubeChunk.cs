using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

public class CubeChunk : Chunk {

    public Block[,,] blocks;

    protected MeshFilter filter;
    protected MeshCollider col;

    protected bool rendering;
    protected bool meshDataReady;

    public override void VmStart(Pos position, ChunkController chunkController)
    {
        Pos = position;
        transform.position = position;
        this.ChunkController = chunkController;
        Vm = chunkController.vm;

        blocks = new Block[ChunkSize, ChunkSize, ChunkSize];
        Vm.components.chunkFiller.FillChunk(this);

        if (filter == null)
        {
            col = gameObject.AddComponent<MeshCollider>();
            filter = gameObject.AddComponent<MeshFilter>();
            var renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.material = chunkController.vm.components.textureLoader.material;
        }

        transform.parent = chunkController.transform;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Clears all data out of the chunk so that it can be returned to the chunk pool
    /// </summary>
    public override void Clear()
    {
        base.Clear();

        if (filter.mesh != null) filter.mesh.Clear();
        if(col.sharedMesh!=null) col.sharedMesh.Clear();
        blocks = new Block[0,0,0];
    }

    public override void LateUpdate()
    {
        if (RenderStale)
        {
            Render();
        }

        if (meshDataReady && Vm.settings.threading)
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

    #region Mesh create and render
    public override void Render()
    {
        if (rendering) return;
        rendering = true;
        RenderStale = false;

        if (!Rendered)
        {
            CreateChunkNeighbors();
        }

        // Set the rendered flag to true even though the mesh isn't rendered yet because all
        // the work for it is done and does not need to be called again.
        Rendered = true;

        if (Vm.settings.threading)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(CreateChunkMeshDelegate), meshData);
        }
        else
        {
            CreateChunkMesh(meshData);
            AssignMesh(meshData);

            meshData.Clear();
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
                ChunkController.CreateChunk(Pos + (ChunkController.chunkSize * (Pos)dir));
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
                    for (int z = 0; z < blocks.GetLength(2); z++)
                    {
                        blocks[x, y, z].GetBlockType(ChunkController.vm).Render(this, new Pos(x, y, z) + Pos, blocks[x, y, z], meshData);
                    }
                }
            }
        }

        meshDataReady = true;
    }

    /// <summary>
    /// Takes the mesh data passed to it and assigns it to the chunks renderer replacing the original contents
    /// </summary>
    /// <param name="meshData">The meshdata to assign</param>
    protected override void AssignMesh(MeshData meshData)
    {

        var mesh = new Mesh()
        {
            name = "Mesh for " + name,
            vertices = meshData.verts.ToArray(),
            triangles = meshData.tris.ToArray(),
            uv = meshData.uvs.ToArray()
        };
        filter.mesh = mesh;

        mesh = new Mesh()
        {
            name = "Collision mesh for " + name,
            vertices = meshData.verts.ToArray(),
            triangles = meshData.tris.ToArray()
        };
        col.sharedMesh = mesh;

        meshData.Clear();
    }
    #endregion

    #region Get, Set blocks
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
            blockPos.y - Pos.y,
            blockPos.z - Pos.z
        ];
    }

    /// <summary>
    /// Replaces the block at the given location with the newBlock
    /// </summary>
    /// <param name="newBlock">The block to place at the target location</param>
    /// <param name="blockPos">position to place the new block</param>
    /// <returns>Returns the block that was replaced</returns>
    public override Block SetBlock(Block newBlock, Pos blockPos, bool updateRender = true)

    {
        var oldBlock = GetBlock(Pos);
        oldBlock.GetBlockType(ChunkController.vm).OnDestroy(this, Pos, oldBlock, 0);

        blocks[
            blockPos.x - Pos.x,
            blockPos.y - Pos.y,
            blockPos.z - Pos.z
        ] = newBlock.GetBlockType(ChunkController.vm).OnCreate(this, Pos, newBlock);

        if (updateRender) RenderSoon();

        return oldBlock;
    }
    #endregion

    #region Save and load
    public override void AddUnsavedBlock(Pos pos){ }
    public override void ClearUnsavedBlocks(){ }
    public override bool HasUnsavedBlocks(){ throw new NotImplementedException(); }
    public override void DeserializeChunk(List<byte> data){ }
    public override List<byte> SerializeChunk(byte storeMode){ throw new NotImplementedException(); } 
    #endregion
}
