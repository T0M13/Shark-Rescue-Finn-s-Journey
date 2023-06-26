using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChunkTypes;

[System.Serializable]
public class ChunkListPrefabs
{
    public EChunkType chunkType = new();
    [Tooltip("Unique Prefabs.")]
    public List<GameObject> ChunkPrefabs = new();
    [Tooltip("Here the respective prefabs are copied x times.")]
    public List<GameObject> disabledChunkList = new();
    [Tooltip("Currently active Obstacles")]
    public List<GameObject> activeChunkList = new();
}
