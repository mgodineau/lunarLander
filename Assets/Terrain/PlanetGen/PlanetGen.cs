using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGen : MonoBehaviour
{
	[SerializeField] private int crystalsCount = 1000;
	[SerializeField] private int craterCount = 5;
	
	[SerializeField] private LevelParameters _levelParams;
	public LevelParameters LevelParams{
		get {return _levelParams;}
	}
	
	public List<TerrainLayer> layers = new List<TerrainLayer>();
	
	private List<LandingZone> landingZones = new List<LandingZone>();
	private List<LocalizedItem> items = new List<LocalizedItem>();
	
	private LinkedList<IObjectsView> _objectsViews = new LinkedList<IObjectsView>();
	
	
	private void Awake() {
		layers.Add( new RandomLayer(1, 30) );
		layers.Add( new RandomLayer(0.1f, 5) );
		
		for( int i=0; i<craterCount; i++ ) {
			layers.Add( new Crater(Random.onUnitSphere, Random.Range(0.5f, 1.0f)) );
		}
		
		landingZones.Add( new LZrefuel(Vector3.right) );
		landingZones.Add( new LZrefuel(Vector3.up) );
		landingZones.Add( new LZradar(Vector3.forward) );
		landingZones.Add( new LZradar(Vector3.back) );
		landingZones.Add( new LZrocketBuilder(Vector3.forward + Vector3.right*0.2f, _levelParams) );
		
		generateRocketParts();
	}
	
	private void Start() {
		UImanager.Instance.instrumentsManager.AddKnownObjects(getObjects());	//tmp, pour debug
	}
	
	
	internal void RemoveObject(LocalizedObject obj)
	{
		if( obj is LocalizedItem ) {
			items.Remove( obj as LocalizedItem );
		} else if( obj is LandingZone ) {
			landingZones.Remove(obj as LandingZone);
		}
		NotifyRemoveObject(obj);
	}
	
	
	public void RemoveItem(InventoryItem item)
	{
		foreach( LocalizedItem locItem in items) {
			if( locItem.Item == item ) {
				RemoveObject(locItem);
				return;
			}
		}
	}
	
	private void NotifyRemoveObject( LocalizedObject obj ) {
		foreach( IObjectsView view in _objectsViews ) {
			view.RemoveObject(obj);
		}
	}
	
	private void NotifyAddObject( LocalizedObject obj ) {
		foreach( IObjectsView view in _objectsViews ) {
			view.AddObject(obj);
		}
	}
	
	public void NotifyUpdateObject(LocalizedObject obj)
	{
		foreach( IObjectsView view in _objectsViews ) {
			view.UpdateObject(obj);
		}
	}
	
	
	public void AddItem(InventoryItem item, Vector3 spherePosition)
	{
		AddItem( new LocalizedItem(item, spherePosition) );
	}
	
	public LocalizedItem AddItem(InventoryItem item, Vector3 spherePosition, float height, float rotation=0)
	{
		LocalizedItem locItem = new LocalizedItem(item, spherePosition, height, rotation);
		AddItem( locItem );
		return locItem;
	}
	
	public void AddItem( LocalizedItem item ) {
		items.Add(item);
		NotifyAddObject(item);
	}
	
	public List<LocalizedObject> getObjects() {
		List<LocalizedObject> objects = new List<LocalizedObject>();
		objects.AddRange(landingZones);
		objects.AddRange( items );
		
		return objects;
	}
	
	public List<LandingZone> getLandingZones() {
		return landingZones;
	}
	
	public List<LocalizedItem> GetItems() {
		return items;
	}
	
	private void generateCrystals() {
		
		for( int i=0; i<crystalsCount; i++ ) {
			AddItem( new Crystal(), UnityEngine.Random.onUnitSphere  );
		}
	}
	
	private void generateRocketParts() {
		
		for( int i=0; i<LevelParams.RocketPartsCount; i++ ) {
			AddItem( new RocketPart(), UnityEngine.Random.onUnitSphere );
		}
		
	}
	
	
	public float GetHeight( Vector3 position ) {
		float height = 0;
		foreach( TerrainLayer layer in layers ) {
			height += layer.GetHeight(position);
		}
		return height;
	}
	
	
	public float GetMaxHeight() {
		float maxHeight = -Mathf.Infinity;
		foreach( TerrainLayer layer in layers ) {
			maxHeight = Mathf.Max(layer.MaxHeight);
		}
		return maxHeight * 1.5f; //TODO trouver une meilleure méthode
	}
	
	
	internal float GetMinHeight()
	{
		float minHeight = Mathf.Infinity;
		foreach( TerrainLayer layer in layers ) {
			minHeight = Mathf.Min(layer.MinHeight);
		}
		return minHeight;
	}
	
	
	private void OnValidate() {
		foreach( TerrainLayer layer in layers ) {
			layer.OnValidate();
		}
	}

	
	public void AddObjectsView( IObjectsView view ) {
		view.SetObjectsCollection( getObjects() );
		_objectsViews.AddLast(view);
	}

	
	
}
