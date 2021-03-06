﻿using UnityEngine;

public abstract class LayerStore : MonoBehaviour
{
    public abstract TerrainLayer[] GetLayers(Voxelmetric vm);
}