using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Lander : MonoBehaviour
{
    
    [SerializeField]
    private float thrust = 1.0f;
    [SerializeField]
    private float angularThrust = 1.0f;
    
    [SerializeField]
    private Animator thrustAnim;
    
    private Rigidbody2D rb;
    
    
    [SerializeField]
    private float worldRotationSpeed = 10.0f;
    
    
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void FixedUpdate()
    {
        bool thrustInput = Input.GetKey(KeyCode.Z);
        if( thrustInput) {
            rb.AddRelativeForce( Vector2.up * thrust, ForceMode2D.Force );
        }
        thrustAnim.SetBool("thrust", thrustInput);
        
        float torqueDir = 0;
        if( Input.GetKey(KeyCode.Q) ) {
            torqueDir++;
        }
        if( Input.GetKey(KeyCode.D) ) {
            torqueDir--;
        }
        rb.AddTorque(angularThrust * torqueDir, ForceMode2D.Force);
    }
    
    
    
    private void Update() {
        
        float rotationDir = 0;
        if( Input.GetKey(KeyCode.A) ) {
            rotationDir++;
        }
        if( Input.GetKey(KeyCode.E) ) {
            rotationDir--;
        }
        
        if( rotationDir != 0 ) {
            TerrainManager.Instance.RotateAround( transform.position.x, rotationDir * worldRotationSpeed * Time.deltaTime );
        }
    }
    
    
    private void OnCollisionEnter2D(Collision2D other) {
        
        
        LZbehaviour lz = other.gameObject.GetComponent<LZbehaviour>();
        if( lz != null ) {
            Debug.Log("Collision with : " + other.gameObject.name);
            InstrumentsManager.Instance.KnownLZ.Add(lz.LZscript);
        }
        
    }
    
}
