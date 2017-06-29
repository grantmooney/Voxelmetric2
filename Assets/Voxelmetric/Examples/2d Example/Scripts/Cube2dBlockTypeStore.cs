public class Cube2dBlockTypeStore : BlockTypeStore
{
    public override BlockType[] GetBlockTypes(Voxelmetric vm)
    {
        return new BlockType[] {
            new Cube2dBlockType {
                blockName = "rock",
                id = 2,
                solid = true,
                canBeWalkedOn = true,
                canBeWalkedThrough = false,
                textureName = "rock",
            },
            new Cube2dBlockType {
                blockName = "dirt",
                id = 3,
                solid = true,
                canBeWalkedOn = true,
                canBeWalkedThrough = false,
                textureName = "dirt",
            },
            new FoliageCube2dBlockType {
                blockName = "grass",
                id = 4,
                solid = true,
                canBeWalkedOn = true,
                canBeWalkedThrough = false,
                textureName = "dirt",
                foliageTextureName = "grass"
            },
            new Cube2dBlockType {
                blockName = "sand",
                id = 5,
                solid = true,
                canBeWalkedOn = true,
                canBeWalkedThrough = true,
                textureName = "sand",
            }
        };
    }
}
