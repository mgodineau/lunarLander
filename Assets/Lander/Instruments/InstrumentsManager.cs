using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentsManager : MonoBehaviour
{
    private static InstrumentsManager _instance;
    public static InstrumentsManager Instance
    {
        get { return _instance; }
    }
    
    
    [SerializeField] private Transform _lander;
    public Transform Lander
    {
        get { return _lander; }
    }
    
    
    
    [SerializeField] private RectTransform _canvasRect;
    public RectTransform CanvasRect {
        get{ return _canvasRect; }
    }
    
    private List<LandingZone> _knownLZ = new List<LandingZone>();
    public List<LandingZone> KnownLZ {
        get{ return _knownLZ; }
    }
    
    
    public enum InstrumentType { Map }


    public void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        AddInstrument(InstrumentType.Map);
    }



    void AddInstrument(InstrumentType instrument)
    {
        //TODO
    }

}
