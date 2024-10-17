using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CasinoGames.Blackjack.UI
{
	public class BlackjackTable : MonoBehaviour
	{
		[Header("Blackjack Manager")]
		[SerializeField]
		private BlackjackManager _blackjackManager;

		[Header("Theme Data")]
		[SerializeField]
		private BlackjackVisualTheme _tableTheme;

		[Header("Table")]
		[SerializeField]
		Transform _deckTransform;

		[Header("Instantiation")]
		[SerializeField]
		private TableCard _cardPrefab;
		
		[SerializeField]
		private bool _isHandlingCards = false;
		Dictionary<Card, TableCard> _tableCardsDictionary = new Dictionary<Card, TableCard>();

		[Header("Speed")]
		[SerializeField]
		private float _lerpingDuration = 1f;
		[SerializeField]
		private float _flippingDuration = 1f;

		[Header("Hands")]
		[SerializeField]
		private TableHandsManager _tableHandsManager;
		[SerializeField]
		private TableHandsController _dealerTableHand;
		[SerializeField]
		private Transform _playersTableHandsParent;

		private Dictionary<Player, TableHandsController> _playerPositionsDictionary = new Dictionary<Player, TableHandsController>();
		
		[SerializeField]
		private GameObject _playerPositionPlaceholderPrefab;

		[Header("Spacing")]
		[SerializeField]
		private Vector3 _stackedSpacing = new Vector3(0.01f, 0f, -0.03f);
		public Vector3 StackedSpacing => _stackedSpacing;
		[SerializeField]
		private Vector3 _fannedSpacing = new Vector3(0.1f, 0f, -0.03f);
		public Vector3 FannedSpacing => _fannedSpacing;

		private void Reset()
		{
			_blackjackManager = FindObjectOfType<BlackjackManager>();
			_tableHandsManager = GetComponentInChildren<TableHandsManager>();
		}

		private void Awake()
		{
			SetupEvents();
		}

		private void SetupEvents()
		{
			// Setup
			_blackjackManager.OnPlayersCreated += HandlePlayersCreated;
			_blackjackManager.OnDeckCreated += HandleDeckCreated;

			_blackjackManager.OnPlayerHandTurnStarted += HandlePlayerHandTurnStarted;
			_blackjackManager.OnPlayerHandTurnFinished += HandlePlayerHandTurnFinished;

			// Deals
			_blackjackManager.OnCardDealStarted += HandleCardDealStarted;

			// Flipping
			_blackjackManager.OnCardFlipStarted += HandleCardFlipStarted;

			// Game Stages
			_blackjackManager.OnBlackjackReset += HandleBlackjackReset;
		}

		private void HandlePlayerHandTurnStarted(Player player, Hand hand)
		{
			_tableHandsManager.EnableBlinking(player, hand, true);
		}
		
		private void HandlePlayerHandTurnFinished(Player player, Hand hand)
		{
			_tableHandsManager.EnableBlinking(player, hand, false);
		}

		private void HandleDeckCreated(Deck deck)
		{
			if (_tableCardsDictionary.Count > 0)
			{
				foreach (KeyValuePair<Card, TableCard> item in _tableCardsDictionary)
				{
					Destroy(item.Value.gameObject);
				}

				_tableCardsDictionary.Clear();
			}

			Vector3 instantiatingPosition = _deckTransform.position + Vector3.zero;

			for (int i = 0; i < deck.Cards.Count; i++)
			{
				Card card = deck.Cards[i];

				TableCard cardObject = Instantiate(_cardPrefab, _deckTransform);

				cardObject.Sprite.sprite = _tableTheme.GetCardSprite(card.CardSuit, card.Rank);
				cardObject.transform.name = $"{card.CardSuit} {card.Rank} {cardObject.transform.name}";

				cardObject.transform.position = instantiatingPosition;
				cardObject.transform.rotation = _deckTransform.rotation;

				instantiatingPosition += _stackedSpacing;

				_tableCardsDictionary.Add(card, cardObject);
			}
		}

		private void HandlePlayersCreated(Player[] players, Player dealer)
		{
			TableHandsController[] playerPositions = _playersTableHandsParent.GetComponentsInChildren<TableHandsController>();

			for (int i = 0; i < players.Length; i++)
			{
				if (i > playerPositions.Length)
					break;

				_playerPositionsDictionary.Add(players[i], playerPositions[i]);
			}
		}

		// Deal
		private void HandleCardDealStarted(Deal deal)
		{
			if (!_tableCardsDictionary.TryGetValue(deal.Card, out TableCard tableCard))
				return;

			TableHandsController tableHandsController = _tableHandsManager.GetPlayerTableHandsController(deal.Player);
			
			Vector3 spacing = GetArrangementSpacing(deal.Arrangement);
			Vector3 targetPosition = _tableHandsManager.GetNextCardPosition(deal.Player, deal.Hand, spacing);
			
			//StartCoroutine(DealCard(tableCard, tableHandsController.transform.eulerAngles, targetPosition, deal));
			StartCoroutine(DealCard(tableCard, Vector3.zero, targetPosition, deal));

			tableHandsController.AddCardToHand(deal.Hand, tableCard);
		}

		private IEnumerator DealCard(TableCard card, Vector3 targetAngles, Vector3 targetPosition, Deal deal)
		{
			List<Coroutine> runningCoroutines = new List<Coroutine>();

			if (deal.Flip)
			{
				runningCoroutines.Add(StartCoroutine(card.Rotator.Rotate(new Vector3(targetAngles.x, 180f, targetAngles.z), true, _flippingDuration)));
			}

			runningCoroutines.Add(StartCoroutine(card.Mover.Move(targetPosition, null, _lerpingDuration)));

			// Wait for all coroutines to complete
			foreach (var coroutine in runningCoroutines)
			{
				yield return coroutine;
			}

			deal.Callback?.Invoke(deal);
		}

		// Flip
		private void HandleCardFlipStarted(Card card, Action<Card> callback)
		{
			_tableCardsDictionary.TryGetValue(card, out TableCard cardObject);

			if (cardObject == null)
			{
				return;
			}
			else
			{
				StartCoroutine(FlipCard(cardObject, callback));
			}
		}

		private IEnumerator FlipCard(TableCard card, Action<Card> finishCallback = null)
		{
			Vector3 targetAngle = new Vector3(card.transform.eulerAngles.x, 180f, card.transform.eulerAngles.y);

			bool rotationComplete = false;

			Action rotationFinishedCallback = () =>
			{
				rotationComplete = true;
			};

			yield return StartCoroutine(card.Rotator.Rotate(targetAngle, true, _flippingDuration, rotationFinishedCallback));

			yield return new WaitUntil(() => rotationComplete);

			finishCallback?.Invoke(card.Card);
		}

		private void HandleBlackjackReset()
		{
			foreach (KeyValuePair<Player, TableHandsController> playerTableHand in _playerPositionsDictionary)
			{
				playerTableHand.Value.ResetAllHandsCards();
			}

			_dealerTableHand.ResetAllHandsCards();
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
			public TableCard CardObject;

            public GeneratedCard(UI_CardData cardSprite, TableCard cardObject)
            {
                CardSprite = cardSprite;
				CardObject = cardObject;
            }
        }
	}
}