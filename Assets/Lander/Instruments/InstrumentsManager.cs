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
    [SerializeField] private PlanetGen _planet;
    public PlanetGen Planet
    {
        get { return _planet; }
    }
    [SerializeField] private RectTransform _canvasRect;
    public RectTransform CanvasRect {
        get{ return _canvasRect; }
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
