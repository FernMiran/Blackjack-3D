using System;
using System.Collections.Generic;
using UnityEngine;

namespace CasinoGames.Blackjack.UI
{
	public class TableHandsManager : MonoBehaviour
	{
		[SerializeField]
		BlackjackManager _blackjackManager;
		[SerializeField]
		BlackjackTable _blackjackTable; // Create BlackjackConfiguration to store Speeds and Spacings and remove this dependency

		[SerializeField]
		TableHandsController[] _tableHands;
		[SerializeField]
		TableHandsController _dealerTableHand;

		Dictionary<Player, TableHandsController> _tableHandsControllerDictionary = new Dictionary<Player, TableHandsController>();

		private void Reset()
		{
			_blackjackManager = FindObjectOfType<BlackjackManager>();
			_blackjackTable = GetComponentInParent<BlackjackTable>();

			_tableHands = GetComponentsInChildren<TableHandsController>();
		}

		private void Awake()
		{
			_blackjackManager.OnPlayersCreated += HandlePlayerCreated;
		}

		private void HandlePlayerCreated(Player[] players, Player dealer)
		{
			for (int i = 0; i < players.Length; i++)
			{
				_tableHandsControllerDictionary.Add(players[i], _tableHands[i]);
				_tableHands[i].Initialize(players[i].Hands.ToArray());
			}

			_tableHandsControllerDictionary.Add(dealer, _dealerTableHand);
			_dealerTableHand.Initialize(dealer.Hands.ToArray());
		}
		
		public Vector3 GetNextCardPosition(Player player, Hand hand, Vector3 spacing)
		{
			TableHandsController tableHandsController = GetPlayerTableHandsController(player);
			tableHandsController.GetNextCardPositionInHand(hand, spacing);

			return tableHandsController.GetNextCardPositionInHand(hand, spacing); ;
		}

		public TableHandsController GetPlayerTableHandsController(Player player)
		{
			TableHandsController playerTableHand;

			// If player not found in regular players
			if (!_tableHandsControllerDictionary.TryGetValue(player, out playerTableHand))
			{
				// Check if it's the dealer
				if (player.IsDealer)
				{
					playerTableHand = _dealerTableHand;
				}
				else
				{
					Debug.LogError("Player " + player.Name + " has no UI object to animate a deal.");
				}
			}

			return playerTableHand;
		}

		public void EnableBlinking(Player player, Hand hand, bool enableBlinking)
		{
			TableHandsController tableHandsController = GetPlayerTableHandsController(player);
			tableHandsController.EnableBlinking(hand, enableBlinking);
		}
	}
}
