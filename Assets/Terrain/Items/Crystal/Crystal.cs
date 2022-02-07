using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Crystal : InventoryItem
{
    
    private const float density = 1;
    
    private float _mass;
    public override float Mass {
        get{ return _mass; }
    }
    
    public override float Volume {
        get { return Mass / density; }
    }
    
    public override string Name {
        get{ return "Crystal"; }
    }
    

    public Crystal(float mass = 1.0f) {
        _mass = Mathf.Max(mass, 0);
    }
    

    public override ItemBehaviour InstantiateWorldItem( LocalizedItem locItem )
    {
        GameObject instance = GameObject.Instantiate( TerrainManager.Instance.Prefabs.CrystalPref.gameObject);
        ItemBehaviour behaviour = instance.GetComponent<ItemBehaviour>();
        behaviour.LocItem = locItem;
        
        return behaviour;
    }
}
