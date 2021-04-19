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
    [SerializeField] private Transform _lander;
    public Transform Lander
    {
        get { return _lander; }
    }
    
    
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
    
        
    //types des instruments possibles
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
