using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UImanager : MonoBehaviour
{
    
    private static UImanager _instance;
    public static UImanager Instance {
        get { return _instance; }
    }
    
    
    public Lander lander;
    
    public MenuManager menuManager;
    public InstrumentsManager instrumentsManager;
    
    
    
    private void Awake() {
        _instance = this;
    }
    
    
    
    
    
}
