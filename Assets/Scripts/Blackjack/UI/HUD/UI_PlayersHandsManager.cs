using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CasinoGames.Blackjack.UI
{
	public class UI_PlayersHandsManager : MonoBehaviour
	{
		[SerializeField]
		List<UI_PlayerHandsController> _playersHands;

		Dictionary<Hand, UI_PlayerHandsController> _playerHandsDictionary = new Dictionary<Hand, UI_PlayerHandsController>();

		private void Reset()
		{
			_playersHands = GetComponentsInChildren<UI_PlayerHandsController>().ToList();
		}

		public void Initialize(Player[] players, Player dealer)
		{
			for (int playerIndex = 0; playerIndex < players.Length; playerIndex++)
			{
				Player player = players[playerIndex];

				for (int i = 0; i < player.Hands.Count; i++)
				{
					Hand hand = player.Hands[i];
					UI_PlayerHandsController playerHandsPanel = _playersHands[playerIndex];
					playerHandsPanel.Initialize(player);

					_playerHandsDictionary.Add(hand, playerHandsPanel);
				}
			}

			// Last of the player panels is for the dealer.
			UI_PlayerHandsController lastPlayerHands = _playersHands[_playersHands.Count - 1];
			lastPlayerHands.Initialize(dealer);

			_playerHandsDictionary.Add(dealer.Hands[0], lastPlayerHands);
		}

		public void UpdatePanelWithDeal(Deal deal)
		{
			if (_playerHandsDictionary.TryGetValue(deal.Hand, out UI_PlayerHandsController playerHands))
			{
				playerHands.UpdatePanelsValues();
			}
		}

		public void UpdatePanelsWithResults(Dictionary<Hand, Results> playersHandsResults)
		{
			foreach (KeyValuePair<Hand, Results> playerResult in playersHandsResults)
			{
				if (_playerHandsDictionary.TryGetValue(playerResult.Key, out UI_PlayerHandsController playerHandsController))
				{
					playerHandsController.UpdatePanelsWithResults(playersHandsResults);
				}
			}
		}

		public void Split(Hand hand)
		{
			if (_playerHandsDictionary.TryGetValue(hand, out UI_PlayerHandsController playerHandsController))
			{
				playerHandsController.Split();
			}
		}
		
		public void ResetHands()
		{
			foreach (KeyValuePair<Hand, UI_PlayerHandsController> handController in _playerHandsDictionary)
			{
				handController.Value.ResetHands();
			}
		}

	}
}
