using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class Instrument : MonoBehaviour
{


    private float _weight = 0;
    public float Weight
    {
        get { return _weight; }
    }

    private LineData borderLine = new LineData();
    private RectTransform rectTransform;

    private Rect screenRect;


    protected void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    protected void Start() {
        UpdateScreenRect();
        UpdateBorder();
        // EnableBorder();
    }
    
    
    
    private void UpdateScreenRect() {
        // Rect mainRect = InstrumentsManager.Instance.CanvasRect.rect;
        Vector3[] canvasCorners = new Vector3[4];
        InstrumentsManager.Instance.CanvasRect.GetWorldCorners(canvasCorners);
        
        Vector3[] localCorners = new Vector3[4];
        rectTransform.GetWorldCorners(localCorners);
        // screenRect = new Rect(
        //     worldCorners[0].x / Screen.width,
        //     worldCorners[0].y / Screen.height,
        //     (worldCorners[3].x - worldCorners[0].x) / Screen.width,
        //     (worldCorners[1].y - worldCorners[0].y) / Screen.height);
        
        float width = canvasCorners[3].x - canvasCorners[0].x;
        float height = canvasCorners[1].y - canvasCorners[0].y;
        
        screenRect = new Rect(
            (localCorners[0].x - canvasCorners[0].x) / width,
            (localCorners[0].y - canvasCorners[0].y) / height,
            (localCorners[3].x - localCorners[0].x) / width,
            (localCorners[1].y - localCorners[0].y) / height);
    }
    
    
    private void UpdateBorder() {
        borderLine.points.Clear();

        borderLine.points.Add( localToGlobal(Vector3.zero) );
        borderLine.points.Add( localToGlobal(Vector3.up) );
        borderLine.points.Add( localToGlobal(Vector2.one) );
        borderLine.points.Add( localToGlobal(Vector3.right) );
        borderLine.points.Add( localToGlobal(Vector3.zero) );
    }
    
    protected void EnableBorder()
    {
        WireframeRender.Instance.linesUI.Add( borderLine );
    }

    private void DisableBorder()
    {
        WireframeRender.Instance.linesUI.Remove( borderLine );
    }


    protected Vector3 localToGlobal(Vector3 localPos)
    {
        localPos.x = localPos.x * screenRect.width + screenRect.x;
        localPos.y = localPos.y * screenRect.height + screenRect.y;

        return localPos;
    }
}
