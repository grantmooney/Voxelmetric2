﻿public class ExampleBlockTypeStore : BlockTypeStore
{
    public override BlockType[] GetBlockTypes(Voxelmetric vm)
    {
        return new BlockType[] {
            new CubeBlockType {
                blockName = "rock",
                id = 2,
                solid = true,
                canBeWalkedOn = true,
                canBeWalkedThrough = false,
                textureName = "rock"
            },
            new CubeBlockType {
                blockName = "dirt",
                id = 3,
                solid = true,
                canBeWalkedOn = false,
                canBeWalkedThrough = false,
                textureName = "dirt"
            },
            new FoliageCubeBlockType {
                blockName = "grass",
                textureName = "dirt",
                foliageTextureName = "grass",
                id = 4,
                solid = true,
                canBeWalkedOn = true,
                canBeWalkedThrough = true
            },
            new CubeBlockType {
                blockName = "sand",
                id = 5,
                solid = true,
                canBeWalkedOn = true,
                canBeWalkedThrough = true,
                textureName = "sand"
            }
        };
    }
}
