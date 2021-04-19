using UnityEngine;

public abstract class LandingZone : LocalizedObject
{
    
    
    protected LandingZone(Vector3 position) : base( position ) {}
    
    
    public override GameObject createInstance()
    {
        GameObject instance = GameObject.Instantiate( prefToInstantiate().gameObject );
        instance.GetComponent<LZbehaviour>().LZscript = this;
        
        return instance;
    }
    
    protected abstract LZbehaviour prefToInstantiate();
    

}