using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopPosition : MonoBehaviour
{
    
    private float terrainWidth;
    private float halfWidth;
    
    private void Start() {
        terrainWidth = TerrainManager.Instance.TerrainWidth;
        halfWidth = terrainWidth/2;
    }
    
    private void LateUpdate() {
        
        float x = transform.position.x;
        if( x <= -halfWidth ) {
            x += terrainWidth;
        } else if ( x > halfWidth ) {
            x -= terrainWidth;
        }
        
        if( transform.position.x != x ) {
            transform.position = new Vector3( x, transform.position.y, transform.position.z );
        }
    }
    
}
