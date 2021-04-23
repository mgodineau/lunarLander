using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager
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

    private float _maxMass;
    public float MaxWeight
    {
        get { return _maxMass; }
        set
        {
            if (value >= _mass)
            {
                _maxMass = value;
            }
        }
    }


    public bool AddItem(IinventoryItem item)
    {
        if (_items.Contains(item) || _mass + item.Mass > _maxMass)
        {
            return false;
        }
        
        _items.Add(item);
        _mass += item.Mass;
        
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
            _mass -= item.Mass;
            if( item is Instrument ) {
                InstrumentsManager.Instance.DisableInstrument( item as Instrument );
            }
        }
        return result;
    }


    
    public InventoryManager( float maxMass = 20.0f ) {
        _maxMass = Mathf.Max(maxMass, 0);
    }

    
    

}
