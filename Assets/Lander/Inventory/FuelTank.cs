using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelTank : IinventoryItem
{
    
    private float _capacity;
    public float Volume {
        get{ return _capacity; }
    }
    
    
    private float _fuelQuantity;
    public float FuelQuantity
    {
        get { return _fuelQuantity; }
        set { _fuelQuantity = Mathf.Clamp(value, 0, _capacity); }
    }
    
    public float Mass
    {
        get { return _fuelQuantity * fuelDensity; }
    }
    
    
    private float fuelDensity = 1;
    
    
    
    public bool IsEmpty() {
        return FuelQuantity == 0;
    }
    
    public bool ConsumeFuel(float consumption)
    {
        FuelQuantity -= consumption;
        return !IsEmpty();
    }
    
    
    
    public FuelTank(float capacity = 1000, float fuelDensity = 1) : this(capacity, capacity, fuelDensity) { }
    
    public FuelTank(float capacity, float fuelQuantity, float fuelDensity)
    {
        _capacity = Mathf.Max(0, capacity);
        FuelQuantity = fuelQuantity;
        this.fuelDensity =fuelDensity;
    }
    
}
