using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedItem : LocalizedObject
{
    
    private InventoryItem _item;
    public InventoryItem Item {
        get {return _item;}
    }
    
    public override GameObject CreateInstance(Vector3 position, Quaternion rotation, Transform parent)
    {
        ItemBehaviour instance = _item.InstantiateWorldItem(this);
        
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.transform.SetParent(parent, true);
        
        return instance .gameObject;
    }
    
    
    public LocalizedItem( InventoryItem item) : this(item, Vector3.right) {}
    
    public LocalizedItem( InventoryItem item, Vector3 position ) : base(position) {
        this._item = item;
    }
    
    
}
