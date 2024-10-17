using UnityEngine;

namespace CasinoGames.Blackjack.UI
{
	public class TableCard : MonoBehaviour
	{
		public Card Card { get; private set; }

		public SpriteRenderer Sprite => _sprite;
		[SerializeField]
		private SpriteRenderer _sprite;

		public RotateBehaviour Rotator => _rotator;
		[SerializeField]
		private RotateBehaviour _rotator;

		public MoveBehavior Mover => _mover;
		[SerializeField]
		private MoveBehavior _mover;

		private void Reset()
		{
			_rotator = GetComponent<RotateBehaviour>();
			_mover = GetComponent<MoveBehavior>();
		}

		public void SetCard(Card card)
		{
			Card = card;
		}
	}
}
