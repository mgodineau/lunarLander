using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crater : TerrainLayer
{
    
    private Vector3 center;     // vecteur unitaire correspondant au centre du cratère
    private float radius;       // rayon du cratère, en radian, sur une sphère unitaire
    private float outerRadius;
    
    private float height;

    public override float MaxHeight {
        get{ return height * 0.5f; }
    }
    
    public override float MinHeight {
        get{ return -height * 0.5f; }
    }
    
    public override float GetHeight(Vector3 position)
    {
        float localAngle = Vector3.Angle( position, center ) * Mathf.Deg2Rad;
        
        if( localAngle < radius ) {
            return (Mathf.Pow(localAngle/radius, 2)-0.5f) * height;
        }
        if( localAngle < outerRadius ) {
            return Mathf.Pow(1 - (localAngle-radius) / (outerRadius-radius), 2 ) * height * 0.5f;
        }
        
        return 0;
    }
    
    public override void OnValidate() {
        
    }
    
    
    
    public Crater( Vector3 center, float radius = Mathf.PI/4 ) {
        this.center = center.normalized;
        this.radius = Mathf.Clamp(radius, 0, Mathf.PI);
        
        height = radius * 50;
        outerRadius = radius * 1.5f;
        
    }
    
    
    
    
}
