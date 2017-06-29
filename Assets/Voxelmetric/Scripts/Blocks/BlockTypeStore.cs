using UnityEngine;

public abstract class BlockTypeStore : MonoBehaviour
{
    public abstract BlockType[] GetBlockTypes(Voxelmetric vm);
}