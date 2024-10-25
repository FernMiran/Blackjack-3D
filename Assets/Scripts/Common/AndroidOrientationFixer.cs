using UnityEngine;
using UnityEngine.Android;

public class OrientationManager : MonoBehaviour
{
	void Awake()
	{
#if UNITY_ANDROID
		Debug.Log("Fixing Android rotation");

		// Force landscape mode
		Screen.orientation = ScreenOrientation.LandscapeLeft;

		// Optional: Allow both landscape orientations
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;

		Screen.orientation = ScreenOrientation.AutoRotation;
#endif
	}
}