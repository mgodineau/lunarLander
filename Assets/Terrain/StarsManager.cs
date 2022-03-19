using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;



public class StarsManager : MonoBehaviour
{
	private static StarsManager _instance = null;
	public static StarsManager Instance {
		get {return _instance;}
	}
	
	
	[SerializeField] private uint starCount = 1000;
	
	[SerializeField] private float maxCamSize = 150;
	[SerializeField] private float maxFOV = 120;
	
	[SerializeField] private ScreenSpaceSprite sun;
	
	
	private Vector3[] stars;
	
	private Mesh starMesh;
	
	private void Awake() {
		
		starMesh = new Mesh();
		_instance = this;
		
		UpdateStars();
	}
	
	
	
	
	public void Update() {
		UpdateSun();
	}
	
	
	
	public void DrawSky() {
		Quaternion globalToLocalRot = ComputeGlobalToLocalRot();
		Matrix4x4 viewMatrix = ComputeCameraProjection();
		
		DrawStars(globalToLocalRot, viewMatrix);
		UpdateSun(globalToLocalRot, viewMatrix);
	}
	
	
	
	
	
	public void DrawStars() {
		DrawStars( ComputeGlobalToLocalRot(), ComputeCameraProjection() );
	}
	
	private void DrawStars( Quaternion globalToLocalRot, Matrix4x4 viewMatrix ) {
		
		GL.PushMatrix();
			GL.LoadProjectionMatrix( GL.GetGPUProjectionMatrix(viewMatrix, true) );
			Graphics.DrawMeshNow(starMesh, Camera.main.transform.localToWorldMatrix * Matrix4x4.Rotate(globalToLocalRot) );
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
		stars = new Vector3[starCount];
		for( int i=0; i<starCount; i++ ) {
			stars[i] = Random.onUnitSphere;
		}
		
		// copie des étoiles dans les vertices du mesh
		Vector3[] vertices = stars;
		int trianglesLength = vertices.Length;
		if( trianglesLength % 3 != 0 ) {
			trianglesLength += 3 - trianglesLength%3;
		}
		// création du tableau triangles
		int[] triangles = new int[trianglesLength];
		for( int i=0; i<vertices.Length; i++ ) {
			triangles[i] = i;
		}
		
		//affectation des étoiles au mesh
		starMesh.SetVertices(vertices);
		starMesh.SetTriangles(triangles, 0);
		
		SubMeshDescriptor[] desc = {new SubMeshDescriptor(0, vertices.Length, MeshTopology.Points)};
		starMesh.SetSubMeshes( desc );
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
