using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CasinoGames.Blackjack.UI
{
	public class UI_PlayerHandsController : MonoBehaviour
	{
		Player Player;

		[Header("Hands")]
		[SerializeField]
		List<UI_PlayerHandPanel> _handsPanels = new List<UI_PlayerHandPanel>();
		public List<UI_PlayerHandPanel> HandsPanels  => _handsPanels;

		Dictionary<Hand, UI_PlayerHandPanel> handsPanelsDictionary = new Dictionary<Hand, UI_PlayerHandPanel>();

		[SerializeField]
		UI_PlayerHandPanel _handPanelPrefab;

		private void Reset()
		{
			_handsPanels = GetComponentsInChildren<UI_PlayerHandPanel>().ToList();
		}

		public void Initialize(Player player)
		{
			Player = player;

			for (int handIndex = 0; handIndex < _handsPanels.Count; handIndex++)
			{
				UI_PlayerHandPanel handPanel = _handsPanels[handIndex];

				handPanel.Initialize(Player, handIndex);
				handsPanelsDictionary.Add(player.Hands[handIndex], handPanel);
			}
		}

		public void UpdatePanelsValues()
		{
			for (int i = 0; i < _handsPanels.Count; i++)
			{
				//Debug.Log("Updating value of hand panel " + _handsPanels[i].name);
				_handsPanels[i].UpdateValue();
			}
		}

		public void UpdatePanelsWithResults(Dictionary<Hand, Results> handsResults)
		{
			foreach (KeyValuePair<Hand, Results> result in handsResults)
			{
				if (handsPanelsDictionary.TryGetValue(result.Key, out UI_PlayerHandPanel handPanel))
				{
					handPanel.UpdateResult(result.Value);
				}
			}
		}

		public void Split()
		{
			UI_PlayerHandPanel handPanel = Instantiate(_handPanelPrefab);
			handPanel.Initialize(Player, Player.Hands.Count + 1);

			// Move first (original) hand panel to the left (-X from original position)
			// Move second (clone) hand panel to the right (+X from original position)
		}

		public void ResetHands()
		{
			foreach (KeyValuePair<Hand, UI_PlayerHandPanel> handPanel in handsPanelsDictionary)
			{
				handPanel.Value.ResetHand();
			}
		}
	}
}
