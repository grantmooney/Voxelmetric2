using UnityEngine;
public static class MeshArrays {
    public static Vector3[] VertexFoliageFaces(Pos pos, float blockSize)
    {
        var s = 0.5f * blockSize;
        // Grass
        return new Vector3[] {
                // Standing grass
                new Vector3(pos.x - s, pos.y + s, pos.z),
                new Vector3(pos.x - s, pos.y + (3*s), pos.z),
                new Vector3(pos.x + s, pos.y + (3*s), pos.z),
                new Vector3(pos.x + s, pos.y + s, pos.z)
            };
    }


    public static Vector3[] VertexCubeFaces(Pos pos, float blockSize, Direction dir)
    {
        var s = 0.5f * blockSize;

        switch (dir)
        {
            case Direction.north:
                return new Vector3[] {
                    new Vector3(pos.x + s, pos.y - s, pos.z + s),
                    new Vector3(pos.x + s, pos.y + s, pos.z + s),
                    new Vector3(pos.x - s, pos.y + s, pos.z + s),
                    new Vector3(pos.x - s, pos.y - s, pos.z + s)
                };
            case Direction.east:
                return new Vector3[] {
                    new Vector3(pos.x + s, pos.y - s, pos.z - s),
                    new Vector3(pos.x + s, pos.y + s, pos.z - s),
                    new Vector3(pos.x + s, pos.y + s, pos.z + s),
                    new Vector3(pos.x + s, pos.y - s, pos.z + s)
                };
            case Direction.south:
                return new Vector3[] {
                    new Vector3(pos.x - s, pos.y - s, pos.z - s),
                    new Vector3(pos.x - s, pos.y + s, pos.z - s),
                    new Vector3(pos.x + s, pos.y + s, pos.z - s),
                    new Vector3(pos.x + s, pos.y - s, pos.z - s)
                };
            case Direction.west:
                return new Vector3[] {
                    new Vector3(pos.x - s, pos.y - s, pos.z + s),
                    new Vector3(pos.x - s, pos.y + s, pos.z + s),
                    new Vector3(pos.x - s, pos.y + s, pos.z - s),
                    new Vector3(pos.x - s, pos.y - s, pos.z - s)
                };
            case Direction.up:
                return new Vector3[] {
                    new Vector3(pos.x - s, pos.y + s, pos.z + s),
                    new Vector3(pos.x + s, pos.y + s, pos.z + s),
                    new Vector3(pos.x + s, pos.y + s, pos.z - s),
                    new Vector3(pos.x - s, pos.y + s, pos.z - s)
                };
            case Direction.down:
                return new Vector3[] {
                    new Vector3(pos.x - s, pos.y - s, pos.z - s),
                    new Vector3(pos.x + s, pos.y - s, pos.z - s),
                    new Vector3(pos.x + s, pos.y - s, pos.z + s),
                    new Vector3(pos.x - s, pos.y - s, pos.z + s)
                };
            default:
                return null;
        }
    }

    public static int[] TriCubeFaces(int vertIndex) {
        return new int[] {
            (vertIndex - 4),
            (vertIndex - 3),
            (vertIndex - 2),
            (vertIndex - 4),
            (vertIndex - 2),
            (vertIndex - 1)
        };
    }

    public static int[] TriFoliageFaces(int vertIndex)
    {
        return new int[] {
            (vertIndex - 8),
            (vertIndex - 7),
            (vertIndex - 6),
            (vertIndex - 8),
            (vertIndex - 6),
            (vertIndex - 5),
            // Billboard
            (vertIndex - 4),
            (vertIndex - 3),
            (vertIndex - 2),
            (vertIndex - 4),
            (vertIndex - 2),
            (vertIndex - 1)
        };
    }

    public static Vector2[] QuadFaceTexture(Rect texture, int rotation = 0)
    {
        var UVs = new Vector2[4] {
                new Vector2(texture.x, texture.y),
                new Vector2(texture.x, texture.y + texture.height),
                new Vector2(texture.x + texture.width, texture.y + texture.height),
                new Vector2(texture.x + texture.width, texture.y)
            };

        switch (rotation) {
            case 0:
                return UVs;
            case 1:
                return new Vector2[] { UVs[1], UVs[2], UVs[3], UVs[0] };
            case 2:
                return new Vector2[] { UVs[2], UVs[3], UVs[0], UVs[1] };
            case 3:
                return new Vector2[] { UVs[3], UVs[0], UVs[1], UVs[2] };
            default:
                return UVs;
        }
    }
}
