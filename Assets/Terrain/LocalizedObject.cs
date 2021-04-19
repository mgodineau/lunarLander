using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LocalizedObject : ScriptableObject
{
    
    public Vector3 Position
    {
        get { return _position; }
        set { _position = value.normalized; }
    }
    private Vector3 _position;
    
    
    public abstract GameObject createInstance();
    
    
    protected LocalizedObject() : this(Vector3.right) {}
    
    protected LocalizedObject( Vector3 position ) {
        Position = position;
    }
    
}
