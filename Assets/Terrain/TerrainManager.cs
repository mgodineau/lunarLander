using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    private static TerrainManager _instance;
    public static TerrainManager Instance {
        get{ return _instance; }
    }
    
    
    [SerializeField]
    private float _terrainWidth = 10;
    public float TerrainWidth {
        get { return _terrainWidth; }
    }
    
    [SerializeField]
    private int sampleCount = 100;
    [SerializeField]
    private int bgSampleCount = 5;
    
    [SerializeField]
    private float bottomY = -1;
    
    [SerializeField]
    private Material terrainSideMaterial;
    [SerializeField]
    private Material terrainMaterial;
    
    [SerializeField]
    private PlanetGen planet;
    
    
    private Vector3 sliceNormal = Vector3.up;
    private Vector3 sliceOrigine = Vector3.forward;
    
    
    //propriétés du terrain
    private EdgeCollider2D[] terrainColliders = new EdgeCollider2D[2];
    private Mesh terrainSideMesh;
    private Mesh terrainMesh;
    
    private Vector2[] points;
    private Vector3[] vertices;
    private Vector3[] bgVertices;
    
    private List<Vector3> renderLine;
    
    private void Awake() {
        _instance = this; //singleton
        
        //récupération du générateur le planête si il est pas connu
        if( planet == null ) {
            planet = GetComponent<PlanetGen>();
        }
        
        
        //creation des objets du terrain
        terrainSideMesh = new Mesh();
        terrainMesh = new Mesh();
        
        int terrainCount = 2;
        terrainColliders = new EdgeCollider2D[terrainCount];
        
        for( int i=0; i<terrainCount; i++ ) {
            //création de l'objet sur le côté, et ajustement de sa position
            GameObject terrain = new GameObject("terrain_" + i);
            SetTerrainParent( terrain.transform, transform, i );
            
            //ajout d'un MeshRenderer et d'un collider à l'objet
            terrain.AddComponent<MeshFilter>().mesh = terrainSideMesh;
            terrain.AddComponent<MeshRenderer>().material = terrainSideMaterial;
            terrainColliders[i] = terrain.AddComponent<EdgeCollider2D>();
            
            
            //création de l'arrière plan du terrain
            GameObject terrainBackground = new GameObject("terrainBackground_" + i);
            SetTerrainParent( terrainBackground.transform, terrain.transform, 0 );
            terrainBackground.AddComponent<MeshFilter>().mesh = terrainMesh;
            terrainBackground.AddComponent<MeshRenderer>().material = terrainMaterial;
        }
        
        //création de la géométrie du terrain et du collider
        UpdateTerrainStructure();
    }
    
    
    private void SetTerrainParent( Transform child, Transform parent, int i ) {
        child.SetParent(parent);
        child.localPosition = Vector3.zero - Vector3.right * _terrainWidth * i;
        child.localRotation = Quaternion.identity;
        child.transform.localScale = Vector3.one;
    }
    
    
    /// <summary>
    /// Créé  une nouvelle structure de terrain, en mettant à jour la géométrie du mesh et des colliders
    /// </summary>
    /// <remarks>la variable mesh doit être initialisé</remarks>
    private void UpdateTerrainStructure() {
        points = new Vector2[sampleCount+1];
        vertices = new Vector3[(sampleCount+1) * 2];
        
        for( int i=0; i<=sampleCount; i++ ) {
            
            //création des points du collider et du mesh
            float x = _terrainWidth * i / sampleCount;
            points[i] = new Vector2(  x, 1 );
            vertices[i*2] = new Vector3(x, 1, 0);
            vertices[i*2 + 1] = new Vector3(x, bottomY, 0);
            
        }
        
        
        //création des triangles
        int[] triangles = new int[sampleCount * 3 * 2];
        for( int i=0; i<sampleCount; i++) {
            triangles[i*6] = i*2 + 1;
            triangles[i*6+1] = i*2;
            triangles[i*6+2] = i*2 + 2;
            
            triangles[i*6+3] = i*2 + 1;
            triangles[i*6+4] = i*2 + 2;
            triangles[i*6+5] = i*2 + 3;
        }
        
        //affectation de la géométrie au Mesh
        terrainSideMesh.vertices = vertices;
        terrainSideMesh.triangles = triangles;
        
        
        //création de la géométrie de l'arrière plan
        
        //création des vertices
        bgVertices = new Vector3[ (sampleCount+1) * (bgSampleCount+1)];
        for( int z=0; z<=bgSampleCount; z++ ) {
            float realZ = _terrainWidth * z / sampleCount;
            for( int x=0; x<=sampleCount; x++ ) {
                bgVertices[ z*(sampleCount+1) + x ] = new Vector3( _terrainWidth * x / sampleCount, 0, realZ );
            }
        }
        //création des triangles
        int[] bgTriangles = new int[sampleCount * bgSampleCount * 3 * 2];
        for( int i=0; i<bgSampleCount; i++ ) {
            for( int j=0; j<sampleCount; j++ ) {
                bgTriangles[ (i*sampleCount + j) * 6] = i*(sampleCount+1) + j;
                bgTriangles[ (i*sampleCount + j) * 6 + 1] = (i+1)*(sampleCount+1) + j;
                bgTriangles[ (i*sampleCount + j) * 6 + 2] = i*(sampleCount+1) + (j+1);
                
                bgTriangles[ (i*sampleCount + j) * 6 + 3] = (i+1)*(sampleCount+1) + (j+1);
                bgTriangles[ (i*sampleCount + j) * 6 + 4] = i*(sampleCount+1) + (j+1);
                bgTriangles[ (i*sampleCount + j) * 6 + 5] = (i+1)*(sampleCount+1) + j;
            }
        }
        terrainMesh.vertices = bgVertices;
        terrainMesh.triangles = bgTriangles;
        
        
        //MAJ des données de hauteur du terrain
        UpdateTerrain();
    }
    
    
    /// <summary>
    /// Met à jours la géométrie du mesh et du collider des terrains
    /// </summary>
    /// <remarks>les tableaux points, vertices et bgVertices doivent être initialisés, ainsi que mesh</remarks>
    private void UpdateTerrain() {
        
        for( int i=0; i<=sampleCount; i++ ) {
            //récupération de la hauteur
            float ratio = (float) i / sampleCount;
            float angle = 360.0f * ratio;
            Vector3 samplePosition = Quaternion.AngleAxis( angle, sliceNormal ) * sliceOrigine;
            float sample = planet.GetHeight(samplePosition);
            
            //création des points du collider et du mesh
            float x = _terrainWidth * ratio;
            points[i].y = sample;
            vertices[i*2].y = sample;
            
        }
        
        
        terrainSideMesh.vertices = vertices;
        terrainSideMesh.RecalculateBounds();
        foreach( EdgeCollider2D col in terrainColliders ) {
            col.points = points;
        }
        
        
        
        //création de la hauteur de l'arrière plan
        for( int z=0; z<=bgSampleCount; z++) {
            for( int x=0; x<=sampleCount; x++ ) {
                float angleZ = 360.0f * x / sampleCount;
                float angleX = 360.0f * z / sampleCount;
                Vector3 samplePosition = Quaternion.AngleAxis( angleZ, sliceNormal ) * sliceOrigine;
                samplePosition = Quaternion.AngleAxis( angleX, Vector3.Cross(samplePosition, sliceNormal) ) * samplePosition;
                bgVertices[ z*(sampleCount+1) + x ].y = planet.GetHeight(samplePosition);
            }
            // bgVertices[ (z+1)*sampleCount ].y = bgVertices[z * sampleCount].y;
        }
        terrainMesh.vertices = bgVertices;
        terrainMesh.RecalculateBounds();
        terrainMesh.RecalculateNormals();
        
        
        
        //MAJ du rendu de la surface du terrain
        WireframeRender.Instance.linePaths.Remove(renderLine);
        renderLine = new List<Vector2>(points).ConvertAll( v2 => new Vector3(v2.x - _terrainWidth, v2.y, 0) );
        renderLine.AddRange( renderLine.ConvertAll( pos => new Vector3(pos.x + _terrainWidth, pos.y, pos.z ) ) );
        WireframeRender.Instance.linePaths.Add(renderLine);
    }
    
    
    
    
    
    /// <summary>
    /// Effectue une rotation du plan d'évolution du jeu, et met à jour les objets du terrain.
    /// </summary>
    /// <param name="axis2dPosition">la position x de l'axe de rotation, dans le repère 2D (ex : la position du joueur)</param>
    /// <param name="angle">L'angle de rotation, en degré</param>
    public void RotateAround( float axis2dPosition, float angle ) {
        axis2dPosition = axis2dPosition % _terrainWidth;
        Vector3 axis = Quaternion.AngleAxis( 360.0f * axis2dPosition/_terrainWidth, sliceNormal ) * sliceOrigine;
        
        RotateAround( axis, angle );
    }
    
    /// <summary>
    /// Effectue une rotation du plan d'évolution du jeu, et met à jour les objets du terrain.
    /// </summary>
    /// <param name="rotationAxis">L'axe de rotation, dans le repère globale (non nul)</param>
    /// <param name="angle">L'angle de rotation, en degré</param>
    public void RotateAround( Vector3 rotationAxis, float angle ) {
        Quaternion rotation = Quaternion.AngleAxis( angle, rotationAxis );
        sliceNormal = rotation * sliceNormal;
        sliceOrigine = rotation * sliceOrigine;
        
        UpdateTerrain();
    }
    
    
    
    
}
