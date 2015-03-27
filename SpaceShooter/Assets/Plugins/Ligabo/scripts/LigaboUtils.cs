/*----------------------------------------------------------
Copyright (c) 2014 Analytica UK Ltd. All Rights Reserved.
Confidential and Proprietary
----------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ligabo;

namespace Ligabo {

	/// <summary>
	/// Set of handy AR related utilities. 
	/// </summary>
	/// 
	public static class LigaboUtils {


		public static float screenWidth = 960;         
		public static float screenHeight = 540;          

		public static bool debug = false;

		/// <summary>
		/// Ligabo GUI scaler is designed to scale GUI while keeping aspect of GUI elements, supporting screen orientation change in real time and still being referenced to screen width and height.
		/// 
		/// Call this static function as LigaboUtils.GUIScaleBegin(); 
		/// in your OnGUI() where you want scaler to begin to scale your GUI elements.
		/// When this scaler is used, reference all your GUI positions relative to LigaboUtils.screenWidth instead of Screen.width. Similarly with Heigh.
		/// Scaler will then do its magic and will scale entire GUI while keeping aspect. It will fit either height or width of oyur screen depends on screen aspect.
		/// Scaler adapts correctly if rotation of device is allowed as well.
		/// Feel free to modify screenWidth and screenHeight in one of your scripts Start method to fit screen size you have  your GUI referenced to already. 
		/// </summary>
		/// 
		static public void GUIScaleBegin()        
		{                

			//portrait mode, swap with and height
			if (Screen.width<Screen.height && screenWidth>screenHeight)
			{
				//Debug.Log("portrait mode, swaping reference width and height");
				var w=screenWidth;
				screenWidth=screenHeight;
				screenHeight=w;
			} //put it back if rotated
			else if (Screen.width>Screen.height && screenWidth<screenHeight){
				//Debug.Log("lanscape mode, swaping reference width and height");
				var w=screenWidth;
				screenWidth=screenHeight;
				screenHeight=w;
			}


			Vector3 offsetScale = GetOffsetAndScaleFactor();
			GUI.matrix = Matrix4x4.TRS(new Vector3(offsetScale.x, offsetScale.y, 0), Quaternion.identity, new Vector3(offsetScale.z, offsetScale.z, 1));

			//GUIUtility.ScaleAroundPivot( new Vector2 (factor, factor), new Vector2(0,0));

			//GUIUtility.ScaleAroundPivot( new Vector2 (1, 1), new Vector2(Screen.width/2,Screen.height/2));

			
		}

		/// <summary>
		/// Call this static function as LigaboUtils.GUIScaleEnd(); at the end of OnGUI method where you have used  GUIScaleBegin().
		/// It must be called to get scale matrix back to its original form.
		/// </summary>
		/// 
		
		static public void GUIScaleEnd()         
		{  
			Vector3 offsetScale = GetOffsetAndScaleFactor();
			GUI.matrix = Matrix4x4.TRS(new Vector3(offsetScale.x, offsetScale.y, 0), Quaternion.identity, new Vector3(1/offsetScale.z, 1/offsetScale.z, 1));
		}


		/**
		 *	Returns angle in degrees between main or given camera and vertical screen axis projected on zero plane.
		 *	This fix is specifically needed for AR play mode when navigating character by joystick
		 *	Because player can point camera to game board from any angle his perception of what means UP is not just Z axis.
		 *	Up is basically actual camera direction, projected on main playground surface which is target plane.
		 *  @return float camera rotation fix
		 */

		public static float GetCameraFix(Camera cameraObject = null){
			if (cameraObject==null)
				cameraObject = Camera.main;
			
			float ent = 100000.0f;
			Vector3 point1 = Vector3.zero;
			Vector3 point2 = Vector3.zero;
			Plane plane = new Plane(Vector3.up,Vector3.zero);
			
			//raycast to zero plane from center of screen
			Ray ray= cameraObject.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));
			
			if (plane.Raycast(ray, out ent))
			{
				//Debug.Log("Plane Raycast hit at distance: " + ent);
				point1= ray.GetPoint(ent);
			}
			
			//raycast to zero plane from center bottom of the screen
			ray= Camera.main.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height,0));
			ent = 100000.0f;
			if (plane.Raycast(ray, out ent))
			{
				//Debug.Log("Plane Raycast hit at distance: " + ent);
				point2= ray.GetPoint(ent);
			}
			
			//if we have both hits, we return angle between them and global Z axis
			if (point1!=Vector3.zero && point2!=Vector3.zero)
			{
				//Debug.Log("GetCameraFix got hit");
				Vector3 direction = point2-point1;
				//check for position
				float position = Mathf.Sign(Vector3.Dot(Vector3.Cross(Vector3.Cross (Vector3.forward, direction), Vector3.forward), Vector3.right));
				//occationaly can be 0 if exactly on line
				if (position==0) position=1;
				return Vector3.Angle(Vector3.forward, direction)*position;	
				
			}
			
			return 0;
		}



		/**
		 *	Log function used internally by Ligabo code to identify its logs in console.
		 *	You should not use this function yourself
		 */
		public static void Log(string log){
			if(!LigaboUtils.debug)
				return;
			Debug.Log("@Ligabo>"+log);
		}



		static private Vector3 GetOffsetAndScaleFactor(){
			
			float factor;
			float offsetX;
			float offsetY;
			
			//width does fit lets use it as reference in scaling
			if (Screen.width/screenWidth<Screen.height/screenHeight)
			{
				//Debug.Log("width fits first");
				factor = Screen.width / screenWidth;
				offsetX=0;
				offsetY= (Screen.height-screenHeight*factor)/2*factor;
				
			}
			else
			{
				//Debug.Log("height fits first, scrHeigh:"+screenHeight);
				factor = Screen.height  / screenHeight;
				offsetY=0;
				offsetX=(Screen.width-screenWidth*factor)/2*factor;
				
			}
			
			//Debug.Log("scale factor:"+factor+", offx:"+offsetX+", offy:"+offsetY);
			return new Vector3(offsetX,offsetY,factor);
		}

	}
}
