
using UnityEngine;


class MenuEntryDropItem : MenuEntry
{
    
    private InventoryItem _item;
    public InventoryItem Item {
        get { return _item; }
    }
    
    private InventoryManager inventory;
    
    public override void OnClick()
    {
        inventory.RemoveItem(_item);
        
        Vector3 containerPosition = inventory.GetDropPosition();
        LocalizedItem locItem = TerrainManager.Instance.Planet.AddItem(
            _item, 
            TerrainManager.Instance.ConvertXtoDir(containerPosition.x), 
            containerPosition.y );
        
        UImanager.Instance.instrumentsManager.AddKnownObject(locItem);
        // TerrainManager.Instance.UpdateObjetsDisplay();
    }
    
    
    public MenuEntryDropItem( InventoryManager inventory, InventoryItem item ) : base(item.Name + "(" + item.Volume + ")"){
        this.inventory = inventory;
        this._item = item;
    }
    
}