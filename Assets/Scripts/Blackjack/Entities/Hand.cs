using System.Collections.Generic;
using UnityEngine;

namespace CasinoGames.Blackjack
{
	[System.Serializable]
	public class Hand
	{
		public List<Card> Cards => _cards;
		[SerializeField]
		private List<Card> _cards;

        public Hand()
        {
            _cards = new List<Card>();
        }

        public void AddCard(Card card)
		{
			_cards.Add(card);
		}

		public int GetTotalValue()
		{
			int total = 0;
			int aceCount = 0;

			foreach (Card card in _cards)
			{
				if (card.Rank == Rank.Ace)
				{
					aceCount++;
				}
				total += card.GetValue();
			}

			while (total > 21 && aceCount > 0)
			{
				total -= 10;
				aceCount--;
			}

			return total;
		}

		public bool IsBusted()
		{
			return GetTotalValue() > 21;
		}

		public Card GetLastCard()
		{
			return _cards[_cards.Count - 1];
		}

		public void ResetHand()
		{
			_cards.Clear();
		}
	}
}