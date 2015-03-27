using UnityEngine;
using System.Collections;


[System.Serializable]
public class Boundary
{
	public float xMin,xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour {

	public float Speed;
	public float tilt;
	public Boundary boundary;

	public GameObject shot;
	public Transform shotSpawn;

	public float fireRate = 0.5f;
	private float nextFire = 0.0f;

	void Update(){

		if (Input.GetButton ("Fire1") && Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			Instantiate (shot, shotSpawn.position, shotSpawn.rotation); //create a new game object
		}
	}

	void FixedUpdate()
	{
		//called just before each physics setup

		float moveHorizontal = Input.GetAxis ("Horizontal") * Speed;
		float moveVertical = Input.GetAxis("Vertical") * Speed;

		var rigidbody = GetComponent<Rigidbody> ();
		var movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		rigidbody.velocity = movement;

		rigidbody.position = new Vector3 (Mathf.Clamp (rigidbody.position.x, boundary.xMin, boundary.xMax), 
		                                  0.0f, 
		                                  Mathf.Clamp (rigidbody.position.z, boundary.zMin, boundary.zMax));

		rigidbody.rotation = Quaternion.Euler (0.0f, 0.0f, rigidbody.velocity.x * -tilt);

	}

}
