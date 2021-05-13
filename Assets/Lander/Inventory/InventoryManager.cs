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


    public bool AddItem(IinventoryItem item)
    {
        if (_items.Contains(item) || Mass + item.Mass > _maxVolume)
        {
            return false;
        }
        
        _items.Add(item);
        _volume += item.Volume;
        
        if( item is Instrument ) {
            InstrumentsManager.Instance.EnableInstrument(item as Instrument);
        }
        return true;
    }
    
    
    public bool RemoveItem(IinventoryItem item)
    {
        bool result = _items.Remove(item);
        if (result)
        {
            _volume -= item.Volume;
            if( item is Instrument ) {
                InstrumentsManager.Instance.DisableInstrument( item as Instrument );
            }
        }
        return result;
    }


    
    public InventoryManager( float maxVolume = 20.0f ) {
        _maxVolume = Mathf.Max(maxVolume, 0);
    }

    
    

}
