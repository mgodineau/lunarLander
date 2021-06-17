using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentsManager : MonoBehaviour
{
    
    
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
    public enum InstrumentType : int { Map, FuelGauge }


    public void Awake()
    {
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
        if(UImanager.Instance.lander.AddInstrument(instrument) ) {
            instrument.gameObject.SetActive(true);
        }
    }
    
    public void DisableInstrument( Instrument instrument ) {
        instrument.gameObject.SetActive(false);
        UImanager.Instance.lander.RemoveInstrument(instrument);
    }
    

}
