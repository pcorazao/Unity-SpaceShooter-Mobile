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
	private Quaternion calibrationQuaternion;//iphone

	#region iphone
	void Start()
	{
		CalibrateAccellerometer ();
	}
	#endregion

	void Update(){

		if (Input.GetButton ("Fire1") && Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			Instantiate (shot, shotSpawn.position, shotSpawn.rotation); //create a new game object
			var audioSource = GetComponent<AudioSource>();
			audioSource.Play();
		}
	}

	#region iphone
	void CalibrateAccellerometer()
	{
		Vector3 accelerateSnapshot = Input.acceleration;
		Quaternion rotateQuaternion = Quaternion.FromToRotation (new Vector3 (0.0f, 0.0f, -1.0f), accelerateSnapshot);
		calibrationQuaternion = Quaternion.Inverse (rotateQuaternion);
	}
	
	Vector3 FixAcceleration(Vector3 acceleration)
	{
		Vector3 fixedAcceleration = calibrationQuaternion * acceleration;
		return fixedAcceleration;
	}
	#endregion

	void FixedUpdate()
	{
		//called just before each physics setup
		var rigidbody = GetComponent<Rigidbody> ();

		//for keyboard and mouse.
//		float moveHorizontal = Input.GetAxis ("Horizontal") * Speed;
//		float moveVertical = Input.GetAxis("Vertical") * Speed;
//		var movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
//		rigidbody.velocity = movement;

		#region iPhone

		Vector3 accelerationRaw = Input.acceleration;
		Vector3 acceleration = FixAcceleration(accelerationRaw);
		var movement = new Vector3 (acceleration.x, 0.0f, acceleration.y);
		rigidbody.velocity = movement * Speed;

		#endregion

		rigidbody.position = new Vector3 (Mathf.Clamp (rigidbody.position.x, boundary.xMin, boundary.xMax), 
		                                  0.0f, 
		                                  Mathf.Clamp (rigidbody.position.z, boundary.zMin, boundary.zMax));

		rigidbody.rotation = Quaternion.Euler (0.0f, 0.0f, rigidbody.velocity.x * -tilt);

	}



}
