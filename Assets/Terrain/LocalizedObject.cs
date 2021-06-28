using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class LocalizedObject
{
    
    private Vector3 _position;
    public Vector3 Position
    {
        get { return _position; }
        set { _position = value.normalized; }
    }
    
    
    public abstract GameObject CreateInstance( Vector3 position, Quaternion rotation, Transform parent );
    
    
    protected LocalizedObject() : this(Vector3.right) {}
    
    protected LocalizedObject( Vector3 position ) {
        Position = position;
    }
    
}
