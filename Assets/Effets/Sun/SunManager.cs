using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(RectTransform))]
public class SunManager : MonoBehaviour
{
    
    private RectTransform rect;
    
    
    private Vector2 _halfScreenSizes;
    
    public float VerticalScreenSize {
        get { return _halfScreenSizes.y * 2.0f; }
        set { 
            _halfScreenSizes.y = 0.5f * Mathf.Max(value, 0); 
            UpdateScreenRatio();
        }
    }
    
    private Vector2 _screenPos = Vector2.one * 0.5f;
    public Vector2 ScreenPos {
        get { return _screenPos; }
        set{ 
            _screenPos = value; 
            UpdateDisplay();
        }
    }
    
    private void Awake() {
        rect = GetComponent<RectTransform>();
        VerticalScreenSize = rect.anchorMax.y - rect.anchorMin.y;
        
        UpdateDisplay();
    }
    
    
    
    
    public void UpdateDisplay() {
        
        UpdateScreenRatio();
        
        rect.anchorMax = ScreenPos + _halfScreenSizes;
        rect.anchorMin = ScreenPos - _halfScreenSizes;
        
    }
    
    
    private void UpdateScreenRatio() {
        _halfScreenSizes.x = _halfScreenSizes.y * Screen.height / Screen.width;
    }
    
}
