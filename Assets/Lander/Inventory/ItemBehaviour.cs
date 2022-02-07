using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : ObjectBehaviour
{
	
	private LocalizedItem _locItem;
	public LocalizedItem LocItem {
		get {return _locItem;}
		set { 
			_locItem = value;
		}
	}

    public override LocalizedObject LocObject {
		get {return _locItem;}
	}

    public LocalizedItem Pickup() 
	{
		TerrainManager.Instance.RemoveObject(LocItem);
		Destroy( gameObject );
		return LocItem;
	}
	
}
