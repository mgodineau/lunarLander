using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class WireframeRender : MonoBehaviour
{
	
	private static WireframeRender _instance;
	public static WireframeRender Instance {
		get{ return _instance; }
	}
	
	private Camera cam;
	private RenderTexture sourceCopy = null;
	private RenderTexture afterImageRT = null;
	
	[SerializeField] private Material linePostProcMaterial;
	
	[SerializeField] private Material lineGeometryDrawMaterial;
	[SerializeField] private Material lineBackgroundDrawMaterial;
	[SerializeField] private Material uiLineDrawMaterial;
	

	public HashSet<LineData> linesGeometry = new HashSet<LineData>();
	public HashSet<LineData> linesBackground = new HashSet<LineData>();
	public HashSet<LineData> linesUI = new HashSet<LineData>();
	
	
	private int lastCamWidth;
	private int lastCamHeight;
	
	
	private void Awake() {
		_instance = this;
		cam = GetComponent<Camera>();
		cam.depthTextureMode = DepthTextureMode.Depth;
		
		RefreshRenderTextures();
	}
	
	private void Update() {
		bool resolutionChange = lastCamWidth != cam.pixelWidth || lastCamHeight != cam.pixelHeight;
		if( resolutionChange ) {
			RefreshRenderTextures();
		}
	}
	
	private void OnDestroy() {
		ReleaseRenderTextures();
	}
	
	
	
	private void OnRenderImage( RenderTexture source, RenderTexture dest ) {
		
		if( cam.pixelHeight != sourceCopy.height || cam.pixelWidth != sourceCopy.width ) {
			sourceCopy.Release();
			sourceCopy = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 16, RenderTextureFormat.Default);
		}
		
		 //rendu des lignes sur source, et copie de la vraie source dans wireframeRT, parce que ça parche pas dans l'autre sens
		Graphics.Blit( source, sourceCopy );
		Graphics.SetRenderTarget(source);
		GL.Clear( false, true, Color.clear, 1 );
		
		
		//rendu de l'arrière plan
		lineBackgroundDrawMaterial.SetPass(0);
		StarsManager.Instance.DrawStars();
		
		
		//rendu des lignes en 3d
		lineGeometryDrawMaterial.SetPass(0);
		lineGeometryDrawMaterial.color = Color.green;
		foreach( LineData line in linesGeometry ) {
			GL.Begin( GL.LINE_STRIP );
			GL.Color( line.LineColor );
			
			foreach( Vector3 pos in line.points ) {
				GL.Vertex(pos);
			}
			
			GL.End();
		}
		//rendu des lignes 3d, mais avec des meshs
		lineGeometryDrawMaterial.SetColor("_Color", Color.white);
		lineGeometryDrawMaterial.SetPass(0);
		WireframeEffect.DrawAllNow();
		
		//rendu des lignes du HUD
		uiLineDrawMaterial.SetPass(0);
		GL.PushMatrix();
		
			GL.LoadOrtho();
			foreach( LineData line in linesUI ) {
				GL.Begin( GL.LINE_STRIP );
				GL.Color( line.LineColor );
				
				foreach( Vector3 pos in line.points ) {
					Vector3 pos2 = pos;
					pos2.z = 0;
					
					GL.Vertex(pos2);
				}
				
				GL.End();
			}
		GL.PopMatrix();
		
		
		RenderTexture afterImageBuffer = RenderTexture.GetTemporary( afterImageRT.descriptor );
		
		linePostProcMaterial.SetTexture("_MainTex", sourceCopy);
		linePostProcMaterial.SetTexture("_WireframeTex", source);
		linePostProcMaterial.SetTexture("_AfterImageTex", afterImageRT);
		linePostProcMaterial.SetFloat("_DetlaTime", Time.deltaTime);
		
		RenderTexture tmpDest = RenderTexture.GetTemporary(source.descriptor);
		RenderBuffer[] buffers = { tmpDest.colorBuffer, afterImageBuffer.colorBuffer };
		
		MRT( buffers, tmpDest.depthBuffer, linePostProcMaterial);
		
		Graphics.Blit(tmpDest, dest);
		
		RenderTexture.ReleaseTemporary(tmpDest);
		
		Graphics.CopyTexture(afterImageBuffer, afterImageRT);
		RenderTexture.ReleaseTemporary(afterImageBuffer);
		
		
		// Graphics.Blit(source, dest); //DEBUG : montre juste les lignes
	}
	
	
	private Matrix4x4 ApplyZoffset( Matrix4x4 proj, float offset ) {
		Debug.Log( proj );
		proj[2,3] -= offset;
		return proj;
	}
	
	
	private RenderTexture CreateScreenRT() {
		RenderTexture tex = new RenderTexture( cam.pixelWidth, cam.pixelHeight, 16, RenderTextureFormat.ARGB32 );
		tex.useDynamicScale = true;
		tex.enableRandomWrite = true;
		
		Graphics.SetRenderTarget(tex);
		GL.Clear(true, true, Color.clear, 1);
		
		return tex;
	}
	
	
	private void RefreshRenderTextures() {
		
		ReleaseRenderTextures();
		
		sourceCopy = CreateScreenRT();
		afterImageRT = CreateScreenRT();
		
		lastCamWidth = cam.pixelWidth;
		lastCamHeight = cam.pixelHeight;
	}
	
	private void ReleaseRenderTextures() {
		if(sourceCopy != null) {
			sourceCopy.Release();
		}
		if(afterImageRT != null) {
			afterImageRT.Release();
		}
	}
	
	
	private void MRT (RenderBuffer[] rb, RenderBuffer depth, Material mat)
	{
		
		Graphics.SetRenderTarget(rb, depth);
		GL.PushMatrix();
			GL.LoadOrtho();
			mat.SetPass(0);
			GL.Begin(GL.QUADS);
				GL.TexCoord2(0.0f, 0.0f);
				GL.Vertex3(0.0f, 0.0f, 0.1f);
				GL.TexCoord2(1.0f, 0.0f);
				GL.Vertex3(1.0f, 0.0f, 0.1f);
				GL.TexCoord2(1.0f, 1.0f);
				GL.Vertex3(1.0f, 1.0f, 0.1f);
				GL.TexCoord2(0.0f, 1.0f);
				GL.Vertex3(0.0f, 1.0f, 0.1f);
			GL.End();
		GL.PopMatrix();
	}
	
	
	
	
}
