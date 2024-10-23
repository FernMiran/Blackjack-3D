using UnityEngine;
using UnityEngine.UI;

namespace CasinoGames.Blackjack.UI
{
	public class UI_PlayerHandPanel : MonoBehaviour
	{
		[SerializeField]
		Image _iconImage;
		[SerializeField]
		Text _valueText;

		[SerializeField]
		BlackjackVisualTheme _blackjackTableTheme;

		[SerializeField]
		UI_DecisionPanel _decisionPanel;

		public Player Player => _player;
		Player _player;
		public int Hand => _hand;
		int _hand;

		private void Reset()
		{
			_valueText = GetComponentInChildren<Text>(true);
			_decisionPanel = GetComponentInChildren<UI_DecisionPanel>(true);
		}

		public void Initialize(Player player, int hand)
		{
			_player = player;
			_hand = hand;

			_decisionPanel.Initialize(player, hand);

			UpdateValue();
		}

		public void UpdateValue()
		{
			if (_player != null)
			{
				if (!CheckIfHandBusted())
				{
					int newValue = _player.Hands[_hand].GetTotalValue();
					SetValue(newValue);
				}
			}
			else
			{
				Debug.LogError(name + " has Empty player");
			}
		}

		public void UpdateResult(Results result)
		{
			switch (result)
			{
				case Results.Bust:
					SetIcon(_blackjackTableTheme.BustSprite);
					break;
				case Results.Lose:
					SetIcon(_blackjackTableTheme.LoseSprite);
					break;
				case Results.Win:
					SetIcon(_blackjackTableTheme.WinSprite);
					break;
				case Results.Push:
					SetIcon(_blackjackTableTheme.PushSprite);
					break;
				default:
					break;
			}
		}

		public void ResetHand()
		{
			//
		}

		private bool CheckIfHandBusted()
		{
			if (_player.Hands[_hand].IsBusted())
			{
				SetIcon(_blackjackTableTheme.BustSprite);
				return true;
			}

			return false;
		}

		private void SetIcon(Sprite icon)
		{
			_iconImage.gameObject.SetActive(true);
			_iconImage.sprite = icon;

			_valueText.gameObject.SetActive(false);
		}

		private void SetValue(int value)
		{
			_iconImage.gameObject.SetActive(false);

			_valueText.gameObject.SetActive(true);
			_valueText.text = $"{value}";
		}
	}
}
