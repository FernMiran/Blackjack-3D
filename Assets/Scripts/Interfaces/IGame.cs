
using System;

namespace CasinoGames
{
	public interface IGame
	{
		public IPlayer[] Players { get; }

		public event Action OnGameStarted;
		public event Action OnGameEnded;

		public void StartGame();
	}
}
