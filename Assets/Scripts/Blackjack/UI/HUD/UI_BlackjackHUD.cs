using System;
using System.Collections.Generic;
using UnityEngine;

namespace CasinoGames.Blackjack.UI
{
	public class UI_BlackjackHUD : MonoBehaviour
	{
		[Header("Game Logic")]
		[SerializeField]
		BlackjackManager _blackjackManager;

		[Header("Dependencies")]
		[SerializeField]
		UI_PlayersHandsManager _playersHandsManager; // Make an observer pattern for hands panels UI controller.

		[Header("Decision Popup")]
		[SerializeField]
		UI_DecisionPanel _decisionPanelPopup = null;

		private void Reset()
		{
			_blackjackManager = FindObjectOfType<BlackjackManager>();
			_playersHandsManager = GetComponentInChildren<UI_PlayersHandsManager>();
		}

		private void Awake()
		{
			_blackjackManager.OnPlayersCreated += HandlePlayersCreated;

			_blackjackManager.OnCardDealFinished += HandleCardDealFinished;
			_blackjackManager.OnWaitForPlayerDecision += HandleWaitForPlayerDecision;
			_blackjackManager.OnHandsResults += HandleHandsResults;

			_blackjackManager.OnBlackjackReset += HandleReset;
		}

		public void StartGame()
		{
			_blackjackManager.StartGame();
		}

		public void ResetGame()
		{
			_blackjackManager.ResetGame();
		}

		public void TakeDecision(Decision decision, Action<Decision> callback = null)
		{
			callback?.Invoke(decision);
		}

		private void HandlePlayersCreated(Player[] players, Player player)
		{
			_playersHandsManager.Initialize(players, player);
		}

		private void HandleCardDealFinished(Deal deal)
		{
			_playersHandsManager.UpdatePanelWithDeal(deal);
		}

		private void HandleWaitForPlayerDecision(Player player, int hand, Action<Decision> callback)
		{
			InstantiateDecisionPopup(player, hand, callback);
		}

		private void HandlePlayerDecided(Decision decision)
		{
			Destroy(_decisionPanelPopup.gameObject);
		}

		private void HandleHandsResults(Dictionary<Hand, Results> results)
		{
			_playersHandsManager.UpdatePanelsWithResults(results);
		}

		private void HandleReset()
		{
			_playersHandsManager.ResetHands();
		}

		private void InstantiateDecisionPopup(Player player, int hand, Action<Decision> callback)
		{
			_decisionPanelPopup = Instantiate(_decisionPanelPopup, transform);
			_decisionPanelPopup.Initialize(player, hand, callback);

			callback += HandlePlayerDecided;
		}
	}
}
