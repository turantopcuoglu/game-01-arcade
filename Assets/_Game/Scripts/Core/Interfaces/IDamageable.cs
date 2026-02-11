/// <summary>
/// Interface Segregation — any object that can receive damage.
/// ObstacleBase implements this. Future damageable objects (boss, barrel, etc.)
/// can implement it without inheriting from ObstacleBase.
/// </summary>
public interface IDamageable
{
	int CurrentHP { get; }
	int MaxHP { get; }
	bool IsAlive { get; }

	void TakeDamage(int amount);
}
