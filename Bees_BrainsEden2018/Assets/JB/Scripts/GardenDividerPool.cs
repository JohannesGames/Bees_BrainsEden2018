using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GardenDividerPool : MonoBehaviour
{
    [HideInInspector]
    public List<GardenDivider> pooledDividers = new List<GardenDivider>();
    GardenDivider selectedDivider;
    public List<GardenDivider> placedDividers = new List<GardenDivider>();


    public void CreateDividerPool()
    {
        for (int i = 0; i < GameManager.gm.gl.allGardenDividers.Length; i++)
        {
            pooledDividers.Add(Instantiate(GameManager.gm.gl.allGardenDividers[i], transform));
            pooledDividers[i].gameObject.SetActive(false);
        }
    }

    public GardenDivider GetDivider(int randomIndex)
    {
        selectedDivider = pooledDividers[randomIndex];
        pooledDividers.RemoveAt(randomIndex);
        return selectedDivider;
    }

    public void AddToPool(GardenDivider _divider)
    {
        _divider.transform.SetParent(transform);
        pooledDividers.Add(_divider);
        _divider.gameObject.SetActive(false);
        placedDividers.Remove(_divider);
    }

    public void RecallDividers()
    {
        foreach (GardenDivider div in placedDividers)
        {
            AddToPool(div);
        }
    }
}
