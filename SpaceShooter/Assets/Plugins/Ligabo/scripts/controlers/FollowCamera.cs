/*----------------------------------------------------------
Copyright (c) 2014 Analytica UK Ltd. All Rights Reserved.
Confidential and Proprietary
----------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Ligabo;


namespace Ligabo {

	/// <summary>
	/// Class for controlling characted by pointing center of the camera to place or direction character should follow.
	/// Class can be used to move gameobject directly or use custom logic to move the object by reading public X and Y 
	/// properties which are mapped to moveHorizontal and moveVertical similar as standard unity input is using.
	/// Values are provided smoothly betweeen 0 and 1.0
	/// </summary>
	/// 
	public class FollowCamera : MonoBehaviour {

		/// <summary>
		/// texture to show in center of the screen as pointer for user to indicate where character should move.
		/// </summary>
		public Texture2D targetGUITexture;
		/// <summary>
		/// whether the script should move character or not. If set to true, ridgid body will be used to move character if there is one.
		/// Otherwise transform position will be smooothly modified to reach new position.
		/// </summary>
		public bool moveObjectAutomatically;
		/// <summary>
		/// Define boundary in which character can be moved. Define it is scene units.
		/// </summary>
		public MoveBoundary boundary;
		/// <summary>
		/// If script is set not to move character automatically, use this to read out position where it should be actually moved. 
		/// Usefull if you implement your own logic for character movement.
		/// </summary>
		public Vector3 position;
		/// <summary>
		/// If script is set not to move character automatically, use this to read out horizontal direction where it should be actually moved. 
		/// It is value as you would get from standard unity Input, value is between 0 and 1.0
		/// Usefull if you implement your own logic for character movement.
		/// </summary>
		public float moveHorizontal=0f;
		/// <summary>
		/// If script is set not to move character automatically, use this to read out vertical direction where it should be actually moved. 
		/// It is value as you would get from standard unity Input, value is between 0 and 1.0
		/// Usefull if you implement your own logic for character movement.
		/// </summary>
		public float moveVertical=0f;
		/// <summary>
		/// How often position should be updated, in seconds.
		/// </summary>
		public float updateInterval = 0.1f;
		/// <summary>
		/// Movement speed, from 0 to 1 where 1 is max speed
		/// </summar
		public float moveSpeed = 0.5f;
		/// <summary>
		/// Min movement distance. In Scene units. 
		/// </summary>
		public float minMovement=10f;

		private float lastUpdatedPos=0;
		private Vector3 move2pos=Vector3.zero;
		private Vector3 startPos=Vector3.zero;
		private float journeyLength=0;
		private Camera camera;
		private float lastMoveTime;
		private Vector3 lastMovePosition = Vector3.zero;
		private Vector3 startEasePosition=Vector3.zero;
		private float startVertVal = 0f;
		private float startHorVal = 0f;

		/// <summary>
		/// Move limits within which character is allowed to move. Use scene units to define it.
		/// </summary>
		/// 
		[System.Serializable]
		public class MoveBoundary {
			
			public bool useBoundary;
			public float x1, x2, z1, z2;
		}


		// Use this for initialization
		void Start () {
		
			camera = Camera.main;
		}

		// Update is called once per frame
		void Update () {
		
			position = Vector3.up;
			Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
			Ray ray = camera.ScreenPointToRay(new Vector2(Screen.width/2,Screen.height/2));
			float rayDistance;
			if (groundPlane.Raycast(ray, out rayDistance))
				position = ray.GetPoint(rayDistance);

			if(boundary.useBoundary) {

				position = new Vector3 (Mathf.Clamp (position.x, boundary.x1, boundary.x2), 0.0f, Mathf.Clamp (position.z, boundary.z2, boundary.z1));
			}
			else
				position = new Vector3 (position.x, transform.position.y, position.z);

			if(moveObjectAutomatically) 
			{
				LigaboUtils.Log("move distance:"+Vector3.Distance(transform.position,position));
				LigaboUtils.Log("lastUpdatedPos+updateInterval:"+lastUpdatedPos+updateInterval);
				if (lastUpdatedPos+updateInterval<Time.time && Vector3.Distance(transform.position,position)>minMovement)
				{
					move2pos = position;
					startPos = transform.position;
					if (startPos==Vector3.zero)
						startPos=new Vector3(0.00001f,0f,0.00001f); //hack if the character is positioned at 0 at the beginning.
					lastUpdatedPos=Time.time;
					journeyLength = 0;
				}

				journeyLength = (Time.time - lastUpdatedPos)/(1-moveSpeed+0.011111111f);
				var time = journeyLength; //Mathf.SmoothStep(0.0f, 1.0f, journeyLength);

				LigaboUtils.Log("move2pos:"+move2pos);
				LigaboUtils.Log("startPos:"+startPos);

				if (startPos!=Vector3.zero && move2pos!=Vector3.zero){
					if(GetComponent<Rigidbody>() != null)
						GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(startPos, move2pos, time));
					else
						gameObject.transform.position = Vector3.Lerp(startPos, move2pos,  time);
				}
			}
			else if (Vector3.Distance(transform.position,position)>minMovement) {

				Vector3 referenceForward = Vector3.forward;
				Vector3 referenceRight= Vector3.Cross(Vector3.up, referenceForward);
				Vector3 newDirection = position - gameObject.transform.position;

				float angle = Vector3.Angle(newDirection, referenceForward);
				float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));

				float finalAngle = sign * angle;

				if(finalAngle < 0)
					finalAngle = 360f + finalAngle;

				moveVertical = Mathf.Cos(Mathf.Deg2Rad*finalAngle)*moveSpeed;
				moveHorizontal = Mathf.Sin(Mathf.Deg2Rad*finalAngle)*moveSpeed;
				lastMoveTime=Time.time;
				lastMovePosition = position;
				startEasePosition=Vector3.zero;
			}
			else
			{	
				if (startEasePosition==Vector3.zero){
					startEasePosition = transform.position;
					startVertVal = moveVertical;
					startHorVal = moveHorizontal;
				}
				var actualDistance = Vector3.Distance(transform.position,lastMovePosition);
				var startingDistance = Vector3.Distance(startEasePosition,lastMovePosition);
				var actualEaseFactor = 1f;
				if (startingDistance!=0)
					actualEaseFactor = 1f-actualDistance/startingDistance;
				//Debug.Log("lastMovePosition:"+lastMovePosition);
				moveVertical=Mathf.Lerp(startVertVal,0f,actualEaseFactor);
				moveHorizontal=Mathf.Lerp(startHorVal,0f,actualEaseFactor);


			}
		}

		void OnGUI () {

			if (!targetGUITexture)
				return;

			float circleWidth = targetGUITexture.width/2;
			float circleHeight = targetGUITexture.height/2;

			LigaboUtils.GUIScaleBegin();

			if(targetGUITexture != null) {
				if(LigaboState.statusTracking == LigaboState.StatusTracking.Tracking)
					GUI.DrawTexture(new Rect(LigaboUtils.screenWidth/2-targetGUITexture.width/3,LigaboUtils.screenHeight/2-targetGUITexture.height/4,circleWidth,circleHeight),targetGUITexture, ScaleMode.ScaleAndCrop, true);
			}

			LigaboUtils.GUIScaleEnd();
		}
	}
}
