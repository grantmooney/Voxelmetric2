public class FoliageCube2dBlockType : FoliageCubeBlockType
{
    protected override void AddMeshData(Chunk chunk, Pos pos, Block block, ref MeshData meshData)
    {
        foreach (var dir in DirectionUtils.Directions)
        {
            if (dir == Direction.north || dir == Direction.south || !chunk.GetBlock(pos + dir).GetBlockType(vm).IsSolid(chunk, pos, block, DirectionUtils.Opposites[dir]))
            {
                // Side
                meshData.verts.AddRange(MeshArrays.VertexCubeFaces(pos - chunk.Pos, chunk.BlockSize, dir));
                meshData.tris.AddRange(MeshArrays.TriCubeFaces(meshData.verts.Count));
                if (dir == Direction.up)
                {
                    // Foliage top texture
                    meshData.uvs.AddRange(MeshArrays.QuadFaceTexture(foliageTextureSet.GetTexture(chunk, pos, dir)));
                    // Add Grass
                    meshData.verts.AddRange(MeshArrays.VertexFoliageFaces(pos - chunk.Pos, chunk.BlockSize));
                    meshData.tris.AddRange(MeshArrays.TriFoliageFaces(meshData.verts.Count));
                    meshData.uvs.AddRange(MeshArrays.QuadFaceTexture(foliageTextureSet.GetTexture(chunk, pos, Direction.south)));
                }
                else
                {
                    // Normal top uvs
                    meshData.uvs.AddRange(MeshArrays.QuadFaceTexture(textureSet.GetTexture(chunk, pos, dir)));
                }
            }
        }
    }
}

