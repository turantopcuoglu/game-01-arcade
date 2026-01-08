using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Core
{
	public sealed class GameStateMachine
	{
		public GameStateId CurrentId { get; private set; } = GameStateId.None;
		private IGameState _current;
		private readonly Dictionary<GameStateId, IGameState> _states = new();


		public event Action<GameStateId> OnStateChanged;

		public void Register(IGameState state)
		{
			_states[state.Id] = state;
		}

		public void ChangeState(GameStateId next, bool force = false)
		{
			Debug.Log($"[SM] ChangeState {CurrentId} -> {next} (force={force})");

			if (!force && CurrentId != GameStateId.None && CurrentId == next)
			{
				Debug.Log("[SM] ChangeState ignored: already in target state");
				return;
			}

			if (!_states.TryGetValue(next, out var nextState))
			{
				Debug.LogError($"[SM] Target state not registered: {next}");
				return;
			}

			_current?.Exit();

			_current = nextState;
			CurrentId = next;

			OnStateChanged?.Invoke(CurrentId);

			_current.Enter();
		}




		public void Tick(float dt)
		{
			_current?.Tick(dt);
		}
	}
}
