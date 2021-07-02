using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class WireframeEffect : MonoBehaviour
{

    private static Dictionary<Mesh, List<List<Vector3>>> meshToLinesLocal = new Dictionary<Mesh, List<List<Vector3>>>();

    private List<List<Vector3>> linesLocal;
    private List<LineData> linesGlobal;

    [SerializeField]
    private bool removeDiagonals = true;
    [SerializeField]
    private float diagThreshold = 1.0f;
    [SerializeField]
    private Color color = Color.white;

    private MeshFilter meshFilter;


    private delegate void UpdateProcessing();
    private UpdateProcessing currentProcessing = null;


    private void Awake()
    {

        meshFilter = GetComponent<MeshFilter>();

        linesLocal = new List<List<Vector3>>();
        linesGlobal = new List<LineData>();
        UpdateLinesFromMesh();


        if (gameObject.isStatic)
        {
            currentProcessing = EmptyMethod;
        }
        else
        {
            currentProcessing = UpdateGlobalLines;
        }
    }


    private void Start()
    {
        EnableWires();
    }
    
    
    
    private void OnBecameVisible() {
        EnableWires();
        enabled = true;
    }
    
    private void OnBecameInvisible() {
        DisableWires();
        enabled = false;
    }
    
    
    private void OnDisable()
    {
        DisableWires();
    }

    private void OnEnable()
    {
        EnableWires();
    }
    
    
    private void EnableWires() {
        WireframeRender renderInstance = WireframeRender.Instance;
        if (renderInstance != null)
        {
            foreach (LineData line in linesGlobal)
            {
                renderInstance.linesGeometry.Add(line);
            }
        }
    }
    
    private void DisableWires() {
        WireframeRender renderInstance = WireframeRender.Instance;
        if (renderInstance != null)
        {
            foreach (LineData line in linesGlobal)
            {
                renderInstance.linesGeometry.Remove(line);
            }
        }
    }
    
    
    

    private void Update()
    {
        currentProcessing();
    }


    private void EmptyMethod() { }

    private void UpdateGlobalLines()
    {
        for (int i = 0; i < linesLocal.Count; i++)
        {
            for (int j = 0; j < linesLocal[i].Count; j++)
            {
                linesGlobal[i].points[j] = transform.TransformPoint(linesLocal[i][j]);
            }
        }
    }


    /// <summary>
    /// Nécessite de pouvoir lire le mesh
    /// </summary>
    public void UpdateLines()
    {
        DisableWires();
        
        UpdateLinesFromMesh();
        UpdateGlobalLines();
        
        EnableWires();
    }

    private void UpdateLinesFromMesh()
    {

        linesLocal = ExtractLinesSimple(meshFilter.sharedMesh);
        linesGlobal = linesLocal.ConvertAll(line => new LineData(line.ConvertAll(vLocal => transform.TransformPoint(vLocal)), color));
    }


    private List<List<Vector3>> ExtractLinesSimple(Mesh mesh)
    {
        if (meshToLinesLocal.ContainsKey(mesh))
        {
            return meshToLinesLocal[mesh];
        }
        
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
                            if( new List<Edge>(triangles[j].GetEdges()).Contains(edge) && Vector3.Angle(triangles[i].GetNormal(), triangles[j].GetNormal()) <= diagThreshold ) {
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
        meshToLinesLocal.Add(mesh, linesPath);
        
        foreach( Edge edge in displayedEdges ) {
            List<Vector3> line = new List<Vector3>();
            line.Add(edge.vertex_0.position);
            line.Add(edge.vertex_1.position);
            linesPath.Add(line);
        }
        
        return linesPath;
    }


    private List<List<Vector3>> ExtractLinesOptimized(Mesh mesh)
    {
        if (meshToLinesLocal.ContainsKey(mesh))
        {
            return meshToLinesLocal[mesh];
        }

        List<List<Vector3>> linesPath = new List<List<Vector3>>();

        BetterMesh betterMesh = new BetterMesh(mesh);

        // HashSet<Edge> addedEdges = new HashSet<Edge>();
        HashSet<Edge> displayedEdges = GetDisplayedEdge(betterMesh);

        Edge currentEdge = betterMesh.Edges[0];
        Vertex previousVertex = currentEdge.vertex_0;   // la dernière vertex ajoutée à la currentLine

        List<Vector3> currentLine = new List<Vector3>();
        currentLine.Add(previousVertex.position);
        linesPath.Add(currentLine);

        while (displayedEdges.Count > 0)
        {

            //ajout de l'edge courrante
            // addedEdges.Add(currentEdge);
            displayedEdges.Remove(currentEdge);
            previousVertex = previousVertex == currentEdge.vertex_0 ? currentEdge.vertex_1 : currentEdge.vertex_0;
            currentLine.Add(previousVertex.position);


            //sélection de l'edge suivante à ajouter
            bool continueLine = false;
            foreach (Edge nextEdge in betterMesh.Edges)
            {
                if (nextEdge.ContainsVertex(previousVertex) && displayedEdges.Contains(nextEdge))
                {
                    currentEdge = nextEdge;
                    continueLine = true;
                    break;
                }
            }

            if (!continueLine && displayedEdges.Count != 0)
            {
                HashSet<Edge>.Enumerator enumerator = displayedEdges.GetEnumerator();
                enumerator.MoveNext();
                currentEdge = enumerator.Current;
                enumerator.Dispose();
                
                previousVertex = currentEdge.vertex_0;

                currentLine = new List<Vector3>();
                currentLine.Add(previousVertex.position);
                linesPath.Add(currentLine);
            }


        }


        meshToLinesLocal.Add(mesh, linesPath);

        return linesPath;
    }



    private HashSet<Edge> GetDisplayedEdge(BetterMesh mesh)
    {
        if (!removeDiagonals)
        {
            return new HashSet<Edge>(mesh.Edges);
        }


        HashSet<Edge> previousEdges = new HashSet<Edge>();
        HashSet<Edge> displayedEdges = new HashSet<Edge>();

        List<Triangle> triangles = mesh.Triangles;
        for (int i = 0; i < triangles.Count; i++)
        {

            foreach (Edge edge in triangles[i].GetEdges())
            {
                if (!previousEdges.Add(edge))
                {
                    bool ignore = false;
                    for (int j = i + 1; j < triangles.Count && !ignore; j++)
                    {
                        //suppression des edges qui sont sur d'autres triangles parallèles
                        if (new List<Edge>(triangles[j].GetEdges()).Contains(edge) && Vector3.Angle(triangles[i].GetNormal(), triangles[j].GetNormal()) <= diagThreshold)
                        {
                            ignore = true;
                        }
                    }
                    if (!ignore)
                    {
                        displayedEdges.Add(edge);
                    }

                }
            }

        }

        return displayedEdges;
    }



    private bool linesEquals(int[] line1, int[] line2)
    {
        return line1[0] == line2[0] && line1[1] == line2[1]
        || line1[0] == line2[1] && line1[1] == line2[0];
    }

}
