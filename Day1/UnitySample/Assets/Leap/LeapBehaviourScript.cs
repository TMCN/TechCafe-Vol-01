using UnityEngine;
using System.Collections;
using Leap;

public class LeapBehaviourScript : MonoBehaviour {
	Controller leap;

	public int FingerCount;
	public GameObject[] FingerObjects;

	// Use this for initialization
	void Start () {
		leap = new Controller();
		for ( int i = 0; i < FingerObjects.Length; i++ ) {
			FingerObjects[i].transform.localScale = new Vector3( 10, 10, 10 );
		}
	}
	
	// Update is called once per frame
	void Update () {
		Frame frame = leap.Frame();
		FingerCount = frame.Fingers.Count;

		for ( int i = 0; i < FingerObjects.Length; i++ ) {
			var leapFinger = frame.Fingers[i];
			var unityFinger = FingerObjects[i];
			SetVisible( unityFinger, leapFinger.IsValid );
			if ( leapFinger.IsValid ) {
				unityFinger.transform.localPosition = ToVector3( leapFinger.TipPosition );
			}
		}
	}
	
	void SetVisible( GameObject obj, bool visible )
	{
		foreach( Renderer component in obj.GetComponents<Renderer>() ) {
			component.enabled = visible;
		}
	}
	
	Vector3 ToVector3( Vector v )
	{
		return new UnityEngine.Vector3( v.x, v.y, v.z );
	}
}
