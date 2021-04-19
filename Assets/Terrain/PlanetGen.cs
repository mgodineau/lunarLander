using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGen : MonoBehaviour
{
    [SerializeField] private int crystalsCount = 1000;
    
    
    public List<RandomLayer> layers = new List<RandomLayer>();
    
    public List<LandingZone> landingZones = new List<LandingZone>();
    public List<Crystal> crystals = new List<Crystal>();
    
    
    private void Awake() {
        layers.Add( new RandomLayer(1, 30) );
        layers.Add( new RandomLayer(0.1f, 5) );
        
        landingZones.Add( new LZrefuel(Vector3.right) );
        landingZones.Add( new LZrefuel(Vector3.up) );
        
        
        generateCrystals();
    }
    
    
    private void generateCrystals() {
        
        for( int i=0; i<crystalsCount; i++ ) {
            crystals.Add( new Crystal( Random.onUnitSphere ) );
        }
        
    }
    
    
    public float GetHeight( Vector3 position ) {
        float height = 0;
        foreach( TerrainLayer layer in layers ) {
            height += layer.GetHeight(position);
        }
        return height;
    }
    
    
    public float getMaxHeight() {
        float maxHeight = 0;
        foreach( RandomLayer layer in layers ) {
            maxHeight += layer.MaxHeight;
        }
        return maxHeight;
    }
    
    private void OnValidate() {
        foreach( RandomLayer layer in layers ) {
            layer.OnValidate();
        }
    }
    
    
}
