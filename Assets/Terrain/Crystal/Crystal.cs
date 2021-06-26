using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Crystal : LocalizedObject, IinventoryItem
{
    
    private const float density = 1;
    
    private float _mass;
    public float Mass {
        get{ return _mass; }
    }
    
    public float Volume {
        get { return Mass / density; }
    }
    
    public string Name {
        get{ return "Crystal"; }
    }
    

    public Crystal(Vector3 position, float mass = 1.0f) : base(position) {
        _mass = Mathf.Max(mass, 0);
    }
    

    public override GameObject CreateInstance( Vector3 position, Quaternion rotation, Transform parent )
    {
        GameObject instance = GameObject.Instantiate( TerrainManager.Instance.crystalPref.gameObject, position, rotation, parent );
        instance.AddComponent<CrystalBehaviour>().crystalScript = this;
        return instance;
    }
}
