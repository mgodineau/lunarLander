using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Crystal : LocalizedObject
{
    
    public float mass = 10.0f;
    
    public Crystal(Vector3 position) : base(position) {
        
    }

    public override GameObject createInstance()
    {
        return GameObject.Instantiate( TerrainManager.Instance.crystalPref.gameObject );
    }
}
