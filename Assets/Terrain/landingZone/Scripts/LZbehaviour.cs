using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LZbehaviour : MonoBehaviour
{
    
    
    public LandingZone LZscript;
    
    [SerializeField]
    private List<GameObject> installationsPrefs = new List<GameObject>();
    private List<GameObject> installationsInstances = new List<GameObject>();
    
    [SerializeField]
    private float installationsDist = 10.0f;
    
    private void OnEnable() {
        CreateInstallations();
    }
    
    
    
    
    private void CreateInstallations() {
        
        RemoveInstallations();
        foreach( GameObject pref in installationsPrefs ) {
            
            Vector3 pos = transform.position;
            pos += Quaternion.AngleAxis( Random.Range(-180, 0), Vector3.up ) * (Vector3.right * installationsDist);
            pos.y = TerrainManager.Instance.GetHeightOf(pos);
            
            Quaternion rot = Quaternion.AngleAxis( Random.Range(0, 360), Vector3.up);
            
            GameObject instance = GameObject.Instantiate(pref, pos, rot, transform);
            installationsInstances.Add(instance);
            
        }
        
    }
    
    private void RemoveInstallations() {
        
        foreach( GameObject inst in installationsInstances ) {
            Destroy(inst);
        }
        
        installationsInstances.Clear();
        
    }
    
    
    
}
