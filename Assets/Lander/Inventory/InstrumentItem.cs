using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentItem : InventoryItem
{
    
    public override float Volume {
        get{ return instrument.Volume; }
    }
    
    public override float Mass {
        get { return instrument.Mass; }
    }
    
    public override string Name {
        get { return instrument.Name; }
    }
    
    
    private InstrumentBehaviour instrument;
    
    
    public InstrumentItem( InstrumentBehaviour instrument ) {
        this.instrument = instrument;
    }

    public override ItemBehaviour InstantiateWorldItem( LocalizedItem locItem )
    {
        GameObject instance = GameObject.Instantiate( TerrainManager.Instance.Prefabs.CratePref.gameObject );
        ItemBehaviour itemObj = instance.GetComponent<ItemBehaviour>();
        itemObj.LocItem = locItem;
        
        return itemObj;
    }
    
    public void OnItemPickup() {
        UImanager.Instance.instrumentsManager.EnableInstrument( instrument );
    }
    
    public void OnItemDrop() {
        UImanager.Instance.instrumentsManager.DisableInstrument( instrument );
    }
    
}
