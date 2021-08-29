using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelTank : InventoryItem
{
    
    private float _capacity;
    public override float Volume {
        get{ return _capacity; }
    }
    
    
    private float _fuelQuantity;
    public float FuelQuantity
    {
        get { return _fuelQuantity; }
        set { _fuelQuantity = Mathf.Clamp(value, 0, _capacity); }
    }
    
    public override float Mass
    {
        get { return _fuelQuantity * fuelDensity; }
    }

    public override string Name {
        get{ return "Fuel tank"; }
    }
    
    private float fuelDensity = 1;
    
    
    
    public override ItemBehaviour InstantiateWorldItem( LocalizedItem locItem )
    {
        GameObject instance = GameObject.Instantiate( TerrainManager.Instance.cratePref.gameObject );
        ItemBehaviour itemObj = instance.AddComponent<ItemBehaviour>();
        itemObj.item = locItem;
        
        return itemObj;
    }
    
    
    
    public bool IsEmpty() {
        return FuelQuantity == 0;
    }
    
    public bool ConsumeFuel(float consumption)
    {
        FuelQuantity -= consumption;
        return !IsEmpty();
    }
    
    public float Refuel( float qty ) {
        float oldQty = FuelQuantity;
        FuelQuantity += qty;
        return FuelQuantity - oldQty;
    }
    
    
    
    public FuelTank(float capacity = 1000, float fuelDensity = 1) 
        : this(capacity, capacity, fuelDensity) 
    { }
    
    
    public FuelTank(float capacity, float fuelQuantity, float fuelDensity)
    {
        _capacity = Mathf.Max(0, capacity);
        FuelQuantity = fuelQuantity;
        this.fuelDensity =fuelDensity;
    }
    
}
