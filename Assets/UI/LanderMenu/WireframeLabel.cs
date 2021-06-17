using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireframeLabel : UIwireframeElement
{

    [SerializeField]
    private string _text = "";
    public string Text
    {
        get { return _text; }
        set
        {
            _text = value;
            UpdateTextGeometry();
        }
    }

    [SerializeField]
    private float _verticalSize = 0.5f;
    public float VerticalSize
    {
        get { return _verticalSize; }
        set { _verticalSize = Mathf.Max(value, 0); }
    }


    [SerializeField]
    private TextAlignment _alignment = TextAlignment.Left;
    public TextAlignment Alignment
    {
        get { return _alignment; }
        set
        {
            _alignment = value;
            UpdateTextGeometry();
        }
    }
    
    
    List<LineData> textGeometry = new List<LineData>();


    new private void Start()
    {
        base.Start();

        UpdateTextGeometry();
    }


    new private void OnEnable()
    {
        base.OnEnable();

        EnableText();
    }

    new private void OnDisable()
    {
        base.OnDisable();

        DisableText();
    }



    private void UpdateTextGeometry()
    {

        DisableText();
        textGeometry.Clear();

        float ratioInv = 1.0f / (RectRatio() * Screen.width / Screen.height);
        float horizontalSize = VerticalSize * ratioInv;

        float offsetY = (1.0f - VerticalSize) * 0.5f;
        float offsetX = offsetY * ratioInv;
        
        float startX = 0;
        if( Alignment != TextAlignment.Left ) {
            startX = 1 - _text.Length * (horizontalSize + offsetX) - offsetX;
            if( Alignment == TextAlignment.Center ) {
                startX *= 0.5f;
            }
        }
        
        for (int i = 0; i < _text.Length; i++)
        {

            float[] rawPath = WireframeFont.getCharRawPath(_text[i]);
            List<Vector3> coords = new List<Vector3>();

            for (int j = 0; j < rawPath.Length; j += 2)
            {
                float x = (horizontalSize) * (rawPath[j] + i) + ((1 + i) * offsetX) + startX;
                float y = rawPath[j + 1] * VerticalSize + offsetY;
                coords.Add(LocalToGlobal(new Vector3(x, y, 0)));
            }

            textGeometry.Add(new LineData(coords));

        }

        EnableText();

    }


    private void EnableText()
    {
        foreach (LineData path in textGeometry)
        {
            WireframeRender.Instance.linesUI.Add(path);
        }
    }

    private void DisableText()
    {
        foreach (LineData path in textGeometry)
        {
            WireframeRender.Instance.linesUI.Remove(path);
        }
    }

}
