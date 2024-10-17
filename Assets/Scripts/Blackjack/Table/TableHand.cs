using System.Collections.Generic;
using UnityEngine;

namespace CasinoGames.Blackjack.UI
{
	public class TableHand : MonoBehaviour
	{
		public List<TableCard> TableCards { get; private set; } = new List<TableCard>();

		private Hand _hand;

		[SerializeField]
		GameObject _squareMarker;

		[SerializeField]
		Animator _squareMarkerAnimator;

		private void Reset()
		{
			_squareMarkerAnimator = GetComponentInChildren<Animator>();
		}

		public void Initialize(Hand hand)
		{
			_hand = hand;
		}

		public void AddCard(TableCard tableCard)
		{
			TableCards.Add(tableCard);
		}

		public void ResetCards()
		{
			TableCards.Clear();
		}

		public void EnableBlinking(bool enableBlink)
		{
			// Set Animator Flag to "enableBlink"
			_squareMarker.SetActive(enableBlink);
			_squareMarkerAnimator.SetBool("Blink", enableBlink);
		}
	}
}