using System;

namespace CasinoGames.Blackjack
{
	public class Deal
	{
		public Player Player;
		public Hand Hand;
		public Card Card;

		public bool Flip = true;
		public Arrangement Arrangement = Arrangement.Fanned;

		public Action<Deal> Callback;

		public Deal(Player player, Hand hand, Card card, bool flip, Arrangement arrangement, Action<Deal> callback = null)
		{
			Player = player;
			Hand = hand;
			Card = card;

			Flip = flip;
			Arrangement = arrangement;

			Callback = callback;
		}
	}

	public enum Arrangement
	{
		Stacked,
		Fanned
	}
}
