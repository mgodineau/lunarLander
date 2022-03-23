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
	private Color _edgesColor = Color.white;
	public Color EdgesColor {
		get {return _edgesColor;}
		set {
			SetColor32( value );
		}
	}
	
	
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
	
	
	
	private void UpdateLinesFromMesh()
	{
		Mesh mesh = meshFilter.mesh;
		if( meshToWireframe.ContainsKey(mesh) ) {
			wireframeMesh = meshToWireframe[mesh];
			return;
		}
		
		
		
		BetterMesh betterMesh = new BetterMesh(mesh);
		betterMesh.RemoveDuplicates();
		Vector3[] vertices = betterMesh.GetMeshVertices();
		HashSet<Edge> edges = GetDisplayedEdge(betterMesh);
		
		int trianglesLength = edges.Count*2;
		if(trianglesLength%3 != 0) {
			trianglesLength += 3 - trianglesLength%3;
		}
		int[] triangles = new int[trianglesLength];
		int i = 0;
		foreach( Edge edge in edges ) {
			triangles[i] = betterMesh.Vertices.IndexOf(edge.vertex_0);
			triangles[i+1] = betterMesh.Vertices.IndexOf(edge.vertex_1);
			i+=2;
		}
		
		wireframeMesh.SetVertices( vertices );
		wireframeMesh.SetTriangles(triangles, 0);
		
		SubMeshDescriptor[] desc = {new SubMeshDescriptor(0, edges.Count*2, MeshTopology.Lines)};
		wireframeMesh.SetSubMeshes( desc );
		
		SetColor32(_edgesColor);
	}
	
	
	
	private void SetColor32( Color32 color ) {
		
		Color32[] colors = new Color32[wireframeMesh.vertices.Length];
		for( int i=0; i<colors.Length; i++ ) {
			colors[i] = color;
		}
		wireframeMesh.SetColors(colors);
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
				if (!previousEdges.Add(edge)){
					continue;
				}
				
				bool ignore = false;
				for (int j = i + 1; j < triangles.Count; j++)
				{
					//suppression des edges qui sont sur d'autres triangles parallèles
					if (new List<Edge>(triangles[j].GetEdges()).Contains(edge))
					{
						float angleCos = Vector3.Dot( triangles[i].GetNormal(), triangles[j].GetNormal());
						angleCos = Mathf.Abs(angleCos);
						if ( angleCos > Mathf.Cos( Mathf.Deg2Rad * diagThreshold ) ) {
							ignore = true;
							break;
						}
					}
				}
				if (!ignore)
				{
					displayedEdges.Add(edge);
				}

				
			}

		}

		return displayedEdges;
	}



	// private bool linesEquals(int[] line1, int[] line2)
	// {
	// 	return line1[0] == line2[0] && line1[1] == line2[1]
	// 	|| line1[0] == line2[1] && line1[1] == line2[0];
	// }

}
