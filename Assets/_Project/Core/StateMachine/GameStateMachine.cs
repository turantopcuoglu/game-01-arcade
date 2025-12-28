using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Core
{
	public sealed class GameStateMachine
	{
		private readonly Dictionary<GameStateId, IGameState> _states = new();
		private IGameState _current;

		public GameStateId CurrentId => _current?.Id ?? GameStateId.Boot;

		public event Action<GameStateId> OnStateChanged;

		public void Register(IGameState state)
		{
			_states[state.Id] = state;
		}

		public void ChangeState(GameStateId id)
		{
			if (_current != null && _current.Id == id) return;

			_current?.Exit();

			if (!_states.TryGetValue(id, out var next))
				throw new InvalidOperationException($"State not registered: {id}");

			_current = next;
			_current.Enter();

			OnStateChanged?.Invoke(id);
		}

		public void Tick(float dt)
		{
			_current?.Tick(dt);
		}
	}
}
