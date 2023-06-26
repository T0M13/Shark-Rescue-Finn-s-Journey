using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChunkTypes;

[System.Serializable]
public class ChunkListPrefabs
{
    public EChunkType EChunkType = new();
    [Tooltip("Tiles for the player lane.\nTiles that will be used to create a Prefabs.")]
    public List<GameObject> MainFloorTilePrefabs = new();
    [Tooltip("Tiles for the environmet lane.\nTiles that will be used to create a Prefabs.")]
    public List<GameObject> EnvFloorTilePrefabs = new();
    [Tooltip("Currently disabled Chunks")]
    public List<GameObject> DisabledChunkList = new();
    [Tooltip("Currently active Chunks")]
    public List<GameObject> ActiveChunkList = new();
    [Tooltip("Unique Environment Chunk Prefabs.")]
    public List<GameObject> DisabledEnvChunkList = new();
    [Tooltip("Unique Environment Chunk Prefabs.")]
    public List<GameObject> ActiveEnvChunkList = new();
}
