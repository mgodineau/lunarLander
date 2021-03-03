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
        EnableBorder();
    }
    
    
    
    private void UpdateScreenRect() {
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);
        screenRect = new Rect(
            worldCorners[0].x / Screen.width,
            worldCorners[0].y / Screen.height,
            (worldCorners[3].x - worldCorners[0].x) / Screen.width,
            (worldCorners[1].y - worldCorners[0].y) / Screen.height);
    }
    
    
    private void UpdateBorder() {
        Vector3[] borderLineArr = new Vector3[4];
        rectTransform.GetWorldCorners(borderLineArr);
        
        for (int i = 0; i < borderLineArr.Length; i++)
        {
            borderLineArr[i].x /= Screen.width;
            borderLineArr[i].y /= Screen.height;
            borderLineArr[i].z = 0;
        }

        borderLine.points = new List<Vector3>(borderLineArr);
    }
    
    private void EnableBorder()
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
