using System;
using UnityEngine;

namespace CasinoGames.Blackjack
{
	[Serializable]
	public class Card
	{
		[SerializeField]
		Suit _suit;
		public Suit CardSuit => _suit;
		[SerializeField]
		Rank _rank;
		public Rank Rank => _rank;

		public bool IsFlipped { get; set; } = false;
		public string Suit { get; internal set; }

		public Card(Suit suit, Rank rank)
		{
			_suit = suit;
			_rank = rank;
		}

		public override string ToString()
		{
			return _suit.ToString() + _rank.ToString();
		}

		public int GetValue()
		{
			if ((int)Rank <= 10) return (int)Rank;
			if (Rank == Rank.Ace) return 11;
			return 10;
		}
	}

	public enum Suit { Hearts, Diamonds, Clubs, Spades }
	public enum Rank { Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King }

}
