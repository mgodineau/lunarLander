using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectBehaviour : MonoBehaviour
{
    
    public abstract LocalizedObject LocObject {
        get;
    }
    
    
    [SerializeField]
    private float _groundOffset = 0;
    public float GroundOffset {
        get {return _groundOffset;}
        set {
            _groundOffset = Mathf.Max(0, value);
        }
    }
    
    
    public void SetPosition( Vector3 position ) {
        
        position.y += _groundOffset;
        transform.position = position;
    }
    
    
}
