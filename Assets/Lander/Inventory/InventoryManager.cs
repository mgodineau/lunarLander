using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : IinventoryItem
{
    
    private List<IinventoryItem> _items = new List<IinventoryItem>();
    public IEnumerable<IinventoryItem> Items
    {
        get { return _items; }
    }

    public float Mass
    {
        get { 
            float mass = 0;
            foreach( IinventoryItem item in Items ) {
                mass += item.Mass;
            }
            return mass;
        }
    }
    
    private float _volume;
    public float Volume {
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
    
    public string Name {
        get{ return "Inventory"; }
    }
    
    private List<MenuEntry> publicEntries = new List<MenuEntry>();
    
    
    public bool AddItem(IinventoryItem item)
    {
        if (_items.Contains(item) || Mass + item.Mass > _maxVolume)
        {
            return false;
        }
        
        _items.Add(item);
        _volume += item.Volume;
        
        publicEntries.Add( new MenuEntryDropItem(this, item) );
        if( UImanager.Instance != null ) {
            UImanager.Instance.menuManager.UpdateMenuUI();            
        }
        
        if( item is Instrument ) {
            UImanager.Instance.instrumentsManager.EnableInstrument(item as Instrument);
        }
        return true;
    }
    
    
    public bool RemoveItem(IinventoryItem item)
    {
        bool result = _items.Remove(item);
        if (result)
        {
            publicEntries.RemoveAll( entry => entry is MenuEntryDropItem && (entry as MenuEntryDropItem).Item == item );
            if( UImanager.Instance != null) {
                UImanager.Instance.menuManager.UpdateMenuUI();
            }
            
            _volume -= item.Volume;
            if( item is Instrument ) {
                UImanager.Instance.instrumentsManager.DisableInstrument( item as Instrument );
            }
        }
        return result;
    }


    
    public InventoryManager( float maxVolume = 20.0f ) {
        _maxVolume = Mathf.Max(maxVolume, 0);
    }

    public SubMenu GetMenu()
    {
        // publicEntries.Clear();
        
        // foreach( IinventoryItem item in Items ) {
        //     // content.Add( new MenuEntryEmpty(item.Name) );
        //     publicEntries.Add( new MenuEntryDropItem(this, item) );
        // }
        
        return new SubMenu( Name, publicEntries);
    }
}
