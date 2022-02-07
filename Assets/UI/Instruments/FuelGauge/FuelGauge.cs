using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelGauge : InstrumentBehaviour
{
    
    [SerializeField] private float marginX = 0.1f;
    
    //Description des graduations. nombre de subdivisions pour chaque taille de ligne
    [SerializeField] private int[] graduationDescription = {2, 10};
    [SerializeField] private float gradMaxWidth = 0.5f;
    [SerializeField] private float cursorWidth = 0.5f;
    
    private List<LineData> graduations = new List<LineData>();
    private LineData cursor = new LineData();
    
    
    public override float Mass {
        get{ return 1; }
    }

    public override float Volume {
        get{ return 1; }
    }
    
    public override string Name {
        get{ return "Fuel gauge"; }
    }
    
    
    
    new private void OnEnable() {
        base.OnEnable();
        
        EnableDisplay();
    }
    
    
    new private void OnDisable() {
        base.OnDisable();
        
        DisableDisplay();
    }
    
    
    
    new private void Update() {
        base.Update();
        
        UpdateCursor();
    }
    
    
    protected override void BuildUI() {
        base.BuildUI();
        
        CreateGraduation();
        CreateCursor();
        EnableDisplay();
    }
    
    
    
    
    private void CreateGraduation() {
        
        // foreach( LineData line in graduations ) {
        //     WireframeRender.Instance.linesUI.Remove(line);
        // }
        bool enabled = this.enabled;
        this.enabled = false;
        
        graduations = new List<LineData>();
        
        
        float xRight = 1.0f - marginX;
        float marginY = marginX * 0.5f;
        
        List<Vector3> verticalLine = new List<Vector3>();
        verticalLine.Add( LocalToGlobal( new Vector3(xRight, 1.0f - marginY, 0)) );
        verticalLine.Add( LocalToGlobal( new Vector3(xRight, marginY, 0)) );
        
        graduations.Add( new LineData( verticalLine ) );
        
        
        float gradHeight = 1.0f - marginY*2;
        
        for( int i=0; i<graduationDescription.Length; i++ ) {
            
            float currentWidth = gradMaxWidth * (graduationDescription.Length - i) / graduationDescription.Length;
            for( int j=0; j<=graduationDescription[i]; j++ ) {
                
                float y = marginY + gradHeight * j / graduationDescription[i];
                
                List<Vector3> currentLine = new List<Vector3>();
                currentLine.Add( LocalToGlobal( new Vector3(xRight, y, 0) ) );
                currentLine.Add( LocalToGlobal( new Vector3(xRight - currentWidth, y, 0) ) );
                graduations.Add( new LineData(currentLine) );
            }
            
        }
        
        this.enabled = enabled;
        // foreach( LineData line in graduations ) {
        //     WireframeRender.Instance.linesUI.Add(line);
        // }
        
    }
    
    
    
    private void CreateCursor() {
        
        cursor.points.Clear();
        float marginY = marginX *0.5f;
        
        // WireframeRender.Instance.linesUI.Remove(cursor);
        
        List<Vector3> cursorPath = new List<Vector3>();
        cursorPath.Add( LocalToGlobal( new Vector3(0, 0, 0) ) );
        cursorPath.Add( LocalToGlobal( new Vector3(cursorWidth-marginY, 0, 0) ) );
        cursorPath.Add( LocalToGlobal( new Vector3(cursorWidth, marginY*0.5f, 0) ) );
        cursorPath.Add( LocalToGlobal( new Vector3(cursorWidth-marginY, marginY, 0) ) );
        cursorPath.Add( LocalToGlobal( new Vector3(0, marginY, 0) ) );
        
        cursor.points =cursorPath;
        
        // WireframeRender.Instance.linesUI.Add(cursor);
    }
    
    
    private void UpdateCursor() {
        
        float marginY = marginX * 0.5f;
        
        Lander lander = UImanager.Instance.lander;
        float fuelRatio = lander.GetFuelQuantity() / lander.GetFuelCapacity();
        
        float yCenter = marginY + fuelRatio * (1.0f - marginY*2);
        float yUpper = yCenter + marginY*0.5f;
        float yLower = yCenter - marginY*0.5f;
        
        List<Vector3> cursorPath = new List<Vector3>();
        cursorPath.Add( LocalToGlobal( new Vector3( 0 , yLower, 0) ) );
        cursorPath.Add( LocalToGlobal( new Vector3(cursorWidth-marginY, yLower, 0) ) );
        cursorPath.Add( LocalToGlobal( new Vector3(cursorWidth, yCenter, 0) ) );
        cursorPath.Add( LocalToGlobal( new Vector3(cursorWidth-marginY, yUpper, 0) ) );
        cursorPath.Add( LocalToGlobal( new Vector3(0, yUpper, 0) ) );
        cursor.points = cursorPath;
        
    }
    
    
    private void EnableDisplay() {
        WireframeRender wireframeRender = WireframeRender.Instance;
        if( wireframeRender == null ) {
            return;
        }
        
        wireframeRender.linesUI.Add(cursor);
        foreach( LineData line in graduations ) {
            wireframeRender.linesUI.Add(line);
        }
        
    }
    
    
    
    private void DisableDisplay() {
        WireframeRender wireframeRender = WireframeRender.Instance;
        if( wireframeRender == null ) {
            return;
        }
        
        wireframeRender.linesUI.Remove(cursor);
        foreach( LineData line in graduations ) {
            wireframeRender.linesUI.Remove(line);
        }
    }
    
    
    
    
    
}
