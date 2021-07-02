using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RandomLayer : TerrainLayer {
    
    
    private Noise rng;
    
    [SerializeField]
    private float _scale;
    [SerializeField]
    private float _maxHeight;
    
    private float inverseScale;
    private float maxHeightHalf;
    public Vector3 offset;
    
    public float Scale {
        get{ return _scale; }
        set{ 
            _scale = Mathf.Abs(value);
            inverseScale = 1.0f / _scale; 
        }
    }
    public override float MaxHeight {
        get{ return _maxHeight; }
    }
    
    public override float MinHeight {
        get{ return 0; }
    }
    
    
    public override void OnValidate() {
        Scale = _scale;
    }
    
    
    public override float GetHeight( Vector3 position) {
        position = position.normalized + offset;
        return (rng.Evaluate(position * inverseScale) + 1) * maxHeightHalf;
    }
    
    
    
    public RandomLayer(float scale=1, float maxHeight=1) : this(scale, maxHeight, Vector2.one) {}
    
    public RandomLayer( float scale, float maxHeight, Vector2 offset, int seed=0 ) {
        Scale = scale;
        _maxHeight = maxHeight;
        maxHeightHalf = _maxHeight * 0.5f;
        
        this.offset = offset;
        
        rng = new Noise(seed);
    }

}