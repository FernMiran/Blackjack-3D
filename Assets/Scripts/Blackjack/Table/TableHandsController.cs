using System.Collections.Generic;
using UnityEngine;

namespace CasinoGames.Blackjack.UI
{
	public class TableHandsController : MonoBehaviour
	{
		[SerializeField]
		TableHand[] _tableHands;
		Dictionary<Hand, TableHand> _tableHandsDictionary = new Dictionary<Hand, TableHand>();

		private void Reset()
		{
			_tableHands = GetComponentsInChildren<TableHand>();
		}

		public void ResetAllHandsCards()
		{
			foreach (TableHand hand in _tableHands)
			{
				hand.ResetCards();
			}
		}

		public void Initialize(Hand[] hands)
		{
			for (int i = 0; i < hands.Length; i++)
			{
				_tableHands[i].Initialize(hands[i]);
				_tableHandsDictionary.Add(hands[i], _tableHands[i]);
			}
		}

		public void AddCardToHand(Hand hand, TableCard tableCard)
		{
			if (_tableHandsDictionary.TryGetValue(hand, out TableHand tableHand)) 
			{
				tableHand.AddCard(tableCard);
			}
		}

		public Vector3 GetNextCardPositionInHand(Hand hand, Vector3 spacing)
		{
			_tableHandsDictionary.TryGetValue(hand, out TableHand tableHand);

			Vector3 targetPosition = tableHand.transform.position;

			if (tableHand.TableCards.Count > 0)
			{
				targetPosition += tableHand.TableCards.Count * spacing;
			}

			return targetPosition;
		}

		public void EnableBlinking(Hand hand, bool enableBlink)
		{
			_tableHandsDictionary.TryGetValue(hand, out TableHand tableHand);
			tableHand.EnableBlinking(enableBlink);
		}
	}
}