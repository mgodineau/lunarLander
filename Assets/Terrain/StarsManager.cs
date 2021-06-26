using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StarsManager : MonoBehaviour
{
    
    
    
    [SerializeField] private uint starCount = 1000;
    [SerializeField] private float starDepth = 100;
    [SerializeField] private float fov = 90.0f;
    
    private LineData[] starsDisplay;
    
    private Vector3[] stars;
    private float[] starsBrightness;
    
    private void Awake() {
        
        stars = new Vector3[starCount];
        starsBrightness = new float[starCount];
        starsDisplay = new LineData[starCount];
        
        
        
        for( int i=0; i<starCount; i++ ) {
            stars[i] = Random.onUnitSphere;
            starsBrightness[i] = Random.value;
            
            starsDisplay[i] = new LineData( Color.white * starsBrightness[i] );
            starsDisplay[i].points.Add( Vector3.zero );
            starsDisplay[i].points.Add( Vector3.zero );
        }
        
        
    }
    
    public void Start() {
        foreach( LineData star in starsDisplay ) {
            WireframeRender.Instance.linesBackground.Add(star);
        }
    }
    
    
    public void Update() {
        UpdateStars();
    }
    
    private void UpdateStars() {
        
        Vector3 pixelOffset = Vector3.up / Screen.height;
        
        for( int i=0; i<starCount; i++ ) {
            
            Vector3 screenPos = GetScreenPos( stars[i] );
            
            starsDisplay[i].points[0] = screenPos;
            starsDisplay[i].points[1] = screenPos + pixelOffset;
        }
        
    }
    
    public Vector3 GetScreenPos( Vector3 dir ) {
        
        TerrainManager terrain = TerrainManager.Instance;
        
        Vector3 localStarDir = Quaternion.Inverse(
                Quaternion.LookRotation( 
                    -terrain.SliceNormal, 
                    -terrain.ConvertXtoDir(terrain.LightReference.position.x)
                )
            ) * dir;
        
        
        
        return localStarDir.z >= 0 ? 
            new Vector3( 
                ConvertAngleToScreenPos( Mathf.Asin(localStarDir.x), true ),
                ConvertAngleToScreenPos( Mathf.Asin(localStarDir.y), false ),
                starDepth ) :
            Vector3.zero;
            ;
    }
    
    
    private float ConvertAngleToScreenPos( float angle, bool x ) {
        
        float currentFOV = x ? fov : fov * Screen.height / Screen.width;
        return (angle / (currentFOV*0.5f*Mathf.Deg2Rad)) * 0.5f + 0.5f;
    }
    
}
