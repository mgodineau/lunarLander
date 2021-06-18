using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : InputConsumer
{

    private Stack<SubMenu> menuPath = new Stack<SubMenu>();

    [SerializeField]
    private WireframeLabel menuTitle;
    [SerializeField]
    private RectTransform menuRect;

    [SerializeField]
    private MenuEntryUI menuEntryPref;

    private List<MenuEntryUI> buttonsList = new List<MenuEntryUI>();
    
    
    private int _selectedEntryId = 0;
    private int SelectedEntryId {
        get { return _selectedEntryId; }
        set
        {
            while( value < 0 ) {
                value += buttonsList.Count;
            }
            if( buttonsList.Count != 0 ) {
                _selectedEntryId = value % buttonsList.Count;
                buttonsList[_selectedEntryId].Select();
            }
        }
    }
    
    
    
    
    private void Start()
    {

        // List<MenuEntry> entryList = new List<MenuEntry>();
        // entryList.Add(new MenuEntryEmpty("truc"));
        // entryList.Add(new MenuEntryEmpty("machin"));

        // SubMenu testMenu = new SubMenu("Test Menu", entryList);
        // SetMenu(testMenu);

        UpdateMenuUI();

    }
    
    
    private void Update() 
    {
        if( ProcessInput() ) {
            
            if( Input.GetKeyDown(KeyCode.Q) ) {
                PopMenu();
            }
            
            if( Input.GetKeyDown(KeyCode.D) && buttonsList.Count > 0) {
                buttonsList[SelectedEntryId].OnClick();
            }
            
            if( Input.GetKeyDown(KeyCode.Z) ) {
                SelectedEntryId--;
            }
            
            if( Input.GetKeyDown(KeyCode.S) ) {
                SelectedEntryId++;
            }
            
        }
    }
    
    
    
    public void SetMenu(SubMenu menu)
    {
        menuPath.Clear();
        PushMenu(menu);
    }

    public void PushMenu(SubMenu menu)
    {
        menuPath.Push(menu);
        UpdateMenuUI();
    }

    public void PopMenu()
    {
        menuPath.Pop();
        UpdateMenuUI();
    }

    public void ClearMenu()
    {
        menuPath.Clear();
        UpdateMenuUI();
    }


    public void UpdateMenuUI()
    {

        //suppression du menu précédent
        menuTitle.transform.parent.gameObject.SetActive(false);
        foreach (MenuEntryUI entryUI in buttonsList)
        {
            GameObject.Destroy(entryUI.gameObject);
        }
        buttonsList.Clear();


        //retour si aucun menu à afficher
        if (menuPath.Count == 0)
        {
            DisableInputProcessing();
            return;
        }
        
        EnableInputProcessing();
        SubMenu currentMenu = menuPath.Peek();

        // MAJ du titre du nouveau menu
        menuTitle.transform.parent.gameObject.SetActive(true);
        menuTitle.Text = currentMenu.Name;

        float anchorTop = menuRect.anchorMin.y;
        float entryAnchorHeight = menuEntryPref.RectTr.anchorMax.y - menuEntryPref.RectTr.anchorMin.y;

        //affichage de chaque item du menu
        foreach (MenuEntry entry in currentMenu.Content)
        {
            MenuEntryUI currentButton = GameObject.Instantiate(menuEntryPref, transform);
            currentButton.SetEntry(entry);

            RectTransform rect = currentButton.RectTr;
            rect.anchorMin = new Vector2(rect.anchorMin.x, anchorTop - (rect.anchorMax.y - rect.anchorMin.y));
            rect.anchorMax = new Vector2(rect.anchorMax.x, anchorTop);
            anchorTop -= entryAnchorHeight;

            buttonsList.Add(currentButton);
        }
        

        SelectedEntryId = 0;
    }

}


public abstract class MenuEntry
{


    private string _name;
    public string Name
    {
        get { return _name; }
    }


    public abstract void OnClick();

    protected MenuEntry(string name)
    {
        _name = name;
    }

}

public class MenuEntryEmpty : MenuEntry
{

    public override void OnClick()
    {
        Debug.Log("Button \"" + Name + "\" pressed");
    }

    public MenuEntryEmpty(string name) : base(name) { }

}

public class SubMenu : MenuEntry
{
    private List<MenuEntry> _content;
    public List<MenuEntry> Content
    {
        get { return _content; }
    }
    
    public override void OnClick()
    {
        UImanager.Instance.menuManager.PushMenu(this);
    }


    public SubMenu(string name, List<MenuEntry> content) : base(name)
    {
        this._content = content;
    }
}