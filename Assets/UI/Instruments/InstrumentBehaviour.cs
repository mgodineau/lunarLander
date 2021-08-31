using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class InstrumentBehaviour : UIwireframeElement
{

    public abstract float Mass
    {
        get;
    }
    
    public abstract float Volume {
        get;
    }
    
    public abstract string Name {
        get;
    }
    
    
    public InstrumentItem GetInstrumentItem() {
        return new InstrumentItem(this);
    }
    
    /*public ItemBehaviour InstantiateWorldItem()
    {
        GameObject instance = GameObject.Instantiate( TerrainManager.Instance.cratePref.gameObject );
        ItemBehaviour itemObj = instance.AddComponent<ItemBehaviour>();
        itemObj.item = new LocalizedItem(this);
        
        return itemObj;
    }*/
}
