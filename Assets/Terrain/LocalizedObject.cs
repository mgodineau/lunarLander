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
    
    
    
    public float height;
    public float rotation;
    
    public bool isGrounded;
    public bool flattenTerrain;
    

    public abstract ObjectBehaviour CreateInstance(Vector3 position);




    protected LocalizedObject() : this(Vector3.right) { }
    
    protected LocalizedObject(Vector3 position, float height, float rotation = 0) 
        : this( position, false, false, height, rotation ) {}
    
    protected LocalizedObject(Vector3 position, bool isGrounded = true, bool flattenTerrain = false, float height = 0, float rotation = 0)
    {
        Position = position;
        this.isGrounded = isGrounded;
        this.flattenTerrain = flattenTerrain;
        this.height = height;
        this.rotation = rotation;
    }

}
