using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Instrument : UIwireframeElement, IinventoryItem
{

    public abstract float Mass
    {
        get;
    }
    
    public abstract float Volume {
        get;
    }
    
    public abstract string Name {
        get;
    }
}
