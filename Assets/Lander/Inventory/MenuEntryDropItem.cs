

class MenuEntryDropItem : MenuEntry
{
    
    private IinventoryItem _item;
    public IinventoryItem Item {
        get { return _item; }
    }
    
    private InventoryManager inventory;
    
    public override void OnClick()
    {
        inventory.RemoveItem(_item);
        
    }
    
    
    public MenuEntryDropItem( InventoryManager inventory, IinventoryItem item ) : base(item.Name){
        this.inventory = inventory;
        this._item = item;
    }
    
}