using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnvironmentType;

[System.Serializable]
public class ChunkListPrefabs
{
    [Header("Environment Type")]
    public EEnvironmentType EEnvironmentType = new();
    //[Space(10)]
    [Header("Floor Tile Prefabs")]
    [Tooltip("Tiles for the player lane.\nTiles that will be used to create a Prefabs.")]
    public List<GameObject> MainFloorTilePrefabs = new();
    [Tooltip("Tiles for the Environmet lane.\nTiles that will be used to create a Prefabs.")]
    public List<GameObject> EnvFloorTilePrefabs = new();
    //[Space(10)]
    [Header("Active/Disabled Main Chunks")]
    [Tooltip("Currently disabled Chunks")]
    public List<GameObject> DisabledChunkList = new();
    [Tooltip("Currently active Chunks.")]
    public List<GameObject> ActiveChunkList = new();
    //[Space(20)]
    [Header("Finished Environment Chunk Prefabs")]
    [Tooltip("Unique Environment Chunk Prefabs.")]
    public List<GameObject> EnvChunkPrefabs = new();
    [Header("Active/Disabled Environment Chunks")]
    [Tooltip("Currently disabled Env Chunks.")]
    public List<GameObject> DisabledEnvChunkList = new();
    [Tooltip("urrently active Env Chunks.")]
    public List<GameObject> ActiveEnvChunkList = new();
}
