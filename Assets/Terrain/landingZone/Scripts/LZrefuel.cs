using System.Collections.Generic;
using UnityEngine;


public class LZrefuel : LandingZone
{
    
    public LZrefuel(Vector3 position) : base(position)
    {
    }

    public override SubMenu GetMenu( Lander lander )
    {
        List<MenuEntry> entries = new List<MenuEntry>();
        
        entries.Add( new RefuelMenuEntry(lander) );
        
        return new SubMenu( "refuel station", entries );
    }
    

    protected override LZbehaviour PrefToInstantiate()
    {
        return TerrainManager.Instance.Prefabs.LzFuelPref;
    }



    private class RefuelMenuEntry : MenuEntry
    {
        private Lander lander;
        
        public override void OnClick()
        {
            float quantity = lander.Tank.Volume - lander.Tank.FuelQuantity;
            lander.Tank.Refuel( quantity );
            
            UImanager.Instance.menuManager.ClearMenu();
        }
        
        public RefuelMenuEntry(Lander lander ) : base( "Refuel" ) {
            this.lander = lander;
        }
    }

}