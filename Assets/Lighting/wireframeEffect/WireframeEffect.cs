using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class WireframeEffect : MonoBehaviour
{
    
    
    private List<List<Vector3>> linesLocal;
    private List<List<Vector3>> linesGlobal;
    
    [SerializeField]
    private bool removeDiagonals = true;
    private MeshFilter meshFilter;
    
    private void Awake() {
        
        meshFilter = GetComponent<MeshFilter>(); 
        
        linesLocal = new List<List<Vector3>>();
        linesGlobal = new List<List<Vector3>>();
        UpdateLinesFromMesh();
    }


    private void Start() {
        WireframeRender.Instance.linePaths.AddRange(linesGlobal);
    }
    
    private void OnDisable() {
        foreach( List<Vector3> line in linesGlobal ) {
        WireframeRender.Instance.linePaths.Remove(line);
        }
    }
    
    
    private void Update() {
        
        for( int i=0; i<linesLocal.Count; i++ ) {
            for( int j=0; j<linesLocal[i].Count; j++ ) {
                linesGlobal[i][j] = transform.TransformPoint( linesLocal[i][j] );
            }
        }
        
    }
    
    /// <summary>
    /// Nécessite de pouvoir lire le mesh
    /// </summary>
    public void UpdateLines() {
        UpdateLinesFromMesh();
        WireframeRender.Instance.linePaths.AddRange(linesGlobal);
    }
    
    private void UpdateLinesFromMesh() {
        
        foreach( List<Vector3> path in linesGlobal ) {
            WireframeRender.Instance.linePaths.Remove( path );
        }
        linesLocal = ExtractLines( meshFilter.sharedMesh );
        linesGlobal = linesLocal.ConvertAll( line => line.ConvertAll(vLocal => transform.TransformPoint(vLocal)) );
    }
    
    
    private List<List<Vector3>> ExtractLines(Mesh mesh)
    {
        BetterMesh betterMesh = new BetterMesh(mesh);
        List<Edge> previousEdges = new List<Edge>();
        List<Edge> displayedEdges = new List<Edge>();
        
        List<Triangle> triangles = betterMesh.Triangles;
        for( int i=0; i<triangles.Count; i++) {
            
            foreach( Edge edge in triangles[i].GetEdges() ) {
                if( !previousEdges.Contains( edge ) ) {
                    if (removeDiagonals) {
                        bool ignore = false;
                        for( int j=i+1; j<triangles.Count && !ignore; j++ ) {
                            //suppression des edges qui sont sur d'autres triangles parallèles
                            if( new List<Edge>(triangles[j].GetEdges()).Contains(edge) && Vector3.Angle(triangles[i].GetNormal(), triangles[j].GetNormal()) <= 0.01f ) {
                                ignore = true;
                            }
                        }
                        if( !ignore ) {
                            displayedEdges.Add(edge);
                        }
                    } else {
                        displayedEdges.Add(edge);
                    }
                }
                previousEdges.Add(edge);
            }
            
        }
        
        List<List<Vector3>> linesPath = new List<List<Vector3>>();
        foreach( Edge edge in displayedEdges ) {
            List<Vector3> line = new List<Vector3>();
            line.Add(edge.vertex_0.position);
            line.Add(edge.vertex_1.position);
            linesPath.Add(line);
        }
        
        return linesPath;
    }
    
    
    private bool linesEquals( int[] line1, int [] line2 ) {
        return line1[0] == line2[0] && line1[1] == line2[1]
        ||line1[0] == line2[1] && line1[1] == line2[0];
    }
    
}
