using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainDebug : MonoBehaviour
{
    
    [SerializeField][Range(0,10)]
    private float lineLength = 10.0f;
    
    public PlanetGen planetGen;
    public TerrainManager terrainManager;
    public Transform debugLight;
    public Transform lander;
    public Transform slice;
    
    private MeshFilter meshFilter;
    private Mesh mesh;
    
    private Vector3 userDir = Vector3.one;
    private List<Vector3> LZsDir = new List<Vector3>();
    
    private void Awake() {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        
    }
    
    private void Start() {
        UpdateMesh();
        UpdateLight();
        UpdateLZs();
        
        UpdateLanderPos();
    }
    
    private void Update() {
        UpdateLanderPos();
        
        //MAJ de la coupe
        slice.rotation = Quaternion.LookRotation( terrainManager.SliceNormal, Vector3.up ) * Quaternion.Euler( 90, 0, 0 );
        
        //affichage du lander
        Debug.DrawLine(transform.position, transform.position + userDir * lineLength, Color.green);
        
        //affichage des lz
        foreach( Vector3 dir in LZsDir ) {
            Debug.DrawLine( transform.position, transform.position + dir * lineLength, Color.red );
        }
    }
    
    
    
    
    public void UpdateMesh() {
        Vector3[] vertices = mesh.vertices;
        float ratio = Mathf.PI / terrainManager.TerrainWidth;
        
        for( int i=0; i<vertices.Length; i++ ) {
            vertices[i].Normalize();
            vertices[i] *= 1 + planetGen.GetHeight( vertices[i] ) * ratio;
        }
        
        
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        
        meshFilter.mesh = mesh;
    }
    
    public void UpdateLight() {
        debugLight.rotation = Quaternion.LookRotation( terrainManager.globalLightDir, Vector3.up );
    }
    
    public void UpdateLZs() {
        LZsDir = new List<Vector3>();
        foreach( LandingZone lz in planetGen.landingZones ) {
            LZsDir.Add( lz.Position.normalized );
        }
    }
    
    public void UpdateLanderPos() {
        userDir = terrainManager.convertXtoDir( lander.position.x );
    }
    
}
