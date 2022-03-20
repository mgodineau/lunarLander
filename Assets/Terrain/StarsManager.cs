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
	
	
	[SerializeField] private float maxCamSize = 150;
	[SerializeField] private float maxFOV = 120;
	
	[SerializeField] private ScreenSpaceSprite sun;
	
	
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
		DrawStars( ComputeGlobalToLocalRot(), ComputeCameraProjection());
	}
	
	private void DrawStars( Quaternion globalToLocalRot, Matrix4x4 viewMatrix) {
		
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
		Vector3[] vertices = new Vector3[starCount];
		Color32[] colors = new Color32[starCount];
		int[] indices = new int[starCount];
		
		for( int i=0; i<starCount; i++ ) {
			vertices[i] = UnityEngine.Random.onUnitSphere;
			
			byte brightness = (byte)Random.Range(0, 256);
			colors[i] = new Color32(brightness, brightness, brightness, 255);
			
			indices[i] = i;
		}
		
		starMesh.SetVertices(vertices);
		starMesh.SetColors(colors);
		starMesh.SetIndices( indices, MeshTopology.Points, 0);
		
		
		//affectation des étoiles au mesh		
		SubMeshDescriptor[] descriptors = new SubMeshDescriptor[]{new SubMeshDescriptor(0, indices.Length, MeshTopology.Points)};
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
