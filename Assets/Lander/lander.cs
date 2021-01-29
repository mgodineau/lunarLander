using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class lander : MonoBehaviour
{
    
    [SerializeField]
    private float thrust = 1.0f;
    [SerializeField]
    private float angularThrust = 1.0f;
    
    [SerializeField]
    private Animator thrustAnim;
    
    private Rigidbody2D rb;
    
    
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
}
