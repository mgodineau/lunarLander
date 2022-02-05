using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGen : MonoBehaviour
{
    [SerializeField] private int crystalsCount = 1000;
    [SerializeField] private int craterCount = 5;
    
    public List<TerrainLayer> layers = new List<TerrainLayer>();
    
    private List<LandingZone> landingZones = new List<LandingZone>();
    private List<LocalizedItem> items = new List<LocalizedItem>();
    
    
    private void Awake() {
        layers.Add( new RandomLayer(1, 30) );
        layers.Add( new RandomLayer(0.1f, 5) );
        
        for( int i=0; i<craterCount; i++ ) {
            layers.Add( new Crater(Random.onUnitSphere, Random.Range(0.5f, 1.0f)) );
        }
        
        landingZones.Add( new LZrefuel(Vector3.right) );
        landingZones.Add( new LZrefuel(Vector3.up) );
        landingZones.Add( new LZradar(Vector3.forward) );
        
        generateCrystals();
    }
    
    
    internal void RemoveObject(LocalizedObject obj)
    {
        if( obj is LocalizedItem ) {
            RemoveItem( obj as LocalizedItem );
        }
    }
    
    public void RemoveItem(LocalizedItem item)
    {
        items.Remove(item);
    }
    
    public void RemoveItem(InventoryItem item)
    {
        foreach( LocalizedItem locItem in items) {
            if( locItem.Item == item ) {
                RemoveItem(locItem);
                break;
            }
        }
    }
    
    public void AddItem(InventoryItem item, Vector3 spherePosition)
    {
        items.Add( new LocalizedItem(item, spherePosition) );
    }
    
    public void AddItem(InventoryItem item, Vector3 spherePosition, float height, float rotation=0)
    {
        items.Add( new LocalizedItem(item, spherePosition, height, rotation) );
    }
    
    
    public List<LocalizedObject> getObjects() {
        List<LocalizedObject> objects = new List<LocalizedObject>();
        objects.AddRange(landingZones);
        objects.AddRange( items );
        
        return objects;
    }
    
    public List<LandingZone> getLandingZones() {
        return landingZones;
    }
    
    public List<LocalizedItem> GetItems() {
        return items;
    }
    
    private void generateCrystals() {
        
        for( int i=0; i<crystalsCount; i++ ) {
            AddItem( new Crystal(), UnityEngine.Random.onUnitSphere  );
        }
        
    }
    
    
    public float GetHeight( Vector3 position ) {
        float height = 0;
        foreach( TerrainLayer layer in layers ) {
            height += layer.GetHeight(position);
        }
        return height;
    }
    
    
    public float GetMaxHeight() {
        float maxHeight = -Mathf.Infinity;
        foreach( TerrainLayer layer in layers ) {
            maxHeight = Mathf.Max(layer.MaxHeight);
        }
        return maxHeight * 1.5f; //TODO trouver une meilleure méthode
    }
    
    
    internal float GetMinHeight()
    {
        float minHeight = Mathf.Infinity;
        foreach( TerrainLayer layer in layers ) {
            minHeight = Mathf.Min(layer.MinHeight);
        }
        return minHeight;
    }
    
    
    private void OnValidate() {
        foreach( TerrainLayer layer in layers ) {
            layer.OnValidate();
        }
    }

    
}
