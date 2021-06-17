using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class UIwireframeElement : MonoBehaviour
{

    private LineData borderLine = new LineData();
    private RectTransform rectTransform;

    private Rect screenRect;
    
    [SerializeField]
    private bool _borderEnabled = true;
    public bool BorderEnabled {
        get{ return _borderEnabled; }
        set{
            _borderEnabled = value;
            WireframeRender wireframeRender = WireframeRender.Instance;
            if( wireframeRender == null ) {
                return;
            }
            if( _borderEnabled ) {
                wireframeRender.linesUI.Add(borderLine);
            } else {
                wireframeRender.linesUI.Remove(borderLine);
            }
        }
    }


    protected void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    protected void Start()
    {
        UpdateScreenRect();
        UpdateBorder();
    }
    
    
    protected void OnEnable()
    {
        BorderEnabled = _borderEnabled;
    }
    
    protected void OnDisable() {
        WireframeRender.Instance.linesUI.Remove(borderLine);
    }
    

    /// <summary>
    /// MAJ les lignes de la bordure de l'instrument
    /// </summary>
    private void UpdateBorder()
    {
        borderLine.points.Clear();

        borderLine.points.Add(LocalToGlobal(Vector3.zero));
        borderLine.points.Add(LocalToGlobal(Vector3.up));
        borderLine.points.Add(LocalToGlobal(Vector2.one));
        borderLine.points.Add(LocalToGlobal(Vector3.right));
        borderLine.points.Add(LocalToGlobal(Vector3.zero));
    }



    /// <summary>
    /// MAJ le rectangle de l'écran, en fonction de la nouvelle taille réelle
    /// </summary>
    private void UpdateScreenRect()
    {
        Vector3[] canvasCorners = new Vector3[4];
        UImanager.Instance.instrumentsManager.CanvasRect.GetWorldCorners(canvasCorners);

        Vector3[] localCorners = new Vector3[4];
        rectTransform.GetWorldCorners(localCorners);

        float width = canvasCorners[3].x - canvasCorners[0].x;
        float height = canvasCorners[1].y - canvasCorners[0].y;

        screenRect = new Rect(
            (localCorners[0].x - canvasCorners[0].x) / width,
            (localCorners[0].y - canvasCorners[0].y) / height,
            (localCorners[3].x - localCorners[0].x) / width,
            (localCorners[1].y - localCorners[0].y) / height);
    }


    /// <summary>
    /// Conversion d'une position dans le référentiel local vers le référentiel global
    /// </summary>
    /// <param name="localPos"> Une position locale, comprise dans le rectangle (0,0), (1,1) </param>
    /// <returns> la conversion de localPos dans le référentiel de l'écran. </returns>
    protected Vector3 LocalToGlobal(Vector3 localPos)
    {
        localPos.x = localPos.x * screenRect.width + screenRect.x;
        localPos.y = localPos.y * screenRect.height + screenRect.y;

        return localPos;
    }
    
    
    protected float RectRatio() {
        return screenRect.width / screenRect.height;
    }


}
