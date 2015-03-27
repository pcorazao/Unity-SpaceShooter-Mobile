using UnityEngine;
using System.Collections;
using Ligabo;

public class FollowJoystick : MonoBehaviour {

	public string moveJoystickName = "LiGaBOMoveJoystick";
	public float moveForce=10f;
	public float minMoveMagnitude = 0.3f;

	GameObject terrain;
	Joystick moveJoystick;

	// Use this for initialization
	void Start () {
		moveJoystick =  GameObject.Find(moveJoystickName).gameObject.GetComponent<Joystick>();
		if (moveJoystick==null)
			Debug.LogError("move joystick is null");
	}


	// Update is called once per frame
	void FixedUpdate () {
	
		var moveVect =  new Vector2(moveJoystick.position.x, moveJoystick.position.y);
		if (moveJoystick==null || moveVect.magnitude < minMoveMagnitude )
			return;


		//rotate it
		//camera rotation fix
		//Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
		//Debug.Log("camera direction:"+forward);
		//forward.y = 0f; 
		//float ang = Camera.main.transform.rotation.eulerAngles.y; // Vector3.Angle(Vector3.forward,forward);
		float ang = LigaboUtils.GetCameraFix();

		//Debug.Log(ang);
		//Vector3 Quaternion.Euler(0, 30, 0) * dirVector;
		//Vector3 jsPostion =  Quaternion.Euler(0, ang, 0) * new Vector3 (RightStick.x,0,RightStick.y); 

		float jsAngle = Mathf.Atan2(moveJoystick.position.x, moveJoystick.position.y)* Mathf.Rad2Deg;
		if (jsAngle<0) 
			jsAngle+=360;

		//Debug.Log("jAngle:"+jsAngle+", camAngle:"+ang);
		Quaternion rot = Quaternion.Euler(0,jsAngle+ang,0);
		RotateTo(rot);


		//move it
		Vector3 force = Quaternion.AngleAxis(jsAngle+ang, Vector3.up) * Vector3.forward;
		MoveByForce(force.normalized);

	}

	public void MoveByForce(Vector3 normalizedVector, bool relative = false){
		if (!relative)
			GetComponent<Rigidbody>().AddForce (normalizedVector * moveForce*Time.fixedDeltaTime , ForceMode.VelocityChange);
		else
			GetComponent<Rigidbody>().AddRelativeForce(normalizedVector * moveForce*Time.fixedDeltaTime , ForceMode.VelocityChange);
	}
	
	public void RotateTo(Quaternion newRot){

		GetComponent<Rigidbody>().MoveRotation(newRot);
		
	}
}
