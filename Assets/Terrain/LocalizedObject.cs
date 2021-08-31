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
    
    

    public abstract ObjectBehaviour CreateInstance(Vector3 position);




    protected LocalizedObject() : this(Vector3.right) { }
    
    protected LocalizedObject(Vector3 position, float height, float rotation = 0) 
        : this( position, false, height, rotation ) {}
    
    protected LocalizedObject(Vector3 position, bool isGrounded = true, float height = 0, float rotation = 0)
    {
        Position = position;
        this.height = height;
        this.rotation = rotation;
        this.isGrounded = isGrounded;
    }

}
