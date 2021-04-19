using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineData
{

    private Color _lineColor;
    public Color LineColor
    {
        get { return _lineColor; }
    }

    public List<Vector3> points;



    public LineData() : this(new List<Vector3>()) { }

    public LineData(List<Vector3> points) : this(points, Color.white) { }
    
    public LineData(Color lineColor) : this(new List<Vector3>(), lineColor) {}
    
    public LineData(List<Vector3> points, Color lineColor)
    {
        this.points = points;
        this._lineColor = lineColor;
    }

}
