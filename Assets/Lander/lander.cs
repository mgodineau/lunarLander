using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Lander : MonoBehaviour
{
    
    [SerializeField]
    private float minMass = 1.0f;
    
    
    [SerializeField]
    private float thrust = 1.0f;
    [SerializeField]
    private float angularThrust = 1.0f;
    
    [SerializeField]
    private float fuelConsumption = 10.0f;
    
    [SerializeField]
    private Animator thrustAnim;
    private Rigidbody2D rb;
    
    
    [SerializeField]
    private float worldRotationSpeed = 10.0f;
    
    
    [SerializeField] private float destructionVelocity = 10;
    [SerializeField] private DestructionEffect destructionEffect;
    
    
    private InventoryManager inventory = new InventoryManager();
    private FuelTank tank = new FuelTank(5000);
    
    
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        
        inventory.AddItem(tank);
    }
    
    private void Start() {
        InstrumentsManager.Instance.EnableInstrument( InstrumentsManager.InstrumentType.Map );
    }
    
    
    private void FixedUpdate()
    {
        //MAJ de la masse du lander
        rb.mass = inventory.Mass + minMass;
        
        
        //déplacement du lander
        bool thrustInput = Input.GetKey(KeyCode.Z);
        if( thrustInput && tank.ConsumeFuel( fuelConsumption * Time.fixedDeltaTime ) ) {
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
        
        
        // rotation du terrain
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
        
        
        Debug.Log( "fuel = " + tank.FuelQuantity );
    }
    
    
    private void OnCollisionEnter2D(Collision2D other) {
        
        //gestion de la destruction du Lander
        Vector3 relativeVelocity = other.relativeVelocity;
        float impactCos = relativeVelocity.normalized.x;
        
        if( relativeVelocity.magnitude * Mathf.Abs(impactCos) > destructionVelocity ) {
            DestroyLander();
        }
        
        
        //gestion de l'atterrissage sur une LZ
        LZbehaviour lz = other.gameObject.GetComponent<LZbehaviour>();
        if( lz != null ) {
            InstrumentsManager.Instance.KnownLZ.Add(lz.LZscript);
        }
        
    }
    
    
    private void DestroyLander() {
        
        GameObject.Instantiate( destructionEffect ).InitEffect(rb);
        InstrumentsManager.Instance.gameObject.SetActive(false);
        
        //Destruction de l'objet lander
        Destroy(gameObject);
    }
    
    
    
    
    public bool AddInstrument(Instrument instrument) {
        return inventory.AddItem(instrument);
    }
    
    public bool RemoveInstrument( Instrument instrument ) {
        return inventory.RemoveItem(instrument);
    }
    
    
}
