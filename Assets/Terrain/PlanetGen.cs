using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGen : MonoBehaviour
{
    public List<RandomLayer> layers = new List<RandomLayer>();
    
    
    private void Awake() {
        layers.Add( new RandomLayer(0.5f, 1f) );
    }
    
    public float GetHeight( Vector3 position ) {
        float height = 0;
        foreach( TerrainLayer layer in layers ) {
            height += layer.GetHeight(position);
        }
        return height;
    }
    
    
    private void OnValidate() {
        foreach( RandomLayer layer in layers ) {
            layer.OnValidate();
        }
    }
    
    
}
