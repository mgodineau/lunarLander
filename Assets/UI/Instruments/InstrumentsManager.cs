using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentsManager : MonoBehaviour
{
    
    
    //Accès à tous les instruments
    [SerializeField] private InstrumentBehaviour[] instrumentsInstances;
    
    
    
    
    //rectangle du canvas global dans lequel les instruments se trouvent
    [SerializeField] private RectTransform _canvasRect;
    public RectTransform CanvasRect {
        get{ return _canvasRect; }
    }
    
    
    //zones d'atterrissage connues
    private HashSet<LandingZone> _knownLZ = new HashSet<LandingZone>();
    public HashSet<LandingZone> KnownLZ {
        get{ return _knownLZ; }
    }
    
        
    // types des instruments possibles
    public enum InstrumentType : int { Map, FuelGauge }


    public void Awake()
    {
        foreach( InstrumentBehaviour inst in instrumentsInstances ) {
            inst.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
    }
    
    public InstrumentBehaviour GetInstrumentInstance( InstrumentType type ) {
        return instrumentsInstances[(int)type];
    }
    
    
    public void EnableInstrument(InstrumentType instrument)
    {
        EnableInstrument( instrumentsInstances[(int)instrument] );
    }

    public void EnableInstrument(InstrumentBehaviour instrument)
    {
        // if(UImanager.Instance.lander.AddInstrument(instrument) ) {
            instrument.gameObject.SetActive(true);
        // }
    }
    
    public void DisableInstrument( InstrumentBehaviour instrument ) {
        instrument.gameObject.SetActive(false);
        UImanager.Instance.lander.RemoveInstrument(instrument);
    }
    

}
