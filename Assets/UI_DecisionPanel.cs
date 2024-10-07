using System;
using UnityEngine;

namespace CasinoGames.Blackjack.UI
{
	// We should do a IDecisionPanel interface, and create two types of DecisionPanel: Static, and Instantiated, so we can retrieve the Blackjack HUD controller by Reset or Awake methods.
	public class UI_DecisionPanel : MonoBehaviour
	{
		[SerializeField]
		UI_BlackjackHUD _blackjackHUD;
		
		[SerializeField]
		Player _player;
		[SerializeField]
		Hand _hand;

		Action<Decision> _callbackHolder = null;

		private void Reset()
		{
			_blackjackHUD = FindObjectOfType<UI_BlackjackHUD>();
		}

		private void Awake()
		{
			if (_blackjackHUD == null)
				_blackjackHUD = FindObjectOfType<UI_BlackjackHUD>();
		}

		public void Initialize(Player player, int hand, Action<Decision> callback = null)
		{
			_player = player;
			_hand = player.Hands[hand];

			_callbackHolder = callback;

			ShowAvailableDecisions();
		}

		public void ClickHit()
		{
			Decide(Decision.Hit);
		}

		public void ClickStand()
		{
			Decide(Decision.Stand);
		}

		public void ClickDouble()
		{
			//Decide(Decision.Double);
		}
		
		public void ClickSplit()
		{
			//Decide(Decision.Split);
		}
		
		private void ShowAvailableDecisions()
		{
			// Check hand state.
			// Disable unavailable buttons
		}

		private void Decide(Decision decision)
		{
			_blackjackHUD.TakeDecision(decision, _callbackHolder);
		}

	}
}