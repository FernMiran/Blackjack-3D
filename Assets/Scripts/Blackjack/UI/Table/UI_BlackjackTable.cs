using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CasinoGames.Blackjack.UI
{
	public class UI_BlackjackTable : MonoBehaviour
	{
		[Header("Model")]
		[SerializeField]
		private BlackjackManager _blackjackManager;

		[Header("Theme Data")]
		[SerializeField]
		private UI_BlackjackTableTheme _tableTheme;

		private Queue<Deal> _deals = new Queue<Deal>();

		[Header("Table")]
		[SerializeField]
		Transform _deckTransform;

		[Header("Instantiation")]
		[SerializeField]
		private UI_TableCard _cardPrefab;
		
		[SerializeField]
		private bool _isHandlingCards = false;
		Dictionary<Card, UI_TableCard> _card3DObjectsDictionary = new Dictionary<Card, UI_TableCard>();

		[Header("Initial Dealing")]
		[SerializeField]
		private float _lerpingDuration = 1f;
		[SerializeField]
		private float _flippingDuration = 1f;

		[Header("Player Positions")]
		[SerializeField]
		private UI_PlayerTableHand _dealerUIObject;
		[SerializeField]
		private Transform _playersTableHandsParent;
		private Dictionary<Player, UI_PlayerTableHand> _playerPositionsDictionary = new Dictionary<Player, UI_PlayerTableHand>();
		
		[SerializeField]
		private GameObject _playerPositionPlaceholderPrefab;

		[Header("Spacing")]
		[SerializeField]
		private Vector3 _stackedSpacing = new Vector3(0.01f, 0f, -0.03f);
		[SerializeField]
		private Vector3 _fannedSpacing = new Vector3(0.1f, 0f, -0.03f);

		private void Reset()
		{
			_blackjackManager = FindObjectOfType<BlackjackManager>();
		}

		private void Awake()
		{
			// Setup
			_blackjackManager.OnPlayersCreated += HandlePlayersCreated;
            _blackjackManager.OnDeckCreated += HandleDeckCreated;

			// Deals
			_blackjackManager.OnCardDealStarted += HandleCardDealStarted;

			// Flipping
			_blackjackManager.OnCardFlipStarted += HandleCardFlipStarted;

			// Game Stages
			_blackjackManager.OnBlackjackReset += HandleReset;
		}

		// 2D UI
		public void StartGame()
		{
			_blackjackManager.StartGame();
		}

		public void ResetGame()
		{
			_blackjackManager.ResetGame();
		}

		private void HandleDeckCreated(Deck deck)
		{
			if (_card3DObjectsDictionary.Count > 0)
			{
				foreach (KeyValuePair<Card, UI_TableCard> item in _card3DObjectsDictionary)
				{
					Destroy(item.Value.gameObject);
				}

				_card3DObjectsDictionary.Clear();
			}

			Vector3 instantiatingPosition = _deckTransform.position + Vector3.zero;

			for (int i = 0; i < deck.Cards.Count; i++)
			{
				Card card = deck.Cards[i];

				UI_TableCard cardObject = Instantiate(_cardPrefab, _deckTransform);

				cardObject.Sprite.sprite = _tableTheme.GetCardSprite(card.CardSuit, card.Rank);
				cardObject.transform.name = $"{card.CardSuit} {card.Rank} {cardObject.transform.name}";

				cardObject.transform.position = instantiatingPosition;
				cardObject.transform.rotation = _deckTransform.rotation;

				instantiatingPosition += _stackedSpacing;

				_card3DObjectsDictionary.Add(card, cardObject);
			}
		}

		private void HandlePlayersCreated(Player[] players, Player dealer)
		{
			UI_PlayerTableHand[] playerPositions = _playersTableHandsParent.GetComponentsInChildren<UI_PlayerTableHand>();

			for (int i = 0; i < players.Length; i++)
			{
				if (i > playerPositions.Length)
					break;

				_playerPositionsDictionary.Add(players[i], playerPositions[i]);
			}
		}

		void HandleCardFlipStarted(Card card, Action<Card> callback)
		{
			_card3DObjectsDictionary.TryGetValue(card, out UI_TableCard cardObject);

			if (cardObject == null)
			{
				return;
			}
			else
			{
				StartCoroutine(FlipCard(cardObject, callback));
			}
		}

		void HandleCardDealStarted(Deal deal)
		{
			if (!_card3DObjectsDictionary.TryGetValue(deal.Card, out UI_TableCard cardObject))
				return;

			UI_PlayerTableHand playerUIObject = GetPlayerUIObject(deal.Player);

			Vector3 targetPosition = GetNextCardPosition(playerUIObject, deal.Arrangement);
			StartCoroutine(DealCardToPlayer(cardObject, playerUIObject.transform.eulerAngles, targetPosition, deal));

			playerUIObject.UI_Cards.Add(cardObject);
		}

		private IEnumerator DealCardToPlayer(UI_TableCard card, Vector3 targetAngles, Vector3 targetPosition, Deal deal)
		{
			List<Coroutine> runningCoroutines = new List<Coroutine>();

			if (deal.Flip)
			{
				runningCoroutines.Add(StartCoroutine(card.Rotator.Rotate(new Vector3(targetAngles.x, 180f, targetAngles.z))));
			}

			runningCoroutines.Add(StartCoroutine(card.Mover.Move(targetPosition)));

			// Wait for all coroutines to complete
			foreach (var coroutine in runningCoroutines)
			{
				yield return coroutine;
			}

			deal.Callback?.Invoke(deal);
		}

		private IEnumerator FlipCard(UI_TableCard card, Action<Card> finishCallback = null)
		{
			Vector3 targetAngle = new Vector3(card.transform.eulerAngles.x, 180f, card.transform.eulerAngles.y);

			bool rotationComplete = false;

			Action rotationFinishedCallback = () =>
			{
				rotationComplete = true;
			};

			yield return StartCoroutine(card.Rotator.Rotate(targetAngle, true, 1f, rotationFinishedCallback));

			yield return new WaitUntil(() => rotationComplete);

			finishCallback?.Invoke(card.Card);
		}

		private void HandleReset()
		{
			foreach (KeyValuePair<Player, UI_PlayerTableHand> tableHand in _playerPositionsDictionary)
			{
				tableHand.Value.ResetCards();
			}
		}

		private UI_PlayerTableHand GetPlayerUIObject(Player player)
		{
			UI_PlayerTableHand playerUIObject;

			// If player not found in regular players
			if (!_playerPositionsDictionary.TryGetValue(player, out playerUIObject))
			{
				// Check if it's the dealer
				if (player.IsDealer)
				{
					playerUIObject = _dealerUIObject;
				}
				else
				{
					Debug.LogError("Player " + player.Name + " has no UI object to animate a deal.");
				}
			}

			return playerUIObject;
		}

		private Vector3 GetNextCardPosition(UI_PlayerTableHand playerTableHand, Arrangement arrangement)
		{
			List<UI_TableCard> playerAddedUICards = playerTableHand.UI_Cards;

			Vector3 targetPosition = playerTableHand.transform.position;
			Vector3 spacing = GetArrangementSpacing(arrangement);

			if (playerAddedUICards.Count > 0)
			{
				targetPosition += playerAddedUICards.Count * spacing;
			}

			return targetPosition;
		}

		private Vector3 GetArrangementSpacing(Arrangement arrangement)
		{
			Vector3 spacing = Vector3.zero;

			switch (arrangement)
			{
				case Arrangement.Stacked:
					spacing = _stackedSpacing;
					break;
				case Arrangement.Fanned:
					spacing = _fannedSpacing;
					break;
				default:
					break;
			}

			return spacing;
		}

		[SerializeField]
		private class PlayerPosition
		{
			public Player Player;
			public Vector3 Position;

            public PlayerPosition(Player player, Vector3 position)
            {
                Player = player;
				Position = position;
            }
        }

		[Serializable]
		private class GeneratedCard
		{
			public UI_CardData CardSprite;
			public UI_TableCard CardObject;

            public GeneratedCard(UI_CardData cardSprite, UI_TableCard cardObject)
            {
                CardSprite = cardSprite;
				CardObject = cardObject;
            }
        }
	}
}