using UnityEngine;
using System.Collections;

public class DestroyByTime : MonoBehaviour {

	public float liftTime;

	void Start()
	{
		Destroy (gameObject, liftTime);
	}
}
