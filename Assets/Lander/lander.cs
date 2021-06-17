using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Lander : InputConsumer
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
    
    
    private InventoryManager inventory = new InventoryManager( 4000 );
    
    private FuelTank tank = new FuelTank(3000);
    
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        
        EnableInputProcessing();
        inventory.AddItem(tank);
    }
    
    private void Start() {
        UImanager.Instance.instrumentsManager.EnableInstrument( InstrumentsManager.InstrumentType.Map );
        UImanager.Instance.instrumentsManager.EnableInstrument( InstrumentsManager.InstrumentType.FuelGauge );
    }
    
    
    private void FixedUpdate()
    {
        //MAJ de la masse du lander
        rb.mass = inventory.Mass + minMass;
        
        
        //déplacement du lander
        if( ProcessInput() ) {
            bool thrustInput = Input.GetKey(KeyCode.Z) && tank.ConsumeFuel( fuelConsumption * Time.fixedDeltaTime );
            if( thrustInput ) {
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
        
        
        
        // affichage du menu
        if( ProcessInput() && Input.GetKeyDown(KeyCode.S) ) {
            SubMenu mainMenu = CreateMainMenu();
            UImanager.Instance.menuManager.SetMenu( mainMenu );
        }
        
    }
    
    
    private void OnCollisionEnter2D(Collision2D other) {
        
        
        //ramassage automatique d'un objet
        CrystalBehaviour crystal = other.gameObject.GetComponent<CrystalBehaviour>();
        if( crystal != null && inventory.AddItem(crystal.crystalScript)) {
            crystal.Pickup();
            return;
        }
        
        
        //gestion de la destruction du Lander
        Vector3 relativeVelocity = other.relativeVelocity;
        float impactCos = relativeVelocity.normalized.x;
        
        if( relativeVelocity.magnitude * Mathf.Abs(impactCos) > destructionVelocity ) {
            DestroyLander();
            return;
        }
        
        
        //gestion de l'atterrissage sur une LZ
        LZbehaviour lz = other.gameObject.GetComponent<LZbehaviour>();
        if( lz != null ) {
            UImanager.Instance.instrumentsManager.KnownLZ.Add(lz.LZscript);
            return;
        }
        
        
    }
    
    
    
    private void DestroyLander() {
        
        GameObject.Instantiate( destructionEffect ).InitEffect(rb);
        UImanager.Instance.instrumentsManager.gameObject.SetActive(false);
        
        //Destruction de l'objet lander
        Destroy(gameObject);
    }
    
    
    
    
    public bool AddInstrument(Instrument instrument) {
        return inventory.AddItem(instrument);
    }
    
    public bool RemoveInstrument( Instrument instrument ) {
        return inventory.RemoveItem(instrument);
    }
    
    
    public float GetFuelQuantity() {
        return tank.FuelQuantity;
    }
    
    public float GetFuelCapacity() {
        return tank.Volume;
    }
    
    
    private SubMenu CreateMainMenu() {
        
        List<MenuEntry> menuContent = new List<MenuEntry>();
        menuContent.Add( inventory.GetMenu() );
        
        return new SubMenu("menu", menuContent);
        
    }
    
    
    
}
