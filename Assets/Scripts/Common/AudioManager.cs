using CasinoGames.Blackjack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generalized AudioManager to handle all audio playback, including sound effects, ambient sounds, UI sounds, and background music.
/// The audio clips are passed directly to the functions instead of being stored in arrays.
/// </summary>
public class AudioManager : MonoBehaviour
{
	#region Singleton
	public static AudioManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			Debug.LogWarning("More than one instance of " + this);
		}
		else
		{
			Instance = this;
		}

		Initialize();
	}
	#endregion

	[Header("Audio Sources")]
	public AudioSource musicSource; // Dedicated for playing music
	public AudioSource ambientSource; // For continuous ambient sounds
	public AudioSource uiSource; // For menu and UI-related sounds
	public int maxSFXSources = 10; // Max number of simultaneous SFX sources

	private List<AudioSource> activeSFXSources = new List<AudioSource>(); // Pool of active SFX sources
	private Queue<AudioSource> availableSFXSources = new Queue<AudioSource>(); // Pool for reusable SFX sources

	private void Initialize()
	{
		// Create and prepare audio sources for the SFX pool
		for (int i = 0; i < maxSFXSources; i++)
		{
			AudioSource newSFXSource = gameObject.AddComponent<AudioSource>();
			newSFXSource.playOnAwake = false;
			availableSFXSources.Enqueue(newSFXSource);
		}
	}

	/// <summary>
	/// Plays a specific sound effect.
	/// </summary>
	/// <param name="clip">The sound effect to play.</param>
	public void PlaySoundEffect(AudioClip clip)
	{
		PlaySFX(clip);
	}

	/// <summary>
	/// Plays a specific ambient sound. Only one ambient sound is played at a time.
	/// </summary>
	/// <param name="clip">The ambient sound to play.</param>
	public void PlayAmbientSound(AudioClip clip)
	{
		if (clip == null) return;

		ambientSource.clip = clip;
		ambientSource.loop = true;
		ambientSource.Play();
	}

	/// <summary>
	/// Plays a specific background music clip.
	/// </summary>
	/// <param name="clip">The music clip to play.</param>
	public void PlayBackgroundMusic(AudioClip clip)
	{
		if (clip == null) return;

		if (musicSource.isPlaying)
		{
			StartCoroutine(FadeOutFadeIn(musicSource, clip, 1f)); // Optional fade-out for smooth transitions
		}

		musicSource.clip = clip;
		//musicSource.loop = true;
		musicSource.Play();
	}

	private IEnumerator FadeOutFadeIn(AudioSource source, AudioClip clip, float duration)
	{
		if (source.isPlaying)
		{
			yield return StartCoroutine(FadeOut(source, duration)); // Optional fade-out for smooth transitions
		}

		Debug.Log("Faded out finished. Playing next clip");
		source.clip = clip;
		source.Play();
	}

	public void StopBackgroundMusic()
	{
		if (musicSource.isPlaying)
		{
			StartCoroutine(FadeOut(musicSource, 1f)); // Optional fade-out for smooth transitions
		}
		musicSource.Stop();
	}

	/// <summary>
	/// Plays menu music, ensuring any other background music is stopped.
	/// </summary>
	/// <param name="clip">The menu music clip to play.</param>
	public void PlayMenuMusic(AudioClip clip)
	{
		if (clip == null) return;

		if (musicSource.isPlaying)
		{
			StartCoroutine(FadeOut(musicSource, 1f));
		}

		musicSource.clip = clip;
		musicSource.loop = true;
		musicSource.Play();
	}

	/// <summary>
	/// Plays a sound effect from the pool of available sources. Ensures the pool does not exceed the limit.
	/// </summary>
	/// <param name="clip">The AudioClip to play.</param>
	private void PlaySFX(AudioClip clip)
	{
		if (clip == null) return;

		if (availableSFXSources.Count > 0)
		{
			AudioSource sfxSource = availableSFXSources.Dequeue();
			activeSFXSources.Add(sfxSource);
			sfxSource.clip = clip;
			sfxSource.Play();
			StartCoroutine(ReturnToPoolAfterPlayback(sfxSource, clip.length));
		}
		else
		{
			Debug.LogWarning("Max SFX sources reached. Consider increasing the pool size.");
		}
	}

	/// <summary>
	/// Returns an AudioSource back to the pool after it finishes playing.
	/// </summary>
	/// <param name="source">The AudioSource to return.</param>
	/// <param name="delay">The delay in seconds to return the source (should match the clip's length).</param>
	private IEnumerator ReturnToPoolAfterPlayback(AudioSource source, float delay)
	{
		yield return new WaitForSeconds(delay);
		source.Stop();
		activeSFXSources.Remove(source);
		availableSFXSources.Enqueue(source);
	}

	/// <summary>
	/// Smoothly fades out an AudioSource over a given duration.
	/// </summary>
	/// <param name="source">The AudioSource to fade out.</param>
	/// <param name="duration">The duration of the fade.</param>
	private IEnumerator FadeOut(AudioSource source, float duration)
	{
		float startVolume = source.volume;

		while (source.volume > 0)
		{
			Debug.Log("Fading out " + source.volume);
			source.volume -= startVolume * Time.deltaTime / duration;
			yield return null;
		}

		Debug.Log("Faded out.");

		source.Stop();
		source.volume = startVolume; // Reset volume for future use
	}
}

