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
    private RenderTexture wireframeRT;
    
    [SerializeField]
    private Material linePostProcMaterial;
    [SerializeField]
    private Material lineDrawMaterial;
    
    public List<List<Vector3>> linePaths = new List<List<Vector3>>();
    
    // public List<List<Vector3>> linePathsUI = new List<List<Vector3>>();
    public List<LineData> linesUI = new List<LineData>();
    
    private void Awake() {
        _instance = this;
        cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;
        
        wireframeRT = new RenderTexture( cam.pixelWidth, cam.pixelHeight, 16, RenderTextureFormat.Default );
        wireframeRT.useDynamicScale = true;
    }
    
    
    private void OnDestroy() {
        wireframeRT.Release();
    }
    
    
    
    private void OnRenderImage( RenderTexture source, RenderTexture dest ) {
        
        if( cam.pixelHeight != wireframeRT.height || cam.pixelWidth != wireframeRT.width ) {
            wireframeRT.Release();
            wireframeRT = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 16, RenderTextureFormat.Default);
        }
        
        Graphics.Blit( source, wireframeRT ); //rendu des lignes sur source, et copie de la vraie source dans wireframeRT, parce que ça parche pas dans l'autre sens
        Graphics.SetRenderTarget(source);
        GL.Clear( false, true, Color.clear, 1 );
        
        
        //rendu des lignes en 3d
        lineDrawMaterial.SetPass(0);
        foreach( List<Vector3> path in linePaths ) {
            GL.Begin( GL.LINE_STRIP );
            GL.Color( Color.white );
            
            foreach( Vector3 pos in path ) {
                GL.Vertex(pos);
            }
            
            GL.End();
        }
        
        //rendu des lignes du HUD
        GL.PushMatrix();
        
        GL.LoadOrtho();
        foreach( LineData line in linesUI ) {
            GL.Begin( GL.LINE_STRIP );
            GL.Color( line.LineColor );
            
            foreach( Vector3 pos in line.points ) {
                GL.Vertex(pos);
            }
            
            GL.End();
        }
        GL.PopMatrix();
        
        
        linePostProcMaterial.SetTexture("_WireframeTex", source);
        Graphics.Blit(wireframeRT, dest, linePostProcMaterial);
        
    }
    
    
    private Matrix4x4 ApplyZoffset( Matrix4x4 proj, float offset ) {
        Debug.Log( proj );
        proj[2,3] -= offset;
        return proj;
    }
    
    
}
