﻿using UnityEngine;
<<<<<<< HEAD:Assets/Voxelmetric/Scripts/Chunks/CubeChunk.cs
=======
using System;
using System.Threading;
using System.Collections.Generic;
>>>>>>> dev:Assets/Voxelmetric/Chunks/CubeChunk.cs

public class CubeChunk : Chunk {

    public Block[,,] blocks;

    protected MeshFilter _filter;
    protected MeshCollider _col;

    protected bool _rendering;
    protected bool _meshDataReady;

    public override void VmStart(Pos position, ChunkController chunkController)
    {
        pos = position;
        transform.position = position;
        this.chunkController = chunkController;
        vm = chunkController.vm;

        blocks = new Block[chunkSize, chunkSize, chunkSize];
        vm.components.chunkFiller.FillChunk(this);

        if (_filter == null)
        {
            _col = gameObject.AddComponent<MeshCollider>();
            _filter = gameObject.AddComponent<MeshFilter>();
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

        if (_filter.mesh != null) _filter.mesh.Clear();
        if(_col.sharedMesh!=null) _col.sharedMesh.Clear();
        blocks = new Block[0,0,0];
    }

    public override void LateUpdate()
    {
        if (renderStale)
        {
            Render();
        }

        if (_meshDataReady && vm.settings.threading)
        {
            Utils.ProfileCall(() =>
            {
                AssignMesh(_meshData);
            }, "Assign mesh");

            Utils.ProfileCall(() =>
            {
                _meshData.Clear();
            }, "Clear mesh");

            _rendering = false;
            _meshDataReady = false;
        }
    }

    #region Mesh create and render
    public override void Render()
    {
        if (_rendering) return;
        _rendering = true;
        renderStale = false;

        if (!rendered)
        {
            CreateChunkNeighbors();
        }

        // Set the rendered flag to true even though the mesh isn't rendered yet because all
        // the work for it is done and does not need to be called again.
        rendered = true;

        if (vm.settings.threading)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(CreateChunkMeshDelegate), _meshData);
        }
        else
        {
            CreateChunkMesh(_meshData);
            AssignMesh(_meshData);

            _meshData.Clear();
            _rendering = false;
            _meshDataReady = false;
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
                chunkController.CreateChunk(pos + (chunkController.chunkSize * (Pos)dir));
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
                        blocks[x, y, z].GetBlockType(chunkController.vm).Render(this, new Pos(x, y, z) + pos, blocks[x, y, z], meshData);
                    }
                }
            }
        }

        _meshDataReady = true;
    }

    /// <summary>
    /// Takes the mesh data passed to it and assigns it to the chunks renderer replacing the original contents
    /// </summary>
    /// <param name="meshData">The meshdata to assign</param>
    protected override void AssignMesh(MeshData meshData)
    {
<<<<<<< HEAD:Assets/Voxelmetric/Scripts/Chunks/CubeChunk.cs
        filter.mesh = new Mesh
        {
            name = "Mesh for " + name,
            vertices = meshData.verts.ToArray(),
            triangles = meshData.tris.ToArray(),
            uv = meshData.uvs.ToArray()
        };
        
        col.sharedMesh = new Mesh
        {
            name = "Collision mesh for " + name,
            //mesh.vertices = meshData.colVerts.ToArray();
            //mesh.triangles = meshData.colTris.ToArray();
            vertices = meshData.verts.ToArray(),
            triangles = meshData.tris.ToArray()
        };
=======
        Mesh mesh = new Mesh();
        mesh.name = "Mesh for " + name;
        mesh.vertices = meshData.verts.ToArray();
        mesh.triangles = meshData.tris.ToArray();
        mesh.uv = meshData.uvs.ToArray();

        _filter.mesh = mesh;

        mesh = new Mesh();
        mesh.name = "Collision mesh for " + name;
        mesh.vertices = meshData.verts.ToArray();
        mesh.triangles = meshData.tris.ToArray();

        _col.sharedMesh = mesh;
>>>>>>> dev:Assets/Voxelmetric/Chunks/CubeChunk.cs

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
        if (blockPos.x > pos.x + chunkSize - 1 || blockPos.y > pos.y + chunkSize - 1 ||
            blockPos.z > pos.z + chunkSize - 1 || blockPos.x < pos.x ||
            blockPos.y < pos.y || blockPos.z < pos.z)
        {
            return chunkController.GetBlock(blockPos);
        }

        return blocks[
            blockPos.x - pos.x,
            blockPos.y - pos.y,
            blockPos.z - pos.z
        ];
    }

<<<<<<< HEAD:Assets/Voxelmetric/Scripts/Chunks/CubeChunk.cs
    /// <summary>
    /// Replaces the block at the given location with the newBlock
    /// </summary>
    /// <param name="newBlock">The block to place at the target location</param>
    /// <param name="blockPos">position to place the new block</param>
    /// <returns>Returns the block that was replaced</returns>
    public override Block SetBlock(Block newBlock, Pos blockPos)
=======
    public override Block SetBlock(Block newBlock, Pos blockPos, bool updateRender = true)
>>>>>>> dev:Assets/Voxelmetric/Chunks/CubeChunk.cs
    {
        var oldBlock = GetBlock(pos);
        oldBlock.GetBlockType(chunkController.vm).OnDestroy(this, pos, oldBlock, 0);

        blocks[
            blockPos.x - pos.x,
            blockPos.y - pos.y,
            blockPos.z - pos.z
        ] = newBlock.GetBlockType(chunkController.vm).OnCreate(this, pos, newBlock);

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
