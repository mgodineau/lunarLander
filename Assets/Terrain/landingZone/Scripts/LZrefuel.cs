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
        
        foreach( InventoryItem item in lander.Inventory.Items ){
            if( item is Crystal ) {
                entries.Add( new RefuelMenuEntry(item, lander) );
            }
        }
        
        return new SubMenu( "refuel station", entries );
    }
    

    protected override LZbehaviour PrefToInstantiate()
    {
        return TerrainManager.Instance.Prefabs.LzFuelPref;
    }



    private class RefuelMenuEntry : MenuEntry
    {
        private InventoryItem item;
        private Lander lander;
        
        public override void OnClick()
        {
            UImanager.Instance.lander.Inventory.RemoveItem(item);
            UImanager.Instance.lander.Tank.Refuel( item.Mass );
            lander.RefreshMainMenu();
        }
        
        public RefuelMenuEntry( InventoryItem item, Lander lander ) : base( "convert " + item.Name ) {
            this.item = item;
            this.lander = lander;
        }
    }

}