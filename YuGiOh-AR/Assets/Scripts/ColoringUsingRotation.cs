using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ColoringUsingRotation : MonoBehaviour, ITrackableEventHandler {
	public string tag;
	// Use this for initialization

    protected TrackableBehaviour mTrackableBehaviour;
	int foundobj = 0;
    protected virtual void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
		Debug.Log("Trackable2 " + mTrackableBehaviour.TrackableName + " START");
    }

    protected virtual void OnDestroy()
    {
        if (mTrackableBehaviour)
            mTrackableBehaviour.UnregisterTrackableEventHandler(this);
		Debug.Log("Trackable2 " + mTrackableBehaviour.TrackableName + " DESTROYED");
    }

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {	
		if(newStatus == TrackableBehaviour.Status.TRACKED){
			Debug.Log("Trackable2 " + mTrackableBehaviour.TrackableName + " tracked");
			if(foundobj == 0){
				Debug.Log("FIRST TIME");
				foundobj += 1;
				OnTrackingFoundFirstTime();
			}
			else{
				Debug.Log("Trackable2 " + mTrackableBehaviour.TrackableName + " found");
            	OnTrackingFound();	
			}
		}
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            Debug.Log("Trackable2 " + mTrackableBehaviour.TrackableName + " found");
            OnTrackingFound();
        }
    }

	protected virtual void OnTrackingFoundFirstTime()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

		

        // Enable rendering:
        foreach (var component in rendererComponents){
            component.enabled = true;
			var angleY = component.transform.rotation.eulerAngles.y;
			if(angleY < 180){
				Debug.Log(angleY);
				component.material.color = new Color32(100,150,100,90);
			}
		}

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;
    }

    protected virtual void OnTrackingFound()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Enable rendering:
        foreach (var component in rendererComponents){
            component.enabled = true;
		}

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;
    }
}
