using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectBehaviour))]
public class ItemBehaviour : MonoBehaviour
{
    
    private LocalizedItem _locItem;
    public LocalizedItem LocItem {
        get {return _locItem;}
        set { 
            _locItem = value; 
            if( objectBehaviour == null ) { objectBehaviour = GetComponent<ObjectBehaviour>(); }
            objectBehaviour.obj = value;
        }
    }
    
    public ObjectBehaviour objectBehaviour;
    
    
    
    
    public LocalizedItem Pickup() 
    {
        TerrainManager.Instance.RemoveObject(LocItem);
        Destroy( gameObject );
        return LocItem;
    }
    
}
