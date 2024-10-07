using System;
using System.Collections.Generic;

namespace CasinoGames.Blackjack
{
	[Serializable]
	public class Player
	{
		public string Name;
		public bool IsDealer { get; private set; } = false;

		public List<Hand> Hands;

		public Player(string name, bool isDealer = false)
		{
			Name = name;
			IsDealer = isDealer;

			Hands = new List<Hand>();

			if (IsDealer)
			{
				Hands.Add(new Hand());
			}
			else
			{
				Hands.Add(new Hand());
				// In case is splitted
				//Hands.Add(new Hand());
			}
		}

		public bool AreHandsBusted()
		{
			for (int i = 0; i < Hands.Count; i++)
			{
				if (!Hands[i].IsBusted())
				{
					return false;
				}
			}
			return true;
		}

		public void ResetHands()
		{
			foreach (Hand hand in Hands)
			{
				hand.ResetHand();
			}
		}
	}
}
