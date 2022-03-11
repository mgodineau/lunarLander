using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter))]
public class WireframeEffect : MonoBehaviour
{
	
	private static HashSet<WireframeEffect> registeredWireframeObjects = new HashSet<WireframeEffect>();
	
	private static Dictionary<Mesh, Mesh> meshToWireframe = new Dictionary<Mesh, Mesh>();
	
	[SerializeField]
	private bool removeDiagonals = true;
	[SerializeField]
	private float diagThreshold = 1.0f;
	[SerializeField]
	private Color color = Color.white;
	
	
	private MeshFilter meshFilter;
	private Mesh wireframeMesh;
	

	private void Awake()
	{
		wireframeMesh = new Mesh();
		
		
		meshFilter = GetComponent<MeshFilter>();

		UpdateLinesFromMesh();
	}
	
	
	
	private void OnEnable()
	{
		registeredWireframeObjects.Add(this);
	}
	
	private void OnDisable()
	{
		registeredWireframeObjects.Remove(this);
	}

	
	
	
	public static void DrawAllNow() {
		foreach( WireframeEffect effect in registeredWireframeObjects ) {
			effect.DrawNow();
		}
	}
	
	
	private void DrawNow() {
		
		Graphics.DrawMeshNow(wireframeMesh, transform.localToWorldMatrix, 0);
	}
	
	
	
	/// <summary>
	/// Nécessite de pouvoir lire le mesh
	/// </summary>
	public void UpdateLines()
	{
		UpdateLinesFromMesh();
	}
	
	private void OnRenderObject() {
		//Graphics.DrawMeshNow(wireframeMesh, transform.localToWorldMatrix, 0);
	}
	
	
	private void UpdateLinesFromMesh()
	{
		Mesh mesh = meshFilter.mesh;
		if( meshToWireframe.ContainsKey(mesh) ) {
			wireframeMesh = meshToWireframe[mesh];
			return;
		}
		
		//TODO
		Vector3[] vertices = meshFilter.mesh.vertices;
		int[] triangles = new int[vertices.Length - vertices.Length % 3 ];
		for( int i=0; i<triangles.Length; i++ ) {
			triangles[i] = i;
		}
		
		wireframeMesh.SetVertices( vertices );
		wireframeMesh.SetTriangles(triangles, 0);
		
		SubMeshDescriptor[] desc = {new SubMeshDescriptor(0, triangles.Length, MeshTopology.Lines)};
		wireframeMesh.SetSubMeshes( desc );
	}


	// private List<List<Vector3>> ExtractLinesSimple(Mesh mesh)
	// {
	// 	if (meshToLinesLocal.ContainsKey(mesh))
	// 	{
	// 		return meshToLinesLocal[mesh];
	// 	}
		
	// 	BetterMesh betterMesh = new BetterMesh(mesh);
	// 	List<Edge> previousEdges = new List<Edge>();
	// 	List<Edge> displayedEdges = new List<Edge>();
		
	// 	List<Triangle> triangles = betterMesh.Triangles;
	// 	for( int i=0; i<triangles.Count; i++) {
			
	// 		foreach( Edge edge in triangles[i].GetEdges() ) {
	// 			if( !previousEdges.Contains( edge ) ) {
	// 				if (removeDiagonals) {
	// 					bool ignore = false;
	// 					for( int j=i+1; j<triangles.Count && !ignore; j++ ) {
	// 						//suppression des edges qui sont sur d'autres triangles parallèles
	// 						if( new List<Edge>(triangles[j].GetEdges()).Contains(edge) && Vector3.Angle(triangles[i].GetNormal(), triangles[j].GetNormal()) <= diagThreshold ) {
	// 							ignore = true;
	// 						}
	// 					}
	// 					if( !ignore ) {
	// 						displayedEdges.Add(edge);
	// 					}
	// 				} else {
	// 					displayedEdges.Add(edge);
	// 				}
	// 			}
	// 			previousEdges.Add(edge);
	// 		}
			
	// 	}
		
		
	// 	List<List<Vector3>> linesPath = new List<List<Vector3>>();
	// 	meshToLinesLocal.Add(mesh, linesPath);
		
	// 	foreach( Edge edge in displayedEdges ) {
	// 		List<Vector3> line = new List<Vector3>();
	// 		line.Add(edge.vertex_0.position);
	// 		line.Add(edge.vertex_1.position);
	// 		linesPath.Add(line);
	// 	}
		
	// 	return linesPath;
	// }


	// private List<List<Vector3>> ExtractLinesOptimized(Mesh mesh)
	// {
	// 	if (meshToLinesLocal.ContainsKey(mesh))
	// 	{
	// 		return meshToLinesLocal[mesh];
	// 	}

	// 	List<List<Vector3>> linesPath = new List<List<Vector3>>();

	// 	BetterMesh betterMesh = new BetterMesh(mesh);

	// 	// HashSet<Edge> addedEdges = new HashSet<Edge>();
	// 	HashSet<Edge> displayedEdges = GetDisplayedEdge(betterMesh);

	// 	Edge currentEdge = betterMesh.Edges[0];
	// 	Vertex previousVertex = currentEdge.vertex_0;   // la dernière vertex ajoutée à la currentLine

	// 	List<Vector3> currentLine = new List<Vector3>();
	// 	currentLine.Add(previousVertex.position);
	// 	linesPath.Add(currentLine);

	// 	while (displayedEdges.Count > 0)
	// 	{

	// 		//ajout de l'edge courrante
	// 		// addedEdges.Add(currentEdge);
	// 		displayedEdges.Remove(currentEdge);
	// 		previousVertex = previousVertex == currentEdge.vertex_0 ? currentEdge.vertex_1 : currentEdge.vertex_0;
	// 		currentLine.Add(previousVertex.position);


	// 		//sélection de l'edge suivante à ajouter
	// 		bool continueLine = false;
	// 		foreach (Edge nextEdge in betterMesh.Edges)
	// 		{
	// 			if (nextEdge.ContainsVertex(previousVertex) && displayedEdges.Contains(nextEdge))
	// 			{
	// 				currentEdge = nextEdge;
	// 				continueLine = true;
	// 				break;
	// 			}
	// 		}

	// 		if (!continueLine && displayedEdges.Count != 0)
	// 		{
	// 			HashSet<Edge>.Enumerator enumerator = displayedEdges.GetEnumerator();
	// 			enumerator.MoveNext();
	// 			currentEdge = enumerator.Current;
	// 			enumerator.Dispose();
				
	// 			previousVertex = currentEdge.vertex_0;

	// 			currentLine = new List<Vector3>();
	// 			currentLine.Add(previousVertex.position);
	// 			linesPath.Add(currentLine);
	// 		}


	// 	}


	// 	meshToLinesLocal.Add(mesh, linesPath);

	// 	return linesPath;
	// }



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
