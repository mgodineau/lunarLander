using System.Collections.Generic;
using UnityEngine;


public class LZrocketBuilder : LandingZone
{
    
    private LevelParameters levelParams;
    private HashSet<RocketPart> rocketParts;
    
    
    public LZrocketBuilder(Vector3 position, LevelParameters levelParams) : base(position)
    {
        this.levelParams = levelParams;
        rocketParts = new HashSet<RocketPart>();
    }

    public override SubMenu GetMenu( Lander lander )
    {
        LinkedList<MenuEntry> entries = new LinkedList<MenuEntry>();
        foreach( InventoryItem item in lander.Inventory.Items ) {
            if( item is RocketPart ) {
                entries.AddLast( new AddRocketPartEntry(this, item as RocketPart) );
            }
        }
        
        return new SubMenu( "rocket builder", entries );
    }
    

    protected override LZbehaviour PrefToInstantiate()
    {
        return TerrainManager.Instance.Prefabs.LzRocketBuilderPref;
    }
    
    public void AddRocketPart( RocketPart rocketPart ) {
        rocketParts.Add(rocketPart);
        
        if(rocketParts.Count >= levelParams.RocketPartsCount) {
            //TODO fin du jeu
            Debug.Log("Rocket build !");
        }
    }
    
    
    
    private class AddRocketPartEntry : MenuEntry
    {
        LZrocketBuilder builder;
        private RocketPart rocketPart;
        
        public override void OnClick()
        {
            if( rocketPart.inventory != null ) {
                rocketPart.inventory.RemoveItem(rocketPart);
            }
            builder.AddRocketPart(rocketPart);
            
            UImanager.Instance.menuManager.ClearMenu();
        }
        
        public AddRocketPartEntry(LZrocketBuilder builder, RocketPart rocketPart ) 
            : base( "Add " + rocketPart.Name ) 
        {
            this.builder = builder;
            this.rocketPart = rocketPart;
        }
    }

}