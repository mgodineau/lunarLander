using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StarsManager : MonoBehaviour
{
    
    
    
    [SerializeField] private uint starCount = 1000;
    [SerializeField] private float starDepth = 100;
    // [SerializeField] private float fov = 90.0f;
    [SerializeField] private float maxCamSize = 150;
    [SerializeField] private float maxFOV = 120;
    
    // [SerializeField] private SunManager sun;
    [SerializeField] private ScreenSpaceSprite sun;
    
    
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
        float fov = Camera.main.orthographicSize * maxFOV / maxCamSize;
        float verticalFOV = fov * Screen.height / Screen.width;
        
        TerrainManager terrain = TerrainManager.Instance;
        Quaternion globalToLocalRot = Quaternion.Inverse(
                Quaternion.LookRotation( 
                    -terrain.SliceNormal, 
                    -terrain.ConvertXtoDir(terrain.LightReference.position.x)
                )
            );
        
        // affichage des étoiles
        for( int i=0; i<starCount; i++ ) {
            
            Vector3 localStarDir = globalToLocalRot * stars[i];
            Vector3 screenPos = localStarDir.z >= 0 ? 
                new Vector3( 
                    ConvertAngleToScreenPos( Mathf.Asin(localStarDir.x), fov ),
                    ConvertAngleToScreenPos( Mathf.Asin(localStarDir.y), verticalFOV ),
                    starDepth ) :
                Vector3.zero;
            
            
            starsDisplay[i].points[0] = screenPos;
            starsDisplay[i].points[1] = screenPos + pixelOffset;
        }
        
        
        //affichage du soleil
        Vector3 sunLocalDir = globalToLocalRot * -TerrainManager.Instance.globalLightDir;
        // sun.ScreenPos = new Vector2( 
        //     ConvertAngleToScreenPos( Mathf.Asin(Mathf.Clamp(sunLocalDir.x, -1, 1)), fov ),
        //     ConvertAngleToScreenPos( Mathf.Asin(Mathf.Clamp(sunLocalDir.y, -1, 1)), verticalFOV ));
        sun.screenPos = new Vector2( 
            ConvertAngleToScreenPos( Mathf.Asin(Mathf.Clamp(sunLocalDir.x, -1, 1)), fov ),
            ConvertAngleToScreenPos( Mathf.Asin(Mathf.Clamp(sunLocalDir.y, -1, 1)), verticalFOV ));
        
        sun.gameObject.SetActive( sunLocalDir.z >= 0 );
    }
    
    
    private float ConvertAngleToScreenPos( float angle, float currentFOV ) {
        
        return (angle / (currentFOV*0.5f*Mathf.Deg2Rad)) * 0.5f + 0.5f;
    }
    
}
