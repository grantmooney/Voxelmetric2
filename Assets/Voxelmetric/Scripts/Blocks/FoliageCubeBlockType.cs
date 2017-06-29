public class FoliageCubeBlockType : CubeBlockType
{
    protected TextureSet foliageTextureSet;
    public string foliageTextureName;

    public override void Initialize(Voxelmetric vm)
    {
        base.Initialize(vm);
        textureSet = vm.components.textureLoader.GetByName(textureName);
        foliageTextureSet = vm.components.textureLoader.GetByName(foliageTextureName);
    }

    protected override void AddMeshData(Chunk chunk, Pos pos, Block block, ref MeshData meshData)
    {
        foreach (var dir in DirectionUtils.Directions)
        {
            if (!chunk.GetBlock(pos + dir).GetBlockType(vm).IsSolid(chunk, pos, block, DirectionUtils.Opposites[dir]))
            {
                // Side
                meshData.verts.AddRange(MeshArrays.VertexCubeFaces(pos - chunk.Pos, chunk.BlockSize, dir));
                meshData.tris.AddRange(MeshArrays.TriCubeFaces(meshData.verts.Count));
                if (dir == Direction.up)
                {
                    // Foliage top texture
                    meshData.uvs.AddRange(MeshArrays.QuadFaceTexture(foliageTextureSet.GetTexture(chunk, pos, dir)));
                    // Add FoliageGrass
                    meshData.verts.AddRange(MeshArrays.VertexFoliageFaces(pos - chunk.Pos, chunk.BlockSize));
                    meshData.tris.AddRange(MeshArrays.TriFoliageFaces(meshData.verts.Count));
                    meshData.uvs.AddRange(MeshArrays.QuadFaceTexture(foliageTextureSet.GetTexture(chunk, pos, Direction.south)));
                }
                else
                {
                    // Normal sides
                    meshData.uvs.AddRange(MeshArrays.QuadFaceTexture(textureSet.GetTexture(chunk, pos, dir)));
                }
            }
        }
    }
}

