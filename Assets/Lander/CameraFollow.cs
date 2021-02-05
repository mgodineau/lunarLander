using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    
    
    public Transform target;
    
    
    private void Update()
    {
        Vector3 pos = target.transform.position;
        pos.z = transform.position.z;
        transform.position = pos;
    }
}
