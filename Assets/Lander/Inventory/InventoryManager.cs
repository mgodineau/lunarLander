using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : InventoryItem
{
    
    private Lander lander;
    
    private List<InventoryItem> _items = new List<InventoryItem>();
    public IEnumerable<InventoryItem> Items
    {
        get { return _items; }
    }

    public override float Mass
    {
        get { 
            float mass = 0;
            foreach( InventoryItem item in Items ) {
                mass += item.Mass;
            }
            return mass;
        }
    }
    
    private float _volume;
    public override float Volume {
        get {return _volume;}
    }
    
    
    private float _maxVolume;
    public float MaxVolume
    {
        get { return _maxVolume; }
        set
        {
            if (value >= Volume)
            {
                _maxVolume = value;
            }
        }
    }
    
    public override string Name {
        get{ return "Inventory (" + Volume + "/" + MaxVolume + ")"; }
    }
    
    private List<MenuEntry> dropEntries = new List<MenuEntry>();
    
    
    
    public override ItemBehaviour InstantiateWorldItem( LocalizedItem locItem )
    {
        GameObject instance = GameObject.Instantiate( TerrainManager.Instance.Prefabs.CratePref.gameObject );
        ItemBehaviour itemObj = instance.AddComponent<ItemBehaviour>();
        itemObj.LocItem = new LocalizedItem(this);
        
        return itemObj;
    }
    
    
    
    public bool AddItem(InventoryItem item)
    {
        if (_items.Contains(item) || Mass + item.Mass > _maxVolume)
        {
            return false;
        }
        
        _items.Add(item);
        item.inventory = this;
        _volume += item.Volume;
        
        if( item is RocketPart ) {
            dropEntries.Add( new MenuEntryDropItem( this, item) );
        }
        if( UImanager.Instance != null ) {
            UImanager.Instance.menuManager.UpdateMenuUI();            
        }
        
        if( item is InstrumentItem ) {
            (item as InstrumentItem).OnItemPickup();
        }
        return true;
    }
    
    
    public bool RemoveItem(InventoryItem item)
    {
        bool result = _items.Remove(item);
        if (result)
        {
            item.inventory = null;
            dropEntries.RemoveAll( entry => entry is MenuEntryDropItem && (entry as MenuEntryDropItem).Item == item );
            if( UImanager.Instance != null) {
                UImanager.Instance.menuManager.UpdateMenuUI();
            }
            
            _volume -= item.Volume;
            if( item is InstrumentItem ) {
                ( item as InstrumentItem ).OnItemDrop();
            }
        }
        return result;
    }
    
    
    
    
    public SubMenu GetDropMenu() {
        string name = "Drop Item (" + dropEntries.Count + ")";
        return new SubMenu( name, dropEntries );
    }
    
    
    public Vector3 GetDropPosition() {
        // return TerrainManager.Instance.ConvertXtoDir( lander.transform.position.x );
        return lander.DropPosition.position;
    }
    
    
    
    public InventoryManager( Lander lander, float maxVolume = 20.0f ) {
        this.lander = lander;
        _maxVolume = Mathf.Max(maxVolume, 0);
    }
    
}
