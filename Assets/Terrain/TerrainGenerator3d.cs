using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator3d : MonoBehaviour
{
    
    
    private void Start() {
        GetComponent<MeshFilter>().mesh = CreateTerrain(10, 1);
    }
    
    
    public static Mesh CreateTerrain( int resolution, float size ) {
        Mesh mesh = new Mesh();
        
        int count = (int)(resolution * size);
        float tileSize = 1.0f/resolution;
        
        //création des vertices
        Vector3[] vertices = new Vector3[count*count];
        
        for( int i=0; i<count; i++ ) {
            for( int j=0; j<count; j++ ) {
                float x = i * tileSize;
                float z = j * tileSize;
                vertices[i * count + j] = new Vector3( x, Mathf.PerlinNoise(x, z), z );
            }
        }
        
        
        //remplissage avec des triangles
        int[] triangles = new int[ (count-1) * (count-1) * 2 * 3];
        for( int i=1; i<count; i++ ) {
            for( int j=1; j<count; j++ ) {
                int squareId = ((i-1) * (count-1)) + j-1;
                
                //premier triangle
                triangles[squareId*6] = (i-1)*count + j-1;
                triangles[squareId*6+1] = (i-1)*count + j;
                triangles[squareId*6+2] = i*count + j-1;
                
                //second triangle
                triangles[squareId*6+3] = (i)*count + j;
                triangles[squareId*6+4] = i*count + j-1;
                triangles[squareId*6+5] = (i-1)*count + j;
            }
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        
        mesh.RecalculateNormals();
        return mesh;
    }
    
    
    
}
