using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionEffect : MonoBehaviour
{
    
    [SerializeField] List<Rigidbody2D> parts = new List<Rigidbody2D>();
    
    public void InitEffect( Rigidbody2D source ) {
        
        transform.position = source.transform.position;
        transform.rotation = source.transform.rotation;
        transform.localScale = source.transform.localScale;
        
        foreach( Rigidbody2D part in parts ) {
            part.velocity = source.velocity;
            part.angularVelocity = source.angularVelocity;
            
        }
        
    }
    
    
}
