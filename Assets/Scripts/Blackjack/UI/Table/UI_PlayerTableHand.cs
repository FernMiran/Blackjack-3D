using System.Collections.Generic;
using UnityEngine;

namespace CasinoGames.Blackjack.UI
{
	public class UI_PlayerTableHand : MonoBehaviour
	{
		public List<UI_TableCard> UI_Cards { get; private set; } = new List<UI_TableCard>();

		public void ResetCards()
		{
			UI_Cards.Clear();
		}
	}
}