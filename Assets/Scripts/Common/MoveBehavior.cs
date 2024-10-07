using UnityEngine;
using System.Collections;
using System;

public class MoveBehavior : MonoBehaviour
{
	private Coroutine _currentLerpCoroutine;

	public IEnumerator Move(Vector3 targetPosition, Action callback = null, float duration = 1f, float delay = 0f)
	{
		if (_currentLerpCoroutine != null)
		{
			StopCoroutine(_currentLerpCoroutine);
		}

		_currentLerpCoroutine = StartCoroutine(MoveCoroutine(targetPosition, duration, callback, delay));
		yield return _currentLerpCoroutine;
	}

	private IEnumerator MoveCoroutine(Vector3 targetPosition, float duration = 1f, Action callback = null, float delay = 0f)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}

		Vector3 startPosition = transform.position;
		float elapsedTime = 0f;

		// Perform horizontal (x) and vertical (y) movement simultaneously
		while (elapsedTime < duration * 0.7f) // Use 70% of the duration for this phase
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / (duration * 0.7f);

			// Use different easing functions for a more natural movement
			float xProgress = EaseOutQuad(t);
			float yProgress = EaseInOutQuad(t);

			transform.position = new Vector3(
				Mathf.Lerp(startPosition.x, targetPosition.x, xProgress),
				Mathf.Lerp(startPosition.y, targetPosition.y + 0.5f, yProgress), // Add a slight arc
				startPosition.z
			);

			yield return null;
		}

		// Reset elapsedTime for the frontal (z) movement
		elapsedTime = 0f;
		Vector3 currentPosition = transform.position;

		// Perform frontal (z) movement and final y adjustment
		while (elapsedTime < duration * 0.3f) // Use remaining 30% of the duration for this phase
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / (duration * 0.3f);

			float zProgress = EaseInQuad(t);
			float yProgress = EaseInQuad(t);

			transform.position = new Vector3(
				currentPosition.x,
				Mathf.Lerp(currentPosition.y, targetPosition.y, yProgress),
				Mathf.Lerp(currentPosition.z, targetPosition.z, zProgress)
			);

			yield return null;
		}

		// Ensure the final position is set correctly
		transform.position = targetPosition;

		// Invoke the callback if provided
		callback?.Invoke();
	}

	// Easing functions for smoother movement
	private float EaseOutQuad(float t) => 1 - (1 - t) * (1 - t);
	private float EaseInQuad(float t) => t * t;
	private float EaseInOutQuad(float t) => t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
}
