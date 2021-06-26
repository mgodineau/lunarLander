using UnityEngine;

public abstract class LandingZone : LocalizedObject
{
    
    
    protected LandingZone(Vector3 position) : base( position ) {}
    
    
    public override GameObject CreateInstance( Vector3 position, Quaternion rotation, Transform parent )
    {
        GameObject instance = GameObject.Instantiate( PrefToInstantiate(), position, rotation, parent );
        // instance.AddComponent<LZbehaviour>().LZscript = this;
        instance.GetComponent<LZbehaviour>().LZscript = this;
        
        
        return instance;
    }
    
    public abstract SubMenu GetMenu( Lander lander );
    
    protected abstract GameObject PrefToInstantiate();
    

}