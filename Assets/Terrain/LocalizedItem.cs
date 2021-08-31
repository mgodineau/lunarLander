using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedItem : LocalizedObject
{
    
    private InventoryItem _item;
    public InventoryItem Item {
        get {return _item;}
    }
    
    
    public override ObjectBehaviour CreateInstance(Vector3 position)
    {
        ItemBehaviour instance = _item.InstantiateWorldItem(this);
        
        instance.transform.position = position;
        instance.transform.rotation = Quaternion.Euler(0, 0, rotation);
        // instance.transform.SetParent(parent, true);
        
        return instance.objectBehaviour;
    }
    
    
    
    public LocalizedItem( InventoryItem item) 
        : this(item, Vector3.right) 
    {}
    
    public LocalizedItem( InventoryItem item, Vector3 position, float height, float rotation = 0 ) 
        : this(item, position, false, height, rotation)
    {}
    
    public LocalizedItem( InventoryItem item, Vector3 position, bool isGrounded = true, float height = 0, float rotation = 0 ) 
    : base(position, isGrounded, height, rotation) 
    {
        this._item = item;
    }
    
    
}
