using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LZradar : LandingZone
{
	
	private float _scanRadiusDeg = 30;
	public float ScanRadiusDeg {
		get{return _scanRadiusDeg;}
		set{_scanRadiusDeg = Mathf.Max(0.0f, value);}
	}
	
	public LZradar(Vector3 position) : base(position) {
		
	}
	
	public override SubMenu GetMenu(Lander lander)
	{
		List<MenuEntry> menuContent = new List<MenuEntry>();
		menuContent.Add( new RadarScanMenuEntry(this) );
		
		return new SubMenu("radar station", menuContent);
	}

	protected override LZbehaviour PrefToInstantiate()
	{
		return TerrainManager.Instance.Prefabs.LzRadarPref;
	}



    private class RadarScanMenuEntry : MenuEntry
    {
		private LZradar linkedRadar;
		
		public RadarScanMenuEntry(LZradar linkedRadar) : base("scan area") {
			this.linkedRadar = linkedRadar;
		}
		
		
        public override void OnClick()
        {
			PlanetGen planet = TerrainManager.Instance.Planet;
			
			List<LocalizedObject> objects = planet.getObjects();
			
			LinkedList<LocalizedObject> detectedObject = new LinkedList<LocalizedObject>();
			foreach( LocalizedObject obj in objects ) {
				float currentObjAngle = Vector3.Angle( obj.Position, linkedRadar.Position );
				if( currentObjAngle <= linkedRadar.ScanRadiusDeg ) {
					detectedObject.AddLast( obj );
				}
			}
			
			
			UImanager.Instance.instrumentsManager.AddKnownObjects(detectedObject);
			
			UImanager.Instance.menuManager.ClearMenu();
        }
    }


}
