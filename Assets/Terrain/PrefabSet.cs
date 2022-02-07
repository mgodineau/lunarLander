using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabSet", menuName = "ScriptableObjects/PrefabSet", order = 1)]
public class PrefabSet : ScriptableObject
{
    
    [SerializeField] private LZbehaviour _lzDefaultPref;
    [SerializeField] private LZbehaviour _lzFuelPref;
    [SerializeField] private LZbehaviour _lzRadarPref;
    
    [SerializeField] private ItemBehaviour _crystalPref;
    [SerializeField] private ItemBehaviour _cratePref;
    
    
    public LZbehaviour LzDefaultPref {
        get {return _lzDefaultPref;}
    }
    public LZbehaviour LzFuelPref {
        get {return _lzFuelPref;}
    }
    public LZbehaviour LzRadarPref {
        get {return _lzRadarPref;}
    }
    
    public ItemBehaviour CrystalPref {
        get {return _crystalPref;}
    }
    public ItemBehaviour CratePref {
        get {return _cratePref;}
    }
    
}
