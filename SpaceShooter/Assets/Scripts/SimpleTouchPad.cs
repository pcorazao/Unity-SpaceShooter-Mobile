using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SimpleTouchPad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

	public float smoothing;

	private Vector2 origin;
	private Vector2 direction;
	private Vector2 smoothDirection;
	private bool touched;
	private int pointerID;

	void Awake()
	{
		direction = Vector2.zero;
		touched = false;
	}

	public void OnPointerDown(PointerEventData data)
	{
		//Set our start point
		if (!touched) {
			touched = true;
			pointerID = data.pointerId;
			origin = data.position;
			Debug.Log(pointerID);
		}
	}

	public void OnDrag(PointerEventData data)
	{
		//compare the difference between our start point and current pointer pos
		if (data.pointerId == pointerID) {
			Vector2 currentPos = data.position;
			Vector2 directionRaw = currentPos - origin;
			direction = directionRaw.normalized;
			Debug.Log (pointerID);
		}
	}

	public void OnPointerUp(PointerEventData data)
	{
		//Reset Everything
		if (data.pointerId == pointerID) {
			direction = Vector2.zero;
			touched = false;
		}
	}

	public Vector2 GetDirection()
	{
		smoothDirection = Vector2.MoveTowards (smoothDirection, direction, smoothing);
		return smoothDirection;
	}

}
