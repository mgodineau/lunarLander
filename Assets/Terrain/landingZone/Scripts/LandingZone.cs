using UnityEngine;

public abstract class LandingZone : LocalizedObject
{
    
    
    protected LandingZone(Vector3 position) : base( position, true, true ) {}
    
    
    public override ObjectBehaviour CreateInstance( Vector3 position)
    {
        GameObject instance = GameObject.Instantiate( PrefToInstantiate().gameObject, position, Quaternion.identity );
        instance.GetComponent<LZbehaviour>().LZscript = this;
        
        
        return instance.GetComponent<ObjectBehaviour>();
    }
    
    public abstract SubMenu GetMenu( Lander lander );
    
    protected abstract LZbehaviour PrefToInstantiate();
    

}