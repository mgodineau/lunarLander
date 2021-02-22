using UnityEngine;

public abstract class LandingZone
{

    public Vector3 Position
    {
        get { return _position; }
        set { _position = value.normalized; }
    }
    private Vector3 _position;


    protected LandingZone() : this(Vector3.right)
    {

    }

    protected LandingZone(Vector3 position)
    {
        Position = position;
    }

}