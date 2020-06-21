using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "tile_01", menuName = "Tiles/Create Tile", order = 1)]
public class Tile : ScriptableObject
{
    public int id = 0;
    public GameObject prefab;
}
