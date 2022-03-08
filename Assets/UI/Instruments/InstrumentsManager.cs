using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentsManager : MonoBehaviour, IObjectsView
{
	
	
	//Accès à tous les instruments
	[SerializeField] private InstrumentBehaviour[] instrumentsInstances;
	
	
	
	
	//rectangle du canvas global dans lequel les instruments se trouvent
	[SerializeField] private RectTransform _canvasRect;
	public RectTransform CanvasRect {
		get{ return _canvasRect; }
	}
	
	
	//zones d'atterrissage connues
	private HashSet<LocalizedObject> _knownObjects = new HashSet<LocalizedObject>();
	public HashSet<LocalizedObject> KnownObjects {
		get{ return _knownObjects; }
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
		TerrainManager.Instance.Planet.AddObjectsView(this);
	}
	
	
	public void SetObjectsCollection(IEnumerable<LocalizedObject> objects)
	{
		//Update every instruments
		foreach( InstrumentBehaviour instrument in instrumentsInstances ) {
			if( instrument is IObjectsView ) {
				(instrument as IObjectsView).SetObjectsCollection(objects);
			}
		}
		
		HashSet<LocalizedObject> objects_hash = objects is HashSet<LocalizedObject> ? (objects as HashSet<LocalizedObject>) : new HashSet<LocalizedObject>(objects);
		LinkedList<LocalizedObject> objectsToRemove = new LinkedList<LocalizedObject>();
		foreach( LocalizedObject obj in _knownObjects ) {
			if( !objects_hash.Contains(obj) ) {
				objectsToRemove.AddLast(obj);
			}
		}
		_knownObjects.RemoveWhere( (LocalizedObject obj) => {return !objects_hash.Contains(obj);} );
	}
	
	public void UpdateObject(LocalizedObject obj)
    {
        foreach( InstrumentBehaviour instrument in instrumentsInstances ) {
			if( instrument is IObjectsView ) {
				(instrument as IObjectsView).UpdateObject(obj);
			}
		}
    }
	
	public void AddObject(LocalizedObject obj) {}

	public void RemoveObject(LocalizedObject obj)
	{
		_knownObjects.Remove(obj);
		foreach( InstrumentBehaviour instrument in instrumentsInstances ) {
			if( instrument is IObjectsView ) {
				(instrument as IObjectsView).RemoveObject(obj);
			}
		}
	}
	
	
	
	public void AddKnownObjects(IEnumerable<LocalizedObject> knownObjects) {
		foreach( LocalizedObject obj in knownObjects ) {
			AddKnownObject(obj);
		}
	}
	
	public void AddKnownObject(LocalizedObject obj) {
		KnownObjects.Add(obj);
		foreach( InstrumentBehaviour instrument in instrumentsInstances ) {
			if( instrument is IObjectsView ) {
				(instrument as IObjectsView).AddObject(obj);
			}
		}
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
		instrument.gameObject.SetActive(true);
	}

	

	public void DisableInstrument( InstrumentBehaviour instrument ) {
		instrument.gameObject.SetActive(false);
		UImanager.Instance.lander.RemoveInstrument(instrument);
	}

    
}
