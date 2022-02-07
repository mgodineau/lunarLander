using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LZbehaviour : ObjectBehaviour
{
    private LandingZone _lzScript;
    public LandingZone LZscript {
        get {return _lzScript;}
        set {
            _lzScript = value;
        }
    }

    public override LocalizedObject LocObject {
        get {return _lzScript;}
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
            instance.transform.GetChild(0).localScale = new Vector3(1, 10, 1);  //set the ground size
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
