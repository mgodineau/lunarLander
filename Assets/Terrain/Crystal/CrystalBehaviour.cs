using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalBehaviour : MonoBehaviour
{
    
    public Crystal crystalScript;

    
    public void Pickup()
    {
        TerrainManager.Instance.Planet.crystals.Remove(crystalScript);
        Destroy( gameObject );
    }
    
    
}
