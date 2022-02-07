using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "level", menuName = "ScriptableObjects/LevelParameters", order = 1)]
public class LevelParameters : ScriptableObject
{
    
    [SerializeField] private int _rocketPartsCount = 10;
    public int RocketPartsCount {
        get {return Mathf.Max( 0, _rocketPartsCount);}
    }
    
}
