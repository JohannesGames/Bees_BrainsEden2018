using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenTilePool : MonoBehaviour
{
    [HideInInspector]
    public List<GardenTile> pooledTiles = new List<GardenTile>();
    GardenTile selectedTile;

    public void CreateTilePool()
    {
        for (int i = 0; i < GameManager.gm.gl.allGardenTiles.Length; i++)
        {
            pooledTiles.Add(Instantiate(GameManager.gm.gl.allGardenTiles[i], transform));
            pooledTiles[i].gameObject.SetActive(false);
        }
    }

    public GardenTile GetGardenTile(int randomIndex)
    {
        selectedTile = pooledTiles[randomIndex];
        pooledTiles.RemoveAt(randomIndex);
        return selectedTile;
    }

    public void AddToPool(GardenTile _tile)
    {
        _tile.transform.SetParent(transform);
        pooledTiles.Add(_tile);
        _tile.gameObject.SetActive(false);
    }
}
