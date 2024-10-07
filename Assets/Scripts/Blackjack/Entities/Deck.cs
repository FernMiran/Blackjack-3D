using CasinoGames.Blackjack.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CasinoGames.Blackjack
{
	[Serializable]
	public class Deck
	{
		public List<Card> Cards => _cards;
		[SerializeField]
		private List<Card> _cards;

		public Deck(bool shuffle)
		{
			InitializeDeck();

			if (shuffle)
				Shuffle();
		}

		private void InitializeDeck()
		{
			_cards = new List<Card>();
			foreach (Suit suit in Enum.GetValues(typeof(Suit)))
			{
				foreach (Rank rank in Enum.GetValues(typeof(Rank)))
				{
					_cards.Add(new Card(suit, rank));
				}
			}
		}

		public void Shuffle()
		{
			System.Random rng = new System.Random();
			int n = _cards.Count;
			while (n > 1)
			{
				n--;

				// Get random card
				int randomIndex = rng.Next(n + 1);
				Card randomCard = _cards[randomIndex];

				// Swap positions
				_cards[randomIndex] = _cards[n];
				_cards[n] = randomCard;
			}
		}

		public Card DrawCard(bool fromTop = true)
		{
			if (_cards.Count == 0)
			{
				InitializeDeck();
				Shuffle();
			}

			int drawnCardIndex = fromTop ? _cards.Count - 1 : 0;

			Card drawnCard = _cards[drawnCardIndex];
			_cards.RemoveAt(drawnCardIndex);
			return drawnCard;
		}
	}
}