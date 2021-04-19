using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class CameraFollowAdaptSize : MonoBehaviour
{
    
    
    public Transform target;
    
    [SerializeField] private float groundY = 0;
    
    [SerializeField] private float minSize = 30;
    [SerializeField] private float maxSize = 60;
    
    [SerializeField] private float maxLocalheight = 70;
    
    private Camera cam;
    
    private void Start() {
        cam = GetComponent<Camera>();
    }
    
    
    private  void Update() {
        
        if( target == null ) {
            return;
        }
        
        //MAJ de la taille
        float deltaY = target.position.y - groundY;
        cam.orthographicSize = Mathf.Clamp( deltaY, minSize, maxSize );
        
        
        //MAJ de la position
        Vector3 pos = target.transform.position;
        pos.z = transform.position.z;
        
        if( pos.y > groundY + maxSize ) {
            pos.y = Mathf.Max(groundY + maxSize, pos.y - maxLocalheight);
        }
        
        transform.position = pos;
        
    }
    
    
}
