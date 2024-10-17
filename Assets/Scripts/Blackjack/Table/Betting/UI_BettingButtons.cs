using CasinoGames.Blackjack.UI;
using UnityEngine;

namespace CasinoGames.Blackjack
{
	public class UI_BettingButtons : MonoBehaviour
	{
		[SerializeField]
		BlackjackVisualTheme _blackjackTableTheme;

		private void Reset()
		{

		}

		public void Bet(int value)
		{
			BettingChip bettingChip = _blackjackTableTheme.GetBettingChipByValue(value);

		}
	}

}
