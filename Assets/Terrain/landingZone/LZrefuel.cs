using UnityEngine;


public class LZrefuel : LandingZone
{

    public LZrefuel(Vector3 position) : base(position)
    {
    }


    protected override LZbehaviour prefToInstantiate()
    {
        return TerrainManager.Instance.lzPref;
    }

    
}