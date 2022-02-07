using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Map : InstrumentBehaviour
{
    
    [SerializeField] private int linesResolution = 10;
    [SerializeField] private int textureResolution = 50;
    
    [SerializeField] private float iconWidth = 0.01f;
    
    [SerializeField] private float sliceDelta = 10.0f;
    
    [SerializeField] private Color landerColor = Color.red;
    [SerializeField] private Color lzRefuelColor = Color.yellow;
    [SerializeField] private Color lzRadarColor = Color.blue;
    [SerializeField] private Color rocketPartColor = Color.green;
    
    
    private Image image;
    
    //Layout
    LineData bgVertical = new LineData();
    LineData bgHorizontal = new LineData();
    
    // informations
    LineData landerPos;   //position du lander
    LineData sliceLine;   //section visitée
    Dictionary<LocalizedObject, LineData> knownObjectToMarker = new Dictionary<LocalizedObject, LineData>();
    
    public override float Mass {
        get{ return 1; }
    }

    public override float Volume {
        get{ return 1; }
    }
    
    public override string Name {
        get{ return "Map"; }
    }


    new private void Awake() {
        base.Awake();
        
        landerPos = new LineData(landerColor);
        sliceLine = new LineData(landerColor);
        
        image = GetComponent<Image>();
    }
    
    new private void Start() {
        base.Start();
        
        BuildLayout();
        
        EnableLayout();
        EnableInfos();
        
        UpdateHeightmap();
    }
    
    
    new private void OnEnable() {
        base.OnEnable();
        
        EnableLayout();
        EnableInfos();
    }
    
    new private void OnDisable() {
        base.OnDisable();
        
        DisableLayout();
        DisableInfos();
    }
    
    
    
    private void Update() {
        UpdateLanderLocation();
        UpdateSliceLine();
        
        UpdateMarkers();
    }
    
    
    /// <summary>
    /// MAJ de la position du marqueur du lander
    /// </summary>
    private void UpdateLanderLocation() {
        Vector3 landerDir = TerrainManager.Instance.ConvertXtoDir(
            UImanager.Instance.lander.transform.position.x
        );
        Vector3 landerLocalCenter = LocalToGlobal( dirToLocalPos(landerDir) );
        landerPos.points = CreateSquareLine( landerLocalCenter, iconWidth );
    }
    
    /// <summary>
    /// MAJ de la section visible
    /// </summary>
    private void UpdateSliceLine() {
        sliceLine.points.Clear();
        LinkedList<Vector3> linePoints = new LinkedList<Vector3>();
        
        Vector3 sliceNormal = TerrainManager.Instance.SliceNormal;
        if( sliceNormal.y < 0 ) {
            sliceNormal = -sliceNormal;
        }
        Vector3 originDir = TerrainManager.Instance.SliceOrigin;
        Vector3 originLocalPos = dirToLocalPos(originDir);
        linePoints.AddLast(  LocalToGlobal(originLocalPos) );
        
        Vector2 sampleLocalPos = dirToLocalPos( Quaternion.AngleAxis(sliceDelta, sliceNormal) * originDir );
        Vector2 prevSample = dirToLocalPos(originDir);
        int i=2;
        while( sampleLocalPos.x > originLocalPos.x ) {
            linePoints.AddLast(LocalToGlobal(sampleLocalPos) );
            prevSample = sampleLocalPos;
            sampleLocalPos = dirToLocalPos( Quaternion.AngleAxis(sliceDelta * i, sliceNormal) * originDir );
            i++;
        }
        
        //ajout du dernier point, calé sur le bord droit de la map
        sampleLocalPos.x += 1;
        sampleLocalPos -= (sampleLocalPos - prevSample) * (sampleLocalPos.x - 1.0f) / (sampleLocalPos.x - prevSample.x);
        linePoints.AddLast( LocalToGlobal(sampleLocalPos) );
        
        
        i = 2;
        sampleLocalPos = dirToLocalPos( Quaternion.AngleAxis(-sliceDelta, sliceNormal) * originDir );
        prevSample = dirToLocalPos(originDir);
        while( sampleLocalPos.x < originLocalPos.x ) {
            linePoints.AddFirst(LocalToGlobal(sampleLocalPos) );
            prevSample = sampleLocalPos;
            sampleLocalPos = dirToLocalPos( Quaternion.AngleAxis(-sliceDelta * i, sliceNormal) * originDir );
            i++;
        }
        
        //ajout du dernier point, calé sur le bord gauche de la map
        sampleLocalPos.x -= 1;
        sampleLocalPos -= (sampleLocalPos - prevSample) * sampleLocalPos.x / (sampleLocalPos.x - prevSample.x);
        linePoints.AddFirst( LocalToGlobal(sampleLocalPos) );
        
        
        //ajout de la nouvelle ligne au rendu
        sliceLine.points.AddRange( linePoints );
    }
    
    
    
    
    private void UpdateMarkers()
    {
        
        foreach( LocalizedObject currentObj in UImanager.Instance.instrumentsManager.KnownObjects ) {
            
            //creation d'un marker si besoin
            if( !knownObjectToMarker.ContainsKey(currentObj) ) {
                
                LineData line = new LineData( GetMarkerColor(currentObj) );
                
                WireframeRender.Instance.linesUI.Add(line);
                knownObjectToMarker.Add(currentObj, line );
                
            }
            
            //MAJ de la position du parker
            knownObjectToMarker[currentObj].points = CreateSquareLine(
                LocalToGlobal(dirToLocalPos(currentObj.Position)) , 
                TerrainManager.Instance.IsObjectVisible( currentObj ) ? iconWidth : iconWidth / 2
            );
            
        }
        
    }
    
    private Color GetMarkerColor( LocalizedObject obj ) {
        Color color = landerColor;
        if( obj is LZradar ) {
            color = lzRadarColor;
        } else if( obj is LZrefuel ) {
            color = lzRefuelColor;
        } else if( obj is LocalizedItem) {
            color = rocketPartColor;
        }
        return color;
    }
    
    
    
    private List<Vector3> CreateSquareLine( Vector3 localCenter, float width ) {
        List<Vector3> path = new List<Vector3>();
        float height = width * Screen.width / Screen.height;
        path.Add( localCenter + Vector3.up * height );
        path.Add( localCenter + Vector3.right * width );
        path.Add( localCenter - Vector3.up * height );
        path.Add( localCenter - Vector3.right * width );
        path.Add( localCenter + Vector3.up * height );
        
        return path;
    }
    
    
    
    /// <summary>
    /// MAJ de l'arrière plan e la carte, qui représente la heightmap
    /// </summary>
    private void UpdateHeightmap() {
        
        float maxHeight = TerrainManager.Instance.Planet.GetMaxHeight();
        float minHeight = TerrainManager.Instance.Planet.GetMinHeight();
        
        Color[] colors = new Color[textureResolution * textureResolution * 2];
        for( int u=0; u<textureResolution*2; u++ ) {
            for( int v=0; v<textureResolution; v++ ) {
                Vector2 localPos = new Vector2( (float)u / (textureResolution*2), (float)v / textureResolution );
                float height = TerrainManager.Instance.Planet.GetHeight( localPosToDir(localPos) );
                float brightness = (height - minHeight) / (maxHeight - minHeight);
                colors[ v * textureResolution*2 + u ] = new Color(brightness, brightness, brightness, 1 );
            }
        }
        
        Texture2D tex2D = new Texture2D( textureResolution*2, textureResolution );
        tex2D.SetPixels( colors, 0 );
        tex2D.Apply();
        
        Sprite sprite = Sprite.Create( tex2D, new Rect(0, 0, textureResolution*2, textureResolution), Vector2.one * 0.5f );
        image.sprite = sprite;
    }
    
    
    /// <summary>
    /// conversion d'une direction dans le repère global vers une position locale en 2D sur la carte
    /// </summary>
    /// <param name="dir"> La direction à convertir (normalisé) </param>
    /// <returns> La position de dir sur la carte </returns>
    private Vector2 dirToLocalPos( Vector3 dir ) {
        
        float localX = Mathf.Sqrt( 1.0f - dir.y*dir.y );
        float yAngle = Mathf.Atan2( dir.y, localX );
        
        float xAngle = Vector2.SignedAngle( new Vector2(dir.x, dir.z),  Vector2.right );
        
        return new Vector2( (xAngle / 360.0f) +0.5f , yAngle / Mathf.PI + 0.5f );
    }
    
    
    /// <summary>
    /// Conversion d'une position sur la carte vers une direction dans le repère global
    /// </summary>
    /// <param name="localPos"> Une position locale en 2D sur la carte </param>
    /// <returns> La direction normalisé qui correspond à localPos </returns>
    private Vector3 localPosToDir( Vector2 localPos ) {
        Vector3 dir = -Vector3.right;
        dir = Quaternion.AngleAxis( (localPos.y - 0.5f) * 180.0f, -Vector3.forward) * dir;
        dir = Quaternion.AngleAxis( localPos.x * 360.0f, Vector3.up) * dir;
        return dir;
    }
    
    /// <summary>
    /// Construction des lignes intérieures de la carte, stockées dans bgHorizontal et bgVertical
    /// </summary>
    public void BuildLayout() {
        bgHorizontal.points.Clear();
        bgVertical.points.Clear();
        
        //lignes horizontales
        float height = 1.0f / linesResolution;
        bool left = true;
        for( int i=1; i<=linesResolution; i++ ) {
            float x = left ? 0 : 1;
            left = !left;
            
            bgHorizontal.points.Add( LocalToGlobal( new Vector3( x, height * (i-1), 0) ));
            bgHorizontal.points.Add( LocalToGlobal( new Vector3(x, height * i, 0) ));
        }
        
        //lignes verticales
        float width = 0.5f / linesResolution;
        bool down = true;
        for( int i=1; i<=linesResolution*2; i++ ) {
            float y = down ? 0 : 1;
            down = !down;
            
            bgVertical.points.Add( LocalToGlobal( new Vector3( width * (i-1), y, 0) ));
            bgVertical.points.Add( LocalToGlobal( new Vector3( width * i, y, 0) ));
        }
    }
    
    
    
    /// <summary>
    /// Active l'affichage des lignes d'arrière plan
    /// </summary>
    public void EnableLayout() {
        WireframeRender wireframeRender = WireframeRender.Instance;
        if(wireframeRender == null) {
            return;
        }
        wireframeRender.linesUI.Add( bgVertical );
        wireframeRender.linesUI.Add( bgHorizontal );
    }
    
    
    /// <summary>
    /// Désactive l'affichage des lignes d'arrière plan
    /// </summary>
    public void DisableLayout() {
        WireframeRender wireframeRender = WireframeRender.Instance;
        if(wireframeRender == null) {
            return;
        }
        wireframeRender.linesUI.Remove( bgVertical );
        wireframeRender.linesUI.Remove( bgHorizontal );
    }
    
    /// <summary>
    /// Active l'affichage des informations sur la carte, telles que la position du lander, ou la section visitée
    /// </summary>
    public void EnableInfos() {
        WireframeRender wireframeRender = WireframeRender.Instance;
        if(wireframeRender == null) {
            return;
        }
        wireframeRender.linesUI.Add( landerPos );
        wireframeRender.linesUI.Add( sliceLine );
        
        foreach( LineData line in knownObjectToMarker.Values ) {
            wireframeRender.linesUI.Add( line );
        }
    }
    
    /// <summary>
    /// Désactive l'affichage des informations sur la carte
    /// </summary>
    public void DisableInfos() {
        WireframeRender wireframeRender = WireframeRender.Instance;
        if(wireframeRender == null) {
            return;
        }
        wireframeRender.linesUI.Remove( landerPos );
        wireframeRender.linesUI.Remove( sliceLine );
        
        foreach( LineData line in knownObjectToMarker.Values ) {
            wireframeRender.linesUI.Remove( line );
        }
    }
    
}
