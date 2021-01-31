using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    [SerializeField]
    private PlanetGen planet;
    
    public Vector3 sliceNormal = Vector3.up;
    private Vector3 sliceOrigine = Vector3.forward;
    
    [SerializeField]
    private float terrainWidth = 10;
    [SerializeField]
    private int sampleCount = 100;
    
    [SerializeField]
    private List<GameObject> terrainObjects = new List<GameObject>();
    private List<EdgeCollider2D> terrainColliders = new List<EdgeCollider2D>();
    private List<MeshFilter> terrainMesh = new List<MeshFilter>();
    
    [SerializeField]
    private float bottomY = -1;
    
    
    private void Awake() {
        if( planet == null ) {
            planet = GetComponent<PlanetGen>();
        }
        foreach( GameObject terrain in terrainObjects ) {
            terrainColliders.Add( terrain.GetComponent<EdgeCollider2D>() );
            terrainMesh.Add( terrain.GetComponent<MeshFilter>() );
        }
    }
    
    private void Start()
    {
        UpdateTerrain();
    }
    
    
    private void UpdateTerrain() {
        Vector2[] points = new Vector2[sampleCount+1];
        Vector3[] vertices = new Vector3[(sampleCount+1) * 2];
        
        
        for( int i=0; i<sampleCount; i++ ) {
            //récupération de la hauteur
            float ratio = (float) i / sampleCount;
            float angle = 360.0f * ratio;
            Vector3 samplePosition = Quaternion.AngleAxis( angle, sliceNormal ) * sliceOrigine;
            float sample = planet.GetHeight(samplePosition);
            
            //création des points du collider et du mesh
            float x = terrainWidth * ratio;
            points[i] = new Vector2(  x, sample );
            vertices[i*2] = new Vector3(x, sample, 0);
            vertices[i*2 + 1] = new Vector3(x, bottomY, 0);
            
        }
        points[sampleCount] = new Vector2( terrainWidth, points[0].y );
        vertices[sampleCount*2] = new Vector3( terrainWidth, 0, vertices[0].x );
        vertices[sampleCount*2+1] = new Vector3( terrainWidth, bottomY, 0 );
        
        
        int[] faces = new int[sampleCount * 3 * 2]; //TODO changer en triangles
        for( int i=0; i<sampleCount; i++) {
            faces[i*6] = i*2 + 1;
            faces[i*6+1] = i*2;
            faces[i*6+2] = i*2 + 2;
            
            faces[i*6+3] = i*2 + 1;
            faces[i*6+4] = i*2 + 2;
            faces[i*6+5] = i*2 + 3;
        }
        
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = faces;
        
        foreach( EdgeCollider2D col in terrainColliders ) {
            col.points = points;
        }
        
        foreach( MeshFilter filter in terrainMesh ) {
            filter.mesh = mesh;
        }
        
    }
    
    
    
}
