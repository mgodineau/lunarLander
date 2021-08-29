using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEntryPickupItem : MenuEntry
{
    
    InventoryManager inventory;
    ItemBehaviour itemObj;
    
    
    public override void OnClick()
    {
        if( inventory.AddItem(itemObj.item.Item) ) {
            itemObj.Pickup();
        }
    }
    
    
    public MenuEntryPickupItem( InventoryManager inventory, ItemBehaviour itemObj ) : base(itemObj.item.Item.Name) {
        this.inventory = inventory;
        this.itemObj = itemObj;
    }
    
}
