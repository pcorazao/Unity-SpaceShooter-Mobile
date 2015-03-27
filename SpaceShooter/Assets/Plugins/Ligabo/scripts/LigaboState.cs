/*----------------------------------------------------------
Copyright (c) 2014 Analytica UK Ltd. All Rights Reserved.
Confidential and Proprietary
----------------------------------------------------------*/

using UnityEngine;
using System.Collections;

namespace Ligabo {

	/// <summary>
	/// Ligabo is main static class which keeps state of AR tracking and content showing. 
	/// </summary>
	/// 
	public class LigaboState : MonoBehaviour {


		public enum Status {
			Running,
			Stopped
		};

		public enum StatusTracking {
			NotTracking,
			Tracking
		};


		/// <summary>
		/// The status of Ligabo. Set status to Running to start Ligabo functionality. 
		/// This will show and hide target info GUI when tracking is lost or found. Default stopped.
		/// </summary>
		public static Status status = Status.Stopped;
		/// <summary>
		/// The status of tracking. Indicated whether target is being tracked by AR library. Default is not tracking.
		/// </summary>
		public static StatusTracking statusTracking = StatusTracking.NotTracking;
		/// <summary>
		/// Keep this assigned to ligabo skin for default target information GUI overlay. Set to yours if you wish to chnage it.
		/// </summary>
		public GUISkin ligaboSkin;
		/// <summary>
		/// Texture of board icon shown in info GUI find target overlay. Should always be the latest gameboard picture in active dataset.
		/// </summary>
		public Texture miniBoardIcon;
		/// <summary>
		/// Once Ligabo is in running state and target is not being tracked, GUI with information to point camera to liveGameBoard is shown.
		/// You can customize its look by copying skin and modifying its custom styles and asigning it to ligaboSkin variable in inspector.
		/// If you want to provide these instructions yourself, select this option and ligabo library won't show this GUI info. 
		/// </summary>
		public bool doNotUseFindTargetGUI=false;
		/// <summary>
		/// Allow or don't showing content of target (child gameobjects) when target starts to be tracked. This is usefull in case when
		/// you want to show some intermediate objects in AR on target before showing actual target content. When you are ready to show
		/// the content just change this to true. 
		/// </summary>
		public static bool allowShowingTargetContent=true;
		/// <summary>
		/// Select this option when you want to forbid content hiding when tracking is lost
		/// </summary>
		public static bool doNotHideARContent=false;
		/// <summary>
		/// The initial status of Ligabo. Set initial status in inspector on Ligabo game object.
		/// </summary>
		public Status initialStatus = Status.Stopped;


		public static LigaboState instance;

		public void Awake()
		{
			LigaboState.instance = this;
		}

		void Start(){
			LigaboState.status = initialStatus;
		}	

		/// <summary>
		/// Monobehaviour method used to show information find target overlay
		/// 
		/// </summary>
		void OnGUI(){
			if (doNotUseFindTargetGUI)
				return;


			GUI.skin = ligaboSkin;

			//Debug.Log("ligabostatus:"+status);

			LigaboUtils.GUIScaleBegin()  ;

			if (status == Status.Running && statusTracking==StatusTracking.NotTracking)
			{
				DrawTargetWindow();

			}

			LigaboUtils.GUIScaleEnd()  ;
		}

		/// <summary>
		/// This function is drawing find target information GUI window.
		/// </summary>
		/// 
		/// 
		private void DrawTargetWindow ()
		{
			var targetWidth = 540;
			var targetHeight = 430;
			
			//GUI.Box(new Rect(0, 0, LigaboUtils.screenWidth, LigaboUtils.screenHeight), "", GUI.skin.GetStyle("scoreboard") );
			GUI.Box(new Rect(LigaboUtils.screenWidth/2-targetWidth/2,LigaboUtils.screenHeight/2-targetHeight/2,targetWidth,targetHeight),"",GUI.skin.GetStyle("targetFinder"));
			
			var boardWidth = targetWidth*0.8f;
			var boardHeight = targetHeight*0.7f;
			GUI.DrawTexture(new Rect(LigaboUtils.screenWidth/2-boardWidth/2, LigaboUtils.screenHeight/2-boardHeight/2,boardWidth,boardHeight),miniBoardIcon);
			
			var labelRect = new Rect(LigaboUtils.screenWidth/2-boardWidth/2*0.9f, LigaboUtils.screenHeight/2-40,boardWidth*0.9f,80);
			GUI.Label(labelRect,"",GUI.skin.GetStyle("labelCenterWithBack"));
			GUI.Label(labelRect,"Point camera to Live Game Board",GUI.skin.GetStyle("labelCenterWithBack"));
			
			var butRect = new Rect(LigaboUtils.screenWidth/2-(targetWidth*0.6f)/2, LigaboUtils.screenHeight/2+targetHeight/2-60,targetWidth*0.6f,50);
			GUIStyle butStyle = GUI.skin.GetStyle("goButton");
			butStyle.fontSize=20;
			if(GUI.Button(butRect,"Don't have one? Get it here.",butStyle))
				Application.OpenURL("http://www.livegameboard.com/shop/?game");
		}



	}
}
