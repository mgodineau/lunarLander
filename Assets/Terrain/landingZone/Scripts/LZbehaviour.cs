using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectBehaviour))]
public class LZbehaviour : MonoBehaviour
{
    
    private ObjectBehaviour objectBehaviour;
    
    private LandingZone _lzScript;
    public LandingZone LZscript {
        get {return _lzScript;}
        set {
            _lzScript = value;
            if( objectBehaviour == null ) {
                objectBehaviour = GetComponent<ObjectBehaviour>();
                objectBehaviour.obj = value;
            }
        }
    }
    
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
            pos.y = TerrainManager.Instance.GetHeightAt(pos);
            
            Quaternion rot = Quaternion.AngleAxis( Random.Range(0, 360), Vector3.up);
            
            GameObject instance = GameObject.Instantiate(pref, pos, rot, transform);
            instance.transform.GetChild(0).localScale = new Vector3(1, 10, 1);
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
