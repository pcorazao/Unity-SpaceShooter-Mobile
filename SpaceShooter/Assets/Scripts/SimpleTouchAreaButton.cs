using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimpleTouchAreaButton: MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	private bool touched;
	private int pointerID;
	private bool canFire;
	
	void Awake()
	{
		canFire = false;
		touched = false;
	}
	
	public void OnPointerDown(PointerEventData data)
	{
		Debug.Log("fire");
		//Set our start point
		if (!touched) {
			touched = true;
			pointerID = data.pointerId;
			canFire = true;
			Debug.Log(canFire);
		}
	}
	
	public void OnPointerUp(PointerEventData data)
	{
		//Reset Everything
		if (data.pointerId == pointerID) {
			canFire = false;
			touched = false;
			Debug.Log(canFire);
		}
	}

	public bool CanFire()
	{
		return canFire;
	}
}
