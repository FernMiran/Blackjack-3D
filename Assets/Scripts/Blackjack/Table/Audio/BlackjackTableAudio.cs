using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CasinoGames.Blackjack
{
	public class BlackjackTableAudio : MonoBehaviour // Change name to BlackjackAudio?
	{
		[SerializeField]
		BlackjackManager _blackjackManager;

		[Header("SFX")]
		[SerializeField]
		AudioClip _dealCard;
		[SerializeField]
		AudioClip _flipCard;

		[Header("Music")]
		[SerializeField]
		bool _play;
		[SerializeField]
		List<AudioClip> _musicClips;
		[SerializeField]
		int _currentMusicClipIndex;
		private Coroutine _musicRoutine;

		private void Reset()
		{
			_blackjackManager = FindObjectOfType<BlackjackManager>();

			_musicClips = Resources.LoadAll<AudioClip>("Music").ToList();
		}

		private void Awake()
		{
			_blackjackManager.OnCardDealStarted += HandleCardDealStarted;
			_blackjackManager.OnCardFlipStarted += HandleCardFlipStarted;
		}
		void Start()
		{
			if (_musicClips.Count > 0)
			{
				StartPlaying();
			}
		}

		public void StartPlaying()
		{
			if (_musicRoutine != null)
			{
				StopCoroutine(_musicRoutine);
			}

			_musicRoutine = StartCoroutine(PlayNextClipWithDelay());
		}

		IEnumerator PlayNextClipWithDelay()
		{
			while (_play)
			{
				if (_currentMusicClipIndex >= _musicClips.Count)
				{
					_currentMusicClipIndex = 0;  // Loop back to the start
				}

				AudioClip audioClip = _musicClips[_currentMusicClipIndex];

				// Play the next clip
				AudioManager.Instance.PlayBackgroundMusic(_musicClips[_currentMusicClipIndex]);

				Debug.Log("[>] Music clip:" + audioClip.name + " with duration of: " + audioClip.length + "s has started: " + Time.deltaTime);

				// Wait for the clip's duration before playing the next one
				yield return new WaitForSeconds(audioClip.length);

				Debug.Log("[X] Music clip:" + audioClip.name + " with duration of " + audioClip.length + "s has ended: " + Time.deltaTime);

				_currentMusicClipIndex++;
			}
			AudioManager.Instance.StopBackgroundMusic();
			Debug.Log("Music stopped.");
		}

		private void PlayNext()
		{
			if (_musicClips.Count == 0) return; // Safety check if no clips are added

			// Loop back to the first clip after finishing the last one
			if (_currentMusicClipIndex >= _musicClips.Count)
			{
				_currentMusicClipIndex = 0;
			}

			// Play the next clip
			AudioManager.Instance.PlayBackgroundMusic(_musicClips[_currentMusicClipIndex]);

			_currentMusicClipIndex++;
		}

		private void HandleCardDealStarted(Deal deal)
		{
			AudioManager.Instance.PlaySoundEffect(_dealCard);
		}

		private void HandleCardFlipStarted(Flip flip)
		{
			AudioManager.Instance.PlaySoundEffect(_flipCard);
		}
	}
}