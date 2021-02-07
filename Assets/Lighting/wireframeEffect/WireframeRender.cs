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
    private Material lineMaterial;
    
    public List<List<Vector3>> linePaths = new List<List<Vector3>>();
    
    
    private void Awake() {
        _instance = this;
        cam = GetComponent<Camera>();
        
        // lineMaterial = new Material(lineShader);
        wireframeRT = new RenderTexture( cam.pixelWidth, cam.pixelHeight, 16, RenderTextureFormat.Default );
    }
    
    
    private void OnDestroy() {
        wireframeRT.Release();
    }
    
    
    
    private void OnRenderImage( RenderTexture source, RenderTexture dest ) {
        
        
        Graphics.SetRenderTarget(wireframeRT);
        GL.Clear( false, true, Color.clear, 1 );
        
        // lineMaterial.SetPass(0);
        foreach( List<Vector3> path in linePaths ) {
            GL.Begin( GL.LINE_STRIP );
            GL.Color( Color.white );
            
            foreach( Vector3 pos in path ) {
                GL.Vertex(pos);
            }
            
            GL.End();
        }
        
        lineMaterial.SetTexture("_WireframeTex", wireframeRT);
        Graphics.Blit(source, dest, lineMaterial);
    }
    
    
}
