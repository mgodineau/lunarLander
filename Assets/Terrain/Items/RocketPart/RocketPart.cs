using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPart : InventoryItem
{
	
	private const float density = 1;
	
	
	public override float Volume {
		get {return _mass / density;}
	}
	
	private float _mass;
	public override float Mass {
		get {return _mass;}
	}

	private string _name;
	public override string Name {
		get {return _name;}
	}
	
	
	public RocketPart() 
		: this(1.0f, "rocket part")
	{}
	
	public RocketPart( float mass, string name ) {
		_mass = mass;
		_name = name;
	}
	
	
	public override ItemBehaviour InstantiateWorldItem(LocalizedItem locItem)
	{
		GameObject instance = GameObject.Instantiate( TerrainManager.Instance.Prefabs.CratePref.gameObject );
        ItemBehaviour behaviour = instance.GetComponent<ItemBehaviour>();
        behaviour.LocItem = locItem;
        
        return behaviour;
	}
}
