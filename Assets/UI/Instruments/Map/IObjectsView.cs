using System.Collections.Generic;
using UnityEngine;


public interface IObjectsView {
    
    public void SetObjectsCollection(IEnumerable<LocalizedObject> objects);
    
    public void UpdateObject( LocalizedObject obj );
    
    public void AddObject( LocalizedObject obj );
    
    public void RemoveObject( LocalizedObject obj );
    
    
    
}