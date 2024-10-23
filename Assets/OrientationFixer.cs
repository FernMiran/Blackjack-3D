using UnityEngine;

public class OrientationManager : MonoBehaviour
{
	void Awake()
	{
		// Force landscape mode
		Screen.orientation = ScreenOrientation.LandscapeLeft;

		// Optional: Allow both landscape orientations
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;

		Screen.orientation = ScreenOrientation.AutoRotation;
	}
}