using System;
using System.Collections;
using UnityEngine;

public class RotateBehaviour : MonoBehaviour
{
	Coroutine _coroutine;

	public IEnumerator Rotate(Vector3 targetAngles, bool flip = true, float duration = 1f, Action callback = null, float delay = 0f)
	{
		if (_coroutine != null)
		{
			StopCoroutine(_coroutine);
		}
		_coroutine = StartCoroutine(RotationCoroutine(targetAngles, flip, duration, callback, delay));
		yield return _coroutine;
	}

	private IEnumerator RotationCoroutine(Vector3 targetAngles, bool flip = true, float duration = 1f, Action callback = null, float delay = 0f)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}

		Quaternion startRotation = transform.rotation;
		Quaternion targetRotation = Quaternion.Euler(targetAngles);

		// Calculate the axis of rotation for the flip
		Vector3 flipAxis = transform.parent.up; // Assuming the parent is the table

		float elapsedTime = 0f;
		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / duration;

			// Use smooth step for easing
			float smoothT = Mathf.SmoothStep(0, 1, t);

			Quaternion currentRotation;
			if (flip)
			{
				// Perform the flip rotation around the calculated axis
				Quaternion flipRotation = Quaternion.AngleAxis(180 * smoothT, flipAxis);
				currentRotation = startRotation * flipRotation;
			}
			else
			{
				// If not flipping, just lerp between start and target rotations
				currentRotation = Quaternion.Slerp(startRotation, targetRotation, smoothT);
			}

			transform.rotation = currentRotation;
			yield return null;
		}

		// Set final rotation
		transform.rotation = flip ? targetRotation : Quaternion.Euler(startRotation.eulerAngles.x, startRotation.eulerAngles.y, targetAngles.z);

		callback?.Invoke();
	}
}
