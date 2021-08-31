using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItem
{
    
    public abstract float Volume {
        get;
    }
    
    public abstract float Mass {
        get;
    }
    
    public abstract string Name {
        get;
    }
    
    public InventoryManager inventory;
    
    
    public ItemBehaviour InstantiateWorldItem() {
        Vector3 itemPosition = inventory == null ? Vector3.right : TerrainManager.Instance.ConvertXtoDir( inventory.GetDropPosition().x);
        return InstantiateWorldItem( new LocalizedItem(this, itemPosition) );
    }
    
    public abstract ItemBehaviour InstantiateWorldItem( LocalizedItem locItem );
    
    
    public InventoryItem( InventoryManager inventory = null ) {
        this.inventory = inventory;
    }
    
}
