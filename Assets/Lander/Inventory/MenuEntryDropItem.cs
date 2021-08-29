

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
        TerrainManager.Instance.Planet.AddItem(_item, inventory.GetDropPosition() );
        TerrainManager.Instance.UpdateObjetsDisplay();
    }
    
    
    public MenuEntryDropItem( InventoryManager inventory, InventoryItem item ) : base(item.Name + "(" + item.Volume + ")"){
        this.inventory = inventory;
        this._item = item;
    }
    
}