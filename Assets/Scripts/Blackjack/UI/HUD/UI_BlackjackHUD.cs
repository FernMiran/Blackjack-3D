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

		[Header("Sub-Modules")]
		[SerializeField]
		UI_PlayersHandsManager _playersHandsManager;
		[SerializeField]
		UI_Panel _gameButtonsPanel;

		[Header("Decision Popup")]
		[SerializeField]
		UI_DecisionPanel _decisionPanelPopupPrefab = null;
		UI_DecisionPanel _decisionPanelPopupInstance = null;

		private void Reset()
		{
			_blackjackManager = FindObjectOfType<BlackjackManager>();

			// Get UI Modules
			_playersHandsManager = GetComponentInChildren<UI_PlayersHandsManager>();
			_gameButtonsPanel = GetComponentInChildren<UI_GameButtonsPanel>();
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
			_gameButtonsPanel.Hide();
		}

		public void ResetGame()
		{
			_blackjackManager.ResetGame();
		}

		public void QuitGame()
		{
			GameManager.QuitGame();
		}

		public void TakeDecision(Player player, Decision decision, Action<Player, Decision> callback = null)
		{
			callback?.Invoke(player, decision);
		}

		private void HandlePlayersCreated(Player[] players, Player dealer)
		{
			_playersHandsManager.Initialize(players, dealer);
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
			_gameButtonsPanel.Show();
		}

		private void HandleReset()
		{
			_playersHandsManager.ResetAllHands();
		}

		private void InstantiateDecisionPopup(Player player, int hand, Action<Player, Decision> callback)
		{
			_decisionPanelPopupInstance = Instantiate(_decisionPanelPopupPrefab, transform);
			_decisionPanelPopupInstance.Initialize(player, hand, callback);

			callback += HandlePlayerDecided;
		}
	}
}
