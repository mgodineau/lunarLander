using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MenuEntryUI : WireframeLabel
{
    
    private static MenuEntryUI selectedEntry = null;
    
    
    [SerializeField]
    private RectTransform _rectTr;
    public RectTransform RectTr {
        get {
            return _rectTr;
        }
    }
    
    [SerializeField]
    private GameObject selectionArrow;
    
    
    private MenuEntry linkedEntry = null;
    
    
    
    new private void OnEnable() {
        Debug.Log("OnEnable");
        
        base.OnEnable();
        
        Deselect();
    }
    
    
    public void Select() {
        if( selectedEntry != null ) {
            selectedEntry.Deselect();
        }
        selectedEntry = this;
        
        selectionArrow.SetActive(true);
    }
    
    public void Deselect()
    {
        if( selectedEntry == this ) {
            selectedEntry = null;
        }
        
        selectionArrow.SetActive(false);
    }
    
    
    public void SetEntry( MenuEntry entry ) {
        Debug.Log("SetEntry to \"" + entry.Name + "\"");
        linkedEntry = entry;
        
        // textUI.text = linkedEntry.Name;
        Text = linkedEntry.Name;
    }

    

    public void OnClick() {
        if ( linkedEntry != null ) {
            linkedEntry.OnClick();
        }
    }
    
    
}
