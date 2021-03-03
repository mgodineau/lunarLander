using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : Instrument
{
    
    [SerializeField]
    private int resolution = 10;
    
    [SerializeField]
    private float iconWidth = 0.01f;
    
    [SerializeField]
    private float sliceDelta = 10.0f;
    
    LineData bgVertical = new LineData();
    LineData bgHorizontal = new LineData();
    
    LineData landerPos = new LineData(Color.red);
    LineData sliceLine = new LineData(Color.red);
    
    
    new private void Start() {
        base.Start();
        
        BuildLayout();
        
        EnableLayout();
        EnableInfos();
    }
    
    
    private void Update() {
        UpdateLanderLocation();
        UpdateSliceLine();
    }
    
    
    private void UpdateLanderLocation() {
        Vector3 landerDir = TerrainManager.Instance.convertXtoDir(
            InstrumentsManager.Instance.Lander.position.x
        );
        Vector3 landerCenter = localToGlobal( dirToLocalPos(landerDir) );
        landerPos.points = new List<Vector3>();
        float iconHeight = iconWidth * Screen.width / Screen.height;
        landerPos.points.Add( landerCenter + Vector3.up * iconHeight );
        landerPos.points.Add( landerCenter + Vector3.right * iconWidth );
        landerPos.points.Add( landerCenter - Vector3.up * iconHeight );
        landerPos.points.Add( landerCenter - Vector3.right * iconWidth );
        landerPos.points.Add( landerCenter + Vector3.up * iconHeight );
    }
    
    
    private void UpdateSliceLine() {
        sliceLine.points.Clear();
        LinkedList<Vector3> linePoints = new LinkedList<Vector3>();
        
        Vector3 sliceNormal = TerrainManager.Instance.SliceNormal;
        if( sliceNormal.y < 0 ) {
            sliceNormal = -sliceNormal;
        }
        Vector3 originDir = TerrainManager.Instance.SliceOrigin;
        Vector3 originLocalPos = dirToLocalPos(originDir);
        linePoints.AddLast(  localToGlobal(originLocalPos) );
        
        Vector2 sampleLocalPos = dirToLocalPos( Quaternion.AngleAxis(sliceDelta, sliceNormal) * originDir );
        int i=2;
        while( sampleLocalPos.x > originLocalPos.x ) {
            linePoints.AddLast(localToGlobal(sampleLocalPos) );
            sampleLocalPos = dirToLocalPos( Quaternion.AngleAxis(sliceDelta * i, sliceNormal) * originDir );
            i++;
        }
        
        
        i = 2;
        sampleLocalPos = dirToLocalPos( Quaternion.AngleAxis(-sliceDelta, sliceNormal) * originDir );
        while( sampleLocalPos.x < originLocalPos.x ) {
            linePoints.AddFirst(localToGlobal(sampleLocalPos) );
            sampleLocalPos = dirToLocalPos( Quaternion.AngleAxis(-sliceDelta * i, sliceNormal) * originDir );
            i++;
        }
        
        sliceLine.points.AddRange( linePoints );
    }
    
    
    
    private Vector2 dirToLocalPos( Vector3 dir ) {
        
        float localX = Mathf.Sqrt( 1.0f - dir.y*dir.y );
        float yAngle = Mathf.Atan2( dir.y, localX );
        
        float xAngle = Vector2.SignedAngle( new Vector2(dir.x, dir.z), Vector2.right );
        
        return new Vector2( (xAngle / 360.0f) +0.5f , yAngle / Mathf.PI + 0.5f );
    }
    
    
    public void BuildLayout() {
        bgHorizontal.points.Clear();
        bgVertical.points.Clear();
        
        //lignes horizontales
        float height = 1.0f / resolution;
        bool left = true;
        for( int i=1; i<=resolution; i++ ) {
            float x = left ? 0 : 1;
            left = !left;
            
            bgHorizontal.points.Add( localToGlobal( new Vector3( x, height * (i-1), 0) ));
            bgHorizontal.points.Add( localToGlobal( new Vector3(x, height * i, 0) ));
        }
        
        //lignes verticales
        float width = 0.5f / resolution;
        bool down = true;
        for( int i=1; i<=resolution*2; i++ ) {
            float y = down ? 0 : 1;
            down = !down;
            
            bgVertical.points.Add( localToGlobal( new Vector3( width * (i-1), y, 0) ));
            bgVertical.points.Add( localToGlobal( new Vector3( width * i, y, 0) ));
        }
    }
    
    
    
    
    public void EnableLayout() {
        WireframeRender.Instance.linesUI.Add( bgVertical );
        WireframeRender.Instance.linesUI.Add( bgHorizontal );
    }
    
    
    public void DisableLayout() {
        WireframeRender.Instance.linesUI.Remove( bgVertical );
        WireframeRender.Instance.linesUI.Remove( bgHorizontal );
    }
    
    public void EnableInfos() {
        WireframeRender.Instance.linesUI.Add( landerPos );
        WireframeRender.Instance.linesUI.Add( sliceLine );
    }
    
    public void DisableInfos() {
        WireframeRender.Instance.linesUI.Remove( landerPos );
        WireframeRender.Instance.linesUI.Remove( sliceLine );
    }
    
}
