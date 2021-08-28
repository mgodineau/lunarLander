using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSpaceSprite : MonoBehaviour
{
    
    private const float spriteSize = 5.0f;
    
    
    [SerializeField] private float _screenSpaceHeight = 0.5f;
    public float ScreenSpaceHeight {
        get { return _screenSpaceHeight; }
        set { _screenSpaceHeight = Mathf.Max(0, value); }
    }
    
    
    public Vector2 screenPos = Vector2.one * 0.5f;
    
    
    
    private void Update() {
        
        Vector3 camPos = Camera.main.transform.position;
        
        Vector2 orthoSize = new Vector2( 
            Camera.main.orthographicSize * Screen.width / Screen.height, 
            Camera.main.orthographicSize
        ) * 2;
        
        Vector2 offset = screenPos - Vector2.one * 0.5f;
        offset.x *= orthoSize.x;
        offset.y *= orthoSize.y;
        
        Vector3 pos = new Vector3(
            camPos.x + offset.x,
            camPos.y + offset.y,
            transform.position.z
        );
        transform.position = pos;
        
        
        float scale = _screenSpaceHeight * orthoSize.y / spriteSize;
        Vector3 scaleV3 = new Vector3(
            scale,
            scale,
            1
        );
        transform.localScale = scaleV3;
        
    }
    
    
}
