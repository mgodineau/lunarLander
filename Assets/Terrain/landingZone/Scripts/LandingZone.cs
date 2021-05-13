using UnityEngine;

public abstract class LandingZone : LocalizedObject
{
    
    
    protected LandingZone(Vector3 position) : base( position ) {}
    
    
    public override GameObject createInstance( Vector3 position, Quaternion rotation, Transform parent )
    {
        GameObject instance = GameObject.Instantiate( prefToInstantiate(), position, rotation, parent );
        // instance.AddComponent<LZbehaviour>().LZscript = this;
        instance.GetComponent<LZbehaviour>().LZscript = this;
        
        
        return instance;
    }
    
    protected abstract GameObject prefToInstantiate();
    

}