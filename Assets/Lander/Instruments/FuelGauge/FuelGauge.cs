using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelGauge : Instrument
{
    
    [SerializeField] private float marginX = 0.1f;
    
    //Description des graduations. nombre de subdivisions pour chaque taille de ligne
    [SerializeField] private int[] graduationDescription = {2, 10};
    [SerializeField] private float gradMaxWidth = 0.5f;
    [SerializeField] private float cursorWidth = 0.5f;
    
    private List<LineData> graduations = new List<LineData>();
    private LineData cursor = new LineData();
    
    new private void Start() {
        base.Start();
        
        EnableBorder();
        
        CreateGraduation();
        CreateCursor();
    }
    
    private void Update() {
        UpdateCursor();
    }
    
    
    
    
    private void CreateGraduation() {
        
        foreach( LineData line in graduations ) {
            WireframeRender.Instance.linesUI.Remove(line);
        }
        graduations = new List<LineData>();
        
        
        float xRight = 1.0f - marginX;
        float marginY = marginX * 0.5f;
        
        List<Vector3> verticalLine = new List<Vector3>();
        verticalLine.Add( localToGlobal( new Vector3(xRight, 1.0f - marginY, 0)) );
        verticalLine.Add( localToGlobal( new Vector3(xRight, marginY, 0)) );
        
        graduations.Add( new LineData( verticalLine ) );
        
        
        float gradHeight = 1.0f - marginY*2;
        
        for( int i=0; i<graduationDescription.Length; i++ ) {
            
            float currentWidth = gradMaxWidth * (graduationDescription.Length - i) / graduationDescription.Length;
            for( int j=0; j<=graduationDescription[i]; j++ ) {
                
                float y = marginY + gradHeight * j / graduationDescription[i];
                
                List<Vector3> currentLine = new List<Vector3>();
                currentLine.Add( localToGlobal( new Vector3(xRight, y, 0) ) );
                currentLine.Add( localToGlobal( new Vector3(xRight - currentWidth, y, 0) ) );
                graduations.Add( new LineData(currentLine) );
            }
            
        }
        
        
        foreach( LineData line in graduations ) {
            WireframeRender.Instance.linesUI.Add(line);
        }
        
    }
    
    
    
    private void CreateCursor() {
        
        float marginY = marginX *0.5f;
        
        WireframeRender.Instance.linesUI.Remove(cursor);
        
        List<Vector3> cursorPath = new List<Vector3>();
        cursorPath.Add( localToGlobal( new Vector3(0, 0, 0) ) );
        cursorPath.Add( localToGlobal( new Vector3(cursorWidth-marginY, 0, 0) ) );
        cursorPath.Add( localToGlobal( new Vector3(cursorWidth, marginY*0.5f, 0) ) );
        cursorPath.Add( localToGlobal( new Vector3(cursorWidth-marginY, marginY, 0) ) );
        cursorPath.Add( localToGlobal( new Vector3(0, marginY, 0) ) );
        
        cursor = new LineData(cursorPath);
        
        WireframeRender.Instance.linesUI.Add(cursor);
    }
    
    
    private void UpdateCursor() {
        
        float marginY = marginX * 0.5f;
        
        Lander lander = InstrumentsManager.Instance.CurrentLander;
        float fuelRatio = lander.GetFuelQuantity() / lander.GetFuelCapacity();
        
        float yCenter = marginY + fuelRatio * (1.0f - marginY*2);
        float yUpper = yCenter + marginY*0.5f;
        float yLower = yCenter - marginY*0.5f;
        
        List<Vector3> cursorPath = new List<Vector3>();
        cursorPath.Add( localToGlobal( new Vector3( 0 , yLower, 0) ) );
        cursorPath.Add( localToGlobal( new Vector3(cursorWidth-marginY, yLower, 0) ) );
        cursorPath.Add( localToGlobal( new Vector3(cursorWidth, yCenter, 0) ) );
        cursorPath.Add( localToGlobal( new Vector3(cursorWidth-marginY, yUpper, 0) ) );
        cursorPath.Add( localToGlobal( new Vector3(0, yUpper, 0) ) );
        cursor.points = cursorPath;
        
    }
    
    
}
