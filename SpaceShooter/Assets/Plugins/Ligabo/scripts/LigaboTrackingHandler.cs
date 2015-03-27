/*----------------------------------------------------------
Copyright (c) 2014 Analytica UK Ltd. All Rights Reserved.
Confidential and Proprietary
----------------------------------------------------------*/


using UnityEngine;
using System.Collections;


namespace Ligabo {

	/// <summary>
	/// Use this class attached to your Ligabo AR Target to keep automatic content showing on thacking state change.
	/// Class is hooked to AR library and is listening to AR tracking events.
	/// When AR target was detected in camera feed and is being tracked content (child gameobjects) of this target gameobject is shown or hidden 
	/// depending on LigaboState.allowShowingTargetContent flag. 
	/// </summary>

	public class LigaboTrackingHandler : MonoBehaviour, ITrackableEventHandler
	{
		#region PUBLIC_MEMBER_VARIABLES
		/// <summary>
		/// Select this option if this target does not have any content and serves only secondary target recognition which should show the same content as primary target.
		/// Only one target in the scene can be marked primary.
		/// </summary>
		public bool isSecondary=false;

		#endregion // PUBLIC_MEMBER_VARIABLES


		#region PRIVATE_MEMBER_VARIABLES

		private TrackableBehaviour mTrackableBehaviour;
		private LigaboTrackingHandler mainTarget;
		private bool contentShowing=true;
		private bool tracking = false;
		private Vector3 startPosition;
		private Quaternion startRotation;
		
		#endregion // PRIVATE_MEMBER_VARIABLES
		
		
		
		#region UNTIY_MONOBEHAVIOUR_METHODS
		/// <summary>
		/// Monobehaviour function used for initialization and AR events registration
		/// 
		/// </summary>
		void Start()
		{
			startPosition = transform.position;
			startRotation = transform.rotation;
			mTrackableBehaviour = GetComponent<TrackableBehaviour>();
			if (mTrackableBehaviour)
			{
				mTrackableBehaviour.RegisterTrackableEventHandler(this);
			}
			
			OnTrackingLost();
			
			//get default if this is not default
			if (isSecondary){
				
				LigaboTrackingHandler[] targets = FindObjectsOfType<LigaboTrackingHandler>();
				foreach (LigaboTrackingHandler target in targets)
					if (!target.isSecondary)
						mainTarget = target;
				
				if (mainTarget==null)
					LigaboUtils.Log("Ligabo: No main target was found in scene, if you user secondary targets you must have one primary in the scene");
				
			}
		}
		
		#endregion // UNTIY_MONOBEHAVIOUR_METHODS
		/// <summary>
		/// Monobehaviour method used for checking whether content should be shown or not
		/// 
		/// </summary>
		void Update(){
			
			//Smart terrain HACK for stability
			//reset rotation every few as smart terrain sometimes rotates it unexpectedly
			//target should never move 
			if (transform.rotation!=startRotation)
			{
				transform.rotation=startRotation;
				transform.position=startPosition;
			}
			
			if (LigaboState.allowShowingTargetContent && tracking && !contentShowing)
				ShowTargetContent();
			
			if ((!tracking && contentShowing && !LigaboState.doNotHideARContent) || (!LigaboState.allowShowingTargetContent && tracking && contentShowing))
				HideTargetContent();

			
		}
		
		
		#region PUBLIC_METHODS
		
		/// <summary>
		/// Called when the trackable state has changed.
		/// </summary>
		/// <param name="previousStatus">Previous status.</param>
		/// <param name="newStatus">New status.</param>
		public void OnTrackableStateChanged(
			TrackableBehaviour.Status previousStatus,
			TrackableBehaviour.Status newStatus)
		{
			if (newStatus == TrackableBehaviour.Status.DETECTED ||
			    newStatus == TrackableBehaviour.Status.TRACKED ||
			    newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
			{
				OnTrackingFound();
			}
			else
			{
				OnTrackingLost();
			}
		}
		
		
		/// <summary>
		/// Raises the tracking found event.
		/// </summary>
		public void OnTrackingFound()
		{
			
			//if secondary target, call this on primary one
			if (isSecondary)
			{
				if (mainTarget!=null)
					mainTarget.OnTrackingFound();
			}
			
			tracking=true;
			
			LigaboState.statusTracking=LigaboState.StatusTracking.Tracking;
			
		}
		
		/// <summary>
		/// Raises the tracking lost event.
		/// </summary>
		public void OnTrackingLost()
		{
			
			//if secondary target, call this on primary one
			if (isSecondary)
			{
				if (mainTarget!=null)
					mainTarget.OnTrackingLost();
				
				return;
			}
			
			tracking = false;
			
			LigaboState.statusTracking=LigaboState.StatusTracking.NotTracking;
			
		}
		
		#endregion // PUBLIC_METHODS
		
		
		
		#region PRIVATE_METHODS
		
		private void HideTargetContent(){
			Debug.Log("HideTargetContent");
			contentShowing = false;
			
			Renderer[] rendererComponents = GetComponentsInChildren<Renderer>();
			// Enable rendering:
			foreach (Renderer component in rendererComponents) {
				component.enabled = false;
			}
			
			
			Terrain[] TerrainComponents = GetComponentsInChildren<Terrain>();
			
			// Enable rendering:
			foreach (Terrain component in TerrainComponents) {
				component.enabled = false;
			}
			
		}
		
		private void ShowTargetContent(){
			Debug.Log("ShowTargetContent");
			contentShowing = true;
			
			Renderer[] rendererComponents = GetComponentsInChildren<Renderer>();
			
			// Enable rendering:
			foreach (Renderer component in rendererComponents) {
				component.enabled = true;
			}
			
			Terrain[] TerrainComponents = GetComponentsInChildren<Terrain>();
			
			// Enable rendering:
			foreach (Terrain component in TerrainComponents) {
				component.enabled = true;
			}
		}
		
		
		#endregion // PRIVATE_METHODS
	}
}
