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
		UI_DecisionPanel _decisionPanelPopupPrefab = null;
		UI_DecisionPanel _decisionPanelPopupInstance = null;

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
			_blackjackManager.OnPlayerDecided += HandlePlayerDecided;

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

		public void TakeDecision(Player player, Decision decision, Action<Player, Decision> callback = null)
		{
			callback?.Invoke(player, decision);
		}

		private void HandlePlayersCreated(Player[] players, Player player)
		{
			_playersHandsManager.Initialize(players, player);
		}

		private void HandleCardDealFinished(Deal deal)
		{
			_playersHandsManager.UpdatePanelWithDeal(deal);
		}

		private void HandleWaitForPlayerDecision(Player player, int hand, Action<Player, Decision> callback)
		{
			InstantiateDecisionPopup(player, hand, callback);
		}

		private void HandlePlayerDecided(Player player, Decision decision)
		{
			Destroy(_decisionPanelPopupInstance.gameObject);
		}

		private void HandleHandsResults(Dictionary<Hand, Results> results)
		{
			_playersHandsManager.UpdatePanelsWithResults(results);
		}

		private void HandleReset()
		{
			_playersHandsManager.ResetHands();
		}

		private void InstantiateDecisionPopup(Player player, int hand, Action<Player, Decision> callback)
		{
			_decisionPanelPopupInstance = Instantiate(_decisionPanelPopupPrefab, transform);
			_decisionPanelPopupInstance.Initialize(player, hand, callback);

			callback += HandlePlayerDecided;
		}
	}
}
