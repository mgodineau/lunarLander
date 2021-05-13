using UnityEngine;


public class LZrefuel : LandingZone
{
    
    public LZrefuel(Vector3 position) : base(position)
    {
    }


    protected override GameObject prefToInstantiate()
    {
        return TerrainManager.Instance.lzFuelPref;
    }

    
}