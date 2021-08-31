using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEntryPickupItem : MenuEntry
{
    
    InventoryManager inventory;
    ItemBehaviour itemObj;
    
    
    public override void OnClick()
    {
        if( inventory.AddItem(itemObj.LocItem.Item) ) {
            itemObj.Pickup();
        }
    }
    
    
    public MenuEntryPickupItem( InventoryManager inventory, ItemBehaviour itemObj ) : base(itemObj.LocItem.Item.Name) {
        this.inventory = inventory;
        this.itemObj = itemObj;
    }
    
}
