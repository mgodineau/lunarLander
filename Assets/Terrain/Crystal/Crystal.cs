using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Crystal : LocalizedObject, IinventoryItem
{
    
    public float _mass;
    public float Mass {
        get{ return _mass; }
    }
    
    
    public Crystal(Vector3 position, float mass = 1.0f) : base(position) {
        _mass = Mathf.Max(mass, 0);
    }
    

    public override GameObject createInstance()
    {
        return GameObject.Instantiate( TerrainManager.Instance.crystalPref.gameObject );
    }
}
