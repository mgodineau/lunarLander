using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
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
    private AudioSource audioSource;
    
    
    [SerializeField]
    private float worldRotationSpeed = 10.0f;
    
    
    [SerializeField] private float destructionVelocity = 10;
    [SerializeField] private DestructionEffect destructionEffect;
    
    [SerializeField]
    private Transform _dropPosition;
    public Transform DropPosition {
        get {return _dropPosition;}
    }
    
    
    private InventoryManager _inventory;
    public InventoryManager Inventory {
        get{ return _inventory; }
    }
    private FuelTank _tank;
    public FuelTank Tank {
        get{ return _tank; }
    }
    
    private HashSet<ItemBehaviour> pickableItems = new HashSet<ItemBehaviour>();
    private List<MenuEntry> pickableItemEntries = new List<MenuEntry>();
    
    private LZbehaviour currentLZ = null;
    
    
    [SerializeField]
    private AudioClip clipThrust;
    
    private bool playSoundThrust = false;
    
    
    
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        
        EnableInputProcessing();
        
        _inventory = new InventoryManager( this, 4000 );
        
        _tank = new FuelTank(3000);
        _inventory.AddItem(_tank);
    }
    
    private void Start() {
        AddInstrument( UImanager.Instance.instrumentsManager.GetInstrumentInstance(InstrumentsManager.InstrumentType.Map) );
        AddInstrument( UImanager.Instance.instrumentsManager.GetInstrumentInstance(InstrumentsManager.InstrumentType.FuelGauge) );
    }
    
    
    private void FixedUpdate()
    {
        //MAJ de la masse du lander
        rb.mass = _inventory.Mass + minMass;
        
        
        //déplacement du lander
        if( ProcessInput() ) {
            //propulsion du lander
            bool thrustInput = Input.GetKey(KeyCode.Z) && _tank.ConsumeFuel( fuelConsumption * Time.fixedDeltaTime );
            if( thrustInput ) {
                rb.AddRelativeForce( Vector2.up * thrust, ForceMode2D.Force );
            }
            thrustAnim.SetBool("thrust", thrustInput);
            playSoundThrust = thrustInput;
            
            
            // rotation du lander
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
        
        
        
        // gestion du son
        if( playSoundThrust ) {
            if( audioSource.clip != clipThrust || !audioSource.isPlaying ) {
                audioSource.clip = clipThrust;
                audioSource.Play();
            }
        } else if ( audioSource.isPlaying && audioSource.clip == clipThrust ) {
            audioSource.Stop();
        }
        
        
        
        // affichage du menu
        if( ProcessInput() && Input.GetKeyDown(KeyCode.S) ) {
            SubMenu mainMenu = CreateMainMenu();
            UImanager.Instance.menuManager.SetMenu( mainMenu );
        }
        
    }
    
    
    private void OnCollisionEnter2D(Collision2D other) {
        
        
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
            UImanager.Instance.instrumentsManager.AddKnownObject(lz.LZscript);
            currentLZ = lz;
        }
        
    }
    
    private void OnCollisionExit2D( Collision2D other ) {
        if( currentLZ != null && currentLZ.gameObject == other.gameObject ) {
            currentLZ = null;
        }
    }
    
    
    
    private void OnTriggerEnter2D( Collider2D other ) {
        //ajout d'un objet a la liste de ramassage
        ItemBehaviour item = other.gameObject.GetComponent<ItemBehaviour>();
        if( item != null ) {
            pickableItems.Add(item);
            
            UpdatePickableItemEntries();
            UImanager.Instance.menuManager.UpdateMenuUI();
        }
        
    }
    
    private void OnTriggerExit2D( Collider2D other ) {
        //ajout d'un objet a la liste de ramassage
        ItemBehaviour item = other.gameObject.GetComponent<ItemBehaviour>();
        if( item != null ) {
            pickableItems.Remove(item);
            
            UpdatePickableItemEntries();
            UImanager.Instance.menuManager.UpdateMenuUI();
        }
    }
    
    
    
    private void DestroyLander() {
        
        GameObject.Instantiate( destructionEffect ).InitEffect(rb);
        UImanager.Instance.instrumentsManager.gameObject.SetActive(false);
        
        //Destruction de l'objet lander
        Destroy(gameObject);
    }
    
    
    
    
    public bool AddInstrument(InstrumentBehaviour instrument) {
        return _inventory.AddItem(instrument.GetInstrumentItem());
    }
    
    public bool RemoveInstrument( InstrumentBehaviour instrument ) {
        return _inventory.RemoveItem(instrument.GetInstrumentItem());
    }
    
    
    public float GetFuelQuantity() {
        return _tank.FuelQuantity;
    }
    
    public float GetFuelCapacity() {
        return _tank.Volume;
    }
    
    
    public void RefreshMainMenu() {
        if( UImanager.Instance.menuManager.GetCurrentMenu() != null ) {
            UImanager.Instance.menuManager.SetMenu( CreateMainMenu() );
        }
    }
    
    private SubMenu CreateMainMenu() {
        
        List<MenuEntry> menuContent = new List<MenuEntry>();
        menuContent.Add( CreatePickupMenu() );
        menuContent.Add( _inventory.GetMenu() );
        if( currentLZ != null ) {
            menuContent.Add( currentLZ.LZscript.GetMenu(this) );
        }
        
        return new SubMenu("menu", menuContent);        
    }
    
    
    private SubMenu CreatePickupMenu() {
        UpdatePickableItemEntries();
        return new SubMenu("pickup", pickableItemEntries);
    }
    
    private void UpdatePickableItemEntries() {
        pickableItemEntries.Clear();
        foreach( ItemBehaviour crystal in pickableItems ) {
            pickableItemEntries.Add( new MenuEntryPickupItem(_inventory, crystal) );
        }
        
    }
    
}
