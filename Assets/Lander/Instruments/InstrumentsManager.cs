using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentsManager : MonoBehaviour
{
    //instrument Manager est un singleton
    private static InstrumentsManager _instance;
    public static InstrumentsManager Instance
    {
        get { return _instance; }
    }
    
    
    //instance de Lander controllée par le joueur
    [SerializeField] private Lander _currentLander;
    public Lander CurrentLander
    {
        get { return _currentLander; }
    }
    
    
    
    //Accès à tous les instruments
    [SerializeField] private Instrument[] instrumentsInstances;
    
    
    
    
    //rectangle du canvas global dans lequel les instruments se trouvent
    [SerializeField] private RectTransform _canvasRect;
    public RectTransform CanvasRect {
        get{ return _canvasRect; }
    }
    
    
    //zones d'atterrissage connues
    private List<LandingZone> _knownLZ = new List<LandingZone>();
    public List<LandingZone> KnownLZ {
        get{ return _knownLZ; }
    }
    
        
    // types des instruments possibles
    public enum InstrumentType : int { Map }


    public void Awake()
    {
        _instance = this;
        foreach( Instrument inst in instrumentsInstances ) {
            inst.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
    }

    
    
    public void EnableInstrument(InstrumentType instrument)
    {
        EnableInstrument( instrumentsInstances[(int)instrument] );
    }

    public void EnableInstrument(Instrument instrument)
    {
        if(_currentLander.AddInstrument(instrument) ) {
            instrument.gameObject.SetActive(true);
        }
    }
    
    public void DisableInstrument( Instrument instrument ) {
        instrument.gameObject.SetActive(false);
        _currentLander.RemoveInstrument(instrument);
    }
    

}
