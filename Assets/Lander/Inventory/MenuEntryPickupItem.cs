using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEntryPickupItem : MenuEntry
{
    
    InventoryManager inventory;
    CrystalBehaviour item;
    
    
    public override void OnClick()
    {
        if( inventory.AddItem(item.crystalScript) ) {
            item.Pickup();
        }
    }
    
    
    public MenuEntryPickupItem( InventoryManager inventory, CrystalBehaviour item ) : base(item.crystalScript.Name) {
        this.inventory = inventory;
        this.item = item;
    }
    
}
