using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CasinoGames.Blackjack.UI
{
	[CreateAssetMenu(fileName = "New Blackjack UI Data", menuName = "Blackjack/Visuals/Data")]
	public class UI_BlackjackTableTheme : ScriptableObject
	{
		public Font ThemeFont => _font;
		[SerializeField]
		Font _font;

		// Cards
		public List<UI_CardData> CardSprites => _cards;
		[SerializeField]
		List<UI_CardData> _cards;

		public Sprite CardBack => _cardBack;
		[SerializeField]
		public Sprite _cardBack;

		[SerializeField]
		string _cardsSpritesDirectory = "Blackjack/Themes/Default Theme/Cards/"; // Assets => Resources => Blackjack => Sprites
		[SerializeField]
		string _cardsSpriteFilenamePrefix = "card_";

		// Chips
		[SerializeField]
		string _chipsSpritesDirectory = "Blackjack/Themes/Default Theme/Chips"; // Assets => Resources => Blackjack => Sprites
		
		[SerializeField]
		private BettingChip[] _chipsPrefabs; // Populate this array with 1, 5, 25, 100.

		// 2D
		// Icons
		public Sprite BustSprite => _bustSprite;
		[SerializeField]
		Sprite _bustSprite;

		public Sprite WinSprite => _winSprite;
		[SerializeField]
		Sprite _winSprite;

		public Sprite PushSprite => _pushSprite;
		[SerializeField]
		Sprite _pushSprite;

		public Sprite LoseSprite => _loseSprite;
		[SerializeField]
		Sprite _loseSprite;

		private void OnEnable()
		{
			if (_cards.Count != 52)
			{
				RetrieveCardsSprites();
			}

			if (_chipsPrefabs.Length <= 0)
			{
				// Retrieve chips models

				_chipsPrefabs = new BettingChip[]
				{
					new BettingChip { value = 1, prefab = null },
					new BettingChip { value = 5, prefab = null },
					new BettingChip { value = 25, prefab = null },
					new BettingChip { value = 100, prefab = null }
				};
			}
		}

		public Sprite GetCardSprite(Suit suit, Rank rank)
		{
			UI_CardData cardData = _cards.FirstOrDefault(card => card.Suit == suit && card.Rank == rank);
			return cardData.Sprite;
		}

		public BettingChip GetBettingChipByValue(int value)
		{
			return _chipsPrefabs.FirstOrDefault(chip => chip.value == value);
		}

		[ContextMenu("Retrieve Cards Sprites")]
		private void RetrieveCardsSprites()
		{
			_cards.Clear();
			_cards = new List<UI_CardData>();

			// Retrieve card ranks and suits
			foreach (Suit suit in Enum.GetValues(typeof(Suit)))
			{
				foreach (Rank rank in Enum.GetValues(typeof(Rank)))
				{
					string rankString;
					switch (rank)
					{
						case Rank.Ace:
							rankString = "A";
							break;
						case Rank.Jack:
							rankString = "J";
							break;
						case Rank.Queen:
							rankString = "Q";
							break;
						case Rank.King:
							rankString = "K";
							break;
						default:
							rankString = ((int)rank).ToString("D2");
							break;
					}

					string spriteFileName = $"{_cardsSpriteFilenamePrefix}{suit.ToString().ToLower()}_{rankString}";
					Sprite sprite = Resources.Load<Sprite>($"{_cardsSpritesDirectory}{spriteFileName}"); // How to load a sprite from a directory like "Assets/Sprites/{spriteFileName}.png"

					if (sprite != null)
					{
						_cards.Add(new UI_CardData(suit, rank, sprite));
					}
					else
					{
						Debug.LogWarning("Error, the sprite " + spriteFileName + " could not be found.");
					}
				}
			}

			// Retrieve card back
		}
	}

	[Serializable]
	public class UI_CardData
	{
		public Suit Suit;
		public Rank Rank;

		public Sprite Sprite;

		public UI_CardData(Suit suit, Rank rank, Sprite sprite)
		{
			Suit = suit;
			Rank = rank;

			Sprite = sprite;
		}
	}


	[Serializable]
	public class BettingChip
	{
		public string color;
		public int value;
		public GameObject prefab;
	}
}
