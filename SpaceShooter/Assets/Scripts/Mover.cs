using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	public float speed;

	public void Start()
	{
		var rigidBody = GetComponent<Rigidbody>();
		rigidBody.velocity = transform.forward * speed;
	}
}
