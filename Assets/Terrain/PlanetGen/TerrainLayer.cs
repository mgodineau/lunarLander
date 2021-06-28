using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class TerrainLayer {
    
    public abstract float GetHeight( Vector3 position );
    
    public abstract float MaxHeight {
        get;
    }
    
    public abstract void OnValidate();
    
    
}