using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
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
	
	private AudioSource audioSource;
	
	[SerializeField]
	private ClipVariantsCollection buttonDown;
	[SerializeField]
	private ClipVariantsCollection buttonUp;
	
	
	private void Awake() {
		audioSource = GetComponent<AudioSource>();
	}
	
	private void Start()
	{
		PopMenu();
	}
	
	
	private void Update() 
	{
		if( CanProcessInput() ) {
			
			if( Input.GetKeyUp(KeyCode.Q) ) {
				// PlayRandomSound( buttonDown_clips );
				buttonDown.PlayRandomClip(audioSource);
				PopMenu();
			}
			
			if( Input.GetKeyUp(KeyCode.D) && buttonsList.Count > 0) {
				buttonUp.PlayRandomClip(audioSource);
				buttonsList[SelectedEntryId].OnClick();
			}
			
			if( Input.GetKeyUp(KeyCode.Z) ) {
				buttonUp.PlayRandomClip(audioSource);
				SelectedEntryId--;
			}
			
			if( Input.GetKeyUp(KeyCode.S) ) {
				buttonDown.PlayRandomClip(audioSource);
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
		if( menuPath.Count == 0 ) {
			buttonDown.PlayRandomClip(audioSource);
		}
		menuTitle.gameObject.SetActive(true);
		EnableInputProcessing();
		
		menuPath.Push(menu);
		UpdateMenuUI();
	}

	public void PopMenu()
	{
		if ( menuPath.Count > 0 ) {
			menuPath.Pop();
		}
		UpdateMenuUI();
		
		if ( menuPath.Count == 0 ) {
			DisableInputProcessing();
			menuTitle.gameObject.SetActive(false);
		}
	}

	public void ClearMenu()
	{
		menuPath.Clear();
		UpdateMenuUI();
	}
	
	public SubMenu GetCurrentMenu() {
		return menuPath.Count == 0 ? null : menuPath.Peek();
	}
	

	public void UpdateMenuUI()
	{

		//suppression du menu précédent
		foreach (MenuEntryUI entryUI in buttonsList)
		{
			GameObject.Destroy(entryUI.gameObject);
		}
		buttonsList.Clear();


		//retour si aucun menu à afficher
		if (menuPath.Count == 0)
		{
			return;
		}
		
		SubMenu currentMenu = menuPath.Peek();

		// MAJ du titre du nouveau menu
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
	private IEnumerable<MenuEntry> _content;
	public IEnumerable<MenuEntry> Content
	{
		get { return _content; }
	}
	
	public override void OnClick()
	{
		UImanager.Instance.menuManager.PushMenu(this);
	}


	public SubMenu(string name, IEnumerable<MenuEntry> content) : base(name)
	{
		this._content = content;
	}
}