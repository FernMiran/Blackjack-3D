using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace CasinoGames.Blackjack
{
	public class BlackjackManager : MonoBehaviour
	{
		// Deck
		public Deck Deck => _deck;
		[SerializeField]
		private Deck _deck;

		// Hands
		public Player Dealer => _dealer;
		[SerializeField]
		private Player _dealer;

		public List<Player> Players => _players;
		[SerializeField]
		private List<Player> _players = new List<Player>() { new Player("Dealer") };

		public int PlayersCount => _players.Count;
		[SerializeField]
		private int _startingPlayers = 7;

		[SerializeField]
		private int _playerTurn = 0;
		[SerializeField]
		private bool _everyoneDecided = false;
		[SerializeField]
		private bool _isDealingCard = false; // Change to lambda statement in the DrawCard or DealCard methods.

		Dictionary<Hand, Decision> _playersDecisions = new Dictionary<Hand, Decision>();
		Dictionary<Hand, Results> _handsResults = new Dictionary<Hand, Results>();
		// Events

		// Game
		public Action OnGameStarted; 
		public Action OnGameEnded;

		// Stages
		public Action<Deck> OnDeckCreated;
		public Action<Player[], Player> OnPlayersCreated;
		public Action OnInitialCardsDealed;
		public Action<Dictionary<Hand, Results>> OnHandsResults;

		public Action<Player, Hand> OnPlayerHandTurnStarted;
		public Action<Player, Hand> OnPlayerHandTurnFinished;

		/// <summary>
		/// Event for wait for input. Parameters: Player, Hand Index, Callback when Finished
		/// </summary>
		public Action<Player, int, Action<Player, Decision>> OnWaitForPlayerDecision;
		public Action<Player, Decision> OnPlayerDecided;

		// Dealing
		public Action<Deal> OnCardDealStarted;
		public Action<Deal> OnCardDealFinished;

		// Flipping
		public Action<Flip> OnCardFlipStarted;
		public Action<Flip> OnCardFlipFinished;

		// Reset
		public Action OnBlackjackReset;

		private void Awake()
		{
			OnCardDealFinished += HandleCardDealFinishedCallback;
		}

		private void Start()
		{
			InitializeDeck();
			InitializePlayers();

			OnGameStarted?.Invoke();
		}

		private void OnDestroy()
		{
			OnCardDealFinished -= HandleCardDealFinishedCallback;
		}

		public void InitializeDeck()
		{
			_deck = new Deck(shuffle: true);
			OnDeckCreated?.Invoke(_deck);
		}

		public void ResetGame()
		{
			InitializeDeck();
			InitializePlayers();

			// Reset hands
			foreach (Player player in _players)
			{
				player.ResetHands();
			}

			StopAllCoroutines();

			OnBlackjackReset?.Invoke();
		}

		public void StartGame()
		{
			StartCoroutine(DealInitialCards());
		}

		public void SaveDecision(Player player, int handIndex, Decision decision)
		{
			_playersDecisions[player.Hands[handIndex]] = decision;

			int undecidedCount = CountDefaultValues(_playersDecisions);
			_everyoneDecided = undecidedCount == 0;

			string decisionSaveLog = $"{player.Name} {decision}s.";
			if (!_everyoneDecided)
			{
				decisionSaveLog += $" {undecidedCount} of {_playersDecisions.Count} {default(Decision)}.";
			}
			Debug.Log(decisionSaveLog);
		}

		private void InitializePlayers()
		{
			_players.Clear();

			for (int i = 0; i < _startingPlayers; i++)
			{
				Player newPlayer = new Player("Player " + (i + 1));
				_players.Add(newPlayer);

				_playersDecisions.Add(newPlayer.Hands[0], default);
			}

			_dealer = new Player("Player Dealer", isDealer: true);

			OnPlayersCreated?.Invoke(_players.ToArray(), _dealer);
		}

		private IEnumerator DealInitialCards()
		{
			int cardsCount = 2;

			for (int cardCount = 0; cardCount < cardsCount; cardCount++)
			{
				//Debug.Log("Dealing card # " + cardCount);

				foreach (Player player in _players)
				{
					foreach (Hand hand in player.Hands)
					{
						yield return StartCoroutine(DrawCardForPlayerHand(player, hand));
					}
				}

				bool flip = cardCount >= 1 ? false : true;
				yield return StartCoroutine(DrawCardForPlayerHand(_dealer, _dealer.Hands[0], flip));
			}

			OnInitialCardsDealed?.Invoke();
			StartCoroutine(WaitForPlayerDecision());
		}

		private IEnumerator WaitForPlayerDecision()
		{
			// Player turns
			for (_playerTurn = 0; _playerTurn < _startingPlayers; _playerTurn++)
			{
				Player player = _players[_playerTurn];
				bool isPlayerTurn = true;


				if (player.AreHandsBusted())
				{
					Debug.Log($"{player.Name} ({_playerTurn}) hands are busted, skipping");
					continue;
				}

				// Wait for each hand decision
				for (int handIndex = 0; handIndex < player.Hands.Count; handIndex++)
				{
					Hand hand = player.Hands[handIndex];

					if (hand.IsBusted() || hand.GetTotalValue() == 21)
					{
						continue;
					}

					Decision playerDecision = Decision.Undecided;

					Debug.Log($"{player.Name} hand {handIndex + 1} turn.");
					OnPlayerHandTurnStarted?.Invoke(player, hand);

					if (!_playersDecisions.TryGetValue(hand, out playerDecision))
					{
						Debug.LogError("There was no decision slot for " + player.Name);
						yield return null;
					}

					while (isPlayerTurn)
					{
						// While undecided, wait for decision
						if (_playersDecisions[hand] == Decision.Undecided)
						{
							Action<Player, Decision> onPlayerDecided = (player, decision) =>
							{
								_playersDecisions[hand] = decision;
							};

							OnWaitForPlayerDecision?.Invoke(player, handIndex, onPlayerDecided);

							yield return new WaitUntil(() => _playersDecisions[hand] != Decision.Undecided);

							OnPlayerDecided?.Invoke(player, _playersDecisions[hand]);
						}

						// Execute decision
						switch (_playersDecisions[hand])
						{
							case Decision.Undecided:
								Debug.LogError("Undecided " + player + " passed, this must be an error");
								yield return null;
								break;
							case Decision.Hit:
								//Debug.Log("Waiting to hit " + player.Name + " hand #" + handIndex);
								yield return StartCoroutine(DrawCardForPlayerHand(player, hand));
								//Debug.Log("Finished to hit " + player.Name + " hand #" + handIndex + $"{(player.Hands[handIndex].IsBusted() ? ", is busted." : "")}");

								if (IsStillHandTurn(hand))
								{
									_playersDecisions[hand] = Decision.Undecided;
								}
								else
								{
									isPlayerTurn = false;
									OnPlayerHandTurnFinished?.Invoke(player, hand);
								}

								break;
							case Decision.Stand:
								//Debug.Log(player + " standed");
								isPlayerTurn = false;
								_playersDecisions[hand] = Decision.Undecided;

								OnPlayerHandTurnFinished?.Invoke(player, hand);
								yield return null;
								break;
							// case Decision.Split
							//		break;
							// case Decision.Double
							//		break;
							default:
								Debug.LogError("The decision switch entered it's default state, it must be an error");
								yield return null;
								_playersDecisions[hand] = Decision.Undecided;
								break;
						}
					}
				}
			}

			// Dealer turn
			Hand dealerHand = _dealer.Hands[0];

			// Flip suprise card
			yield return StartCoroutine(FlipPlayerHandCard(_dealer, dealerHand, dealerHand.GetLastCard(), flip:true, Arrangement.Fanned));

			// Hit Dealer Hand until Bust, Win or 17.
			//Debug.Log("Dealer turn");
			while (dealerHand.GetTotalValue() < 17) // Check if it's busted too?
			{
				yield return StartCoroutine(DrawCardForPlayerHand(_dealer, _dealer.Hands[0]));
			}
			//Debug.Log("Finished dealer turn ... ");

			// End turn
			DetermineAllResults();

			OnHandsResults?.Invoke(_handsResults);

			ClearDecisions();
		}

		private bool IsStillHandTurn(Hand hand)
		{
			int totalValue = hand.GetTotalValue();
			bool handIsBusted = hand.IsBusted();

			if (!handIsBusted && totalValue != 21)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private int CountDefaultValues<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
		{
			int defaultValuesCount = 0;

			foreach (KeyValuePair<TKey, TValue> kvp in dictionary)
			{
				if (EqualityComparer<TValue>.Default.Equals(kvp.Value, default))
				{
					defaultValuesCount++;
				}
			}
			return defaultValuesCount;
		}

		private void ClearDecisions()
		{
			Debug.Log("Clearing decisions.");

			foreach (Hand handKey in _playersDecisions.Keys.ToList())
			{
				_playersDecisions[handKey] = default;
			}
		}

		private IEnumerator DrawCardForPlayerHand(Player player, Hand hand, bool flip = true, Arrangement arrangement = Arrangement.Fanned)
		{
			Card drawnCardForPlayer = _deck.DrawCard();
			drawnCardForPlayer.Flip(flip);
			hand.AddCard(drawnCardForPlayer);

			Action<Deal> onDealFinished = (deal) => { 
				_isDealingCard = false;
				OnCardDealFinished?.Invoke(deal);
			};

			Deal deal = new Deal(player, hand, drawnCardForPlayer, flip, arrangement, onDealFinished);
			OnCardDealStarted?.Invoke(deal);

			_isDealingCard = true;

			yield return new WaitUntil(() => !_isDealingCard);
		}

		private void HandleCardDealFinishedCallback(Deal deal)
		{
			_isDealingCard = false;
		}

		private IEnumerator FlipCardInPlayerHand(Player player, Hand hand, Card card, bool flip = true, Arrangement arrangement = Arrangement.Fanned)
		{
			yield return null;
		}

		private IEnumerator FlipPlayerHandCard(Player player, Hand hand, Card card, bool flip = true, Arrangement arrangement = Arrangement.Fanned, Action<Deal> callback = null)
		{
			//Debug.Log("Starting to flip.");

			bool flipping = true;

			Action<Flip> onFlipCardFinished = (flip) =>
			{
				flipping = false;
			};

			Flip flipDeal = new Flip(player, hand, card, flip, onFlipCardFinished);

			OnCardFlipStarted?.Invoke(flipDeal);

			yield return new WaitUntil(() => !flipping);
			//Debug.Log("Card flip.");

			card.Flip(!card.IsFlipped);

			OnCardFlipFinished?.Invoke(flipDeal);
		}

		private void DetermineAllResults()
		{
			for (int playerIndex = 0; playerIndex < _startingPlayers; playerIndex++)
			{
				Player player = _players[playerIndex];

				for (int handIndex = 0; handIndex < player.Hands.Count; handIndex++)
				{
					Hand hand = player.Hands[handIndex];
					Results handResult;

					if (hand.IsBusted())
					{
						handResult = Results.Bust;
					}
					else
					{
						handResult = DetermineHandResult(player, handIndex);
					}
					_handsResults.Add(hand, handResult);
				}
			}
		}

		private Results DetermineHandResult(Player player, int handIndex)
		{
			Hand playerHand = player.Hands[handIndex];
			Hand dealerHand = _dealer.Hands[0];

			int playerValue = playerHand.GetTotalValue();
			int dealerValue = dealerHand.GetTotalValue();

			Results result = default;

			if (dealerHand.IsBusted())
			{
				// Player wins
				result = Results.Win;
			}
			else if (!dealerHand.IsBusted() && playerValue < dealerValue)
			{
				// Player loses
				result = Results.Lose;
			}
			else if (!dealerHand.IsBusted() && playerValue > dealerValue)
			{
				// Player wins
				result = Results.Win;
			}
			else if (playerValue == dealerValue)
			{
				// It's a tie
				result = Results.Push;
			}

			string handPronoun = handIndex == 0 ? "first" : "second";
			Debug.Log($"{player.Name} {handPronoun} hand is a {result}. (Player {playerValue} vs Dealer {dealerValue})");

			return result;
		}
	}


	public class Flip
	{
		public Player Player { get; private set; }
		public Hand Hand { get; private set; }
		public Card Card { get; private set; }

		public bool Flipping { get; private set; } = true;
		public Action<Flip> EndCallback { get; private set; } = null;

		public Flip(Player player, Hand hand, Card card, bool flipping = true, Action<Flip> endCallback = null)
		{
			Player = player;
			Hand = hand;
			Card = card;

			Flipping = flipping;
			EndCallback = endCallback;
		}
	}

}