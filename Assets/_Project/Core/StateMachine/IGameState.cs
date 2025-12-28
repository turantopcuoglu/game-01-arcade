namespace Project.Core
{
	public interface IGameState
	{
		GameStateId Id { get; }
		void Enter();
		void Exit();
		void Tick(float dt); // only for calling in active state
	}
}