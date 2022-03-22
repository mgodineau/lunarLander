using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectBehaviour : MonoBehaviour
{
	
	public abstract LocalizedObject LocObject {
		get;
	}
	
	
	[SerializeField]
	private float _groundOffset = 0;
	public float GroundOffset {
		get {return _groundOffset;}
		set {
			_groundOffset = Mathf.Max(0, value);
		}
	}
	
	public Vector3 sliceNormal = Vector3.up;
	
	
	bool isStatic = false;
	private delegate void UpdateAction();
	UpdateAction updateAction = () => {};
	
	
	protected void Awake() {
		isStatic = GetComponent<Rigidbody2D>() == null;
		if ( !isStatic ) {
			updateAction = () => { TerrainManager.Instance.UpdateObjectLocation( this );};
		}
	}
	
	protected void Update() {
		updateAction();
		UpdateDisplayPositionX();
	}
	
	
	public bool IsStatic()
	{
		return isStatic;
	}
	
	public void SetPosition( Vector3 position ) {
		
		position.y += _groundOffset;
		transform.position = position;
	}
	
	
	public void RemoveFromWorld() {
		TerrainManager.Instance.Planet.RemoveObject(LocObject);
	}
	
	private void UpdateDisplayPositionX() {
		float terrainWidth = TerrainManager.Instance.TerrainWidth;
		float cameraX = Camera.main.transform.position.x;
		
		float posX_0 = transform.position.x;
		float posX_1 = posX_0 + ((posX_0<0) ? terrainWidth : -terrainWidth);
		
		Vector3 pos = transform.position;
		pos.x = Mathf.Abs(cameraX - posX_0) < Mathf.Abs(cameraX - posX_1) ? posX_0 : posX_1;
		transform.position = pos;
	}
	
}
