using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    
    public LocalizedItem item;
    
    
    public void Pickup()
    {
        // TerrainManager.Instance.Planet.RemoveItem(item);
        // TerrainManager.Instance.Planet.items.Remove(item);
        TerrainManager.Instance.RemoveObject(item);
        Destroy( gameObject );
    }
    
}
