using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;



public class StarsManager : MonoBehaviour
{
	private static StarsManager _instance = null;
	public static StarsManager Instance {
		get {return _instance;}
	}
	
	
	[SerializeField] private uint starCount = 1000;
	[SerializeField] private List<StarsColor> starsColors;
	
	
	[SerializeField] private float maxCamSize = 150;
	[SerializeField] private float maxFOV = 120;
	
	[SerializeField] private ScreenSpaceSprite sun;
	
	
	private Mesh starMesh;
	
	
	
	[System.Serializable]
	private struct StarsColor {
		public Color color;
		public float weight;
	}
	
	
	
	private void Awake() {
		
		starMesh = new Mesh();
		_instance = this;
		
		UpdateStars();
	}
	
	
	
	
	public void Update() {
		UpdateSun();
	}
	
	
	
	public void DrawSky( Material starsMaterial ) {
		Quaternion globalToLocalRot = ComputeGlobalToLocalRot();
		Matrix4x4 viewMatrix = ComputeCameraProjection();
		
		DrawStars(globalToLocalRot, viewMatrix, starsMaterial);
		UpdateSun(globalToLocalRot, viewMatrix);
	}
	
	
	
	
	
	public void DrawStars( Material mat ) {
		DrawStars( ComputeGlobalToLocalRot(), ComputeCameraProjection(), mat );
	}
	
	private void DrawStars( Quaternion globalToLocalRot, Matrix4x4 viewMatrix, Material mat ) {
		
		GL.PushMatrix();
			GL.LoadProjectionMatrix( GL.GetGPUProjectionMatrix(viewMatrix, true) );
			for( int i=0; i<starsColors.Count; i++ ) {
				mat.color = starsColors[i].color;
				mat.SetPass(0);
				Graphics.DrawMeshNow(starMesh, Camera.main.transform.localToWorldMatrix * Matrix4x4.Rotate(globalToLocalRot), i );
			}
		GL.PopMatrix();
	}
	
	
	
	
	
	public void UpdateSun() {
		UpdateSun(ComputeGlobalToLocalRot(), ComputeCameraProjection());
	}
	
	private void UpdateSun(Quaternion globalToLocalRot, Matrix4x4 viewMatrix) {
		Vector3 sunLocalDir = globalToLocalRot * TerrainManager.Instance.globalLightDir;
		
		Vector4 screenPosV4 = viewMatrix * new Vector4(sunLocalDir.x, sunLocalDir.y, sunLocalDir.z, 1.0f);
		screenPosV4.x /= -screenPosV4.z;
		screenPosV4.y /= screenPosV4.z;
		sun.screenPos = new Vector2( screenPosV4.x+1, screenPosV4.y+1 ) * 0.5f;
		
		sun.gameObject.SetActive( sunLocalDir.z >= 0 );
	}
	
	
	private void UpdateStars() {
		// création des étoiles
		Vector3[] vertices = new Vector3[starCount];
		for( int i=0; i<starCount; i++ ) {
			vertices[i] = UnityEngine.Random.onUnitSphere;
		}
		starMesh.SetVertices(vertices);
		
		// création du tableau triangles
		int[] indices = new int[vertices.Length];
		for( int i=0; i<vertices.Length; i++ ) {
			indices[i] = i;
		}
		
		
		//affectation des étoiles au mesh		
		float weightSum = 0.0f;
		foreach( StarsColor color in starsColors ) {
			weightSum += color.weight;
		}
		int[] colorsStartIndexes = new int[starsColors.Count+1];
		colorsStartIndexes[0] = 0;
		for( int i=0; i<starsColors.Count; i++ ) {
			colorsStartIndexes[i+1] = colorsStartIndexes[i] + Mathf.RoundToInt(starsColors[i].weight * starCount / weightSum);
		}
		
		SubMeshDescriptor[] descriptors = new SubMeshDescriptor[starsColors.Count];
		starMesh.subMeshCount = starsColors.Count;
		for( int i=0; i<descriptors.Length; i++ ) {
			
			int indicesCount = colorsStartIndexes[i+1] - colorsStartIndexes[i];
			starMesh.SetIndices( indices, colorsStartIndexes[i], indicesCount, MeshTopology.Points, i);
			descriptors[i] = new SubMeshDescriptor(colorsStartIndexes[i], indicesCount, MeshTopology.Points);
			
		}
		starMesh.SetSubMeshes( descriptors );
	}
	
	
	private Quaternion ComputeGlobalToLocalRot() {
		float fov = Camera.main.orthographicSize * maxFOV / maxCamSize;
		
		TerrainManager terrain = TerrainManager.Instance;
		return Quaternion.Inverse(
			Quaternion.LookRotation( 
				terrain.SliceNormal, 
				-terrain.ConvertXtoDir(terrain.LightReference.position.x)
			)
		);
	}
	
	private Matrix4x4 ComputeCameraProjection() {
		float fov = Camera.main.orthographicSize * maxFOV / maxCamSize;
		return Matrix4x4.Perspective(fov, Camera.main.aspect, 0.01f, 2);
	}
	
    
}
