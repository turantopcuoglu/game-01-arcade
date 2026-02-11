using UnityEngine;

/// <summary>
/// Data-Driven Design — ScriptableObject.
/// Each level's configuration lives in an asset file editable in Inspector.
/// No code changes needed to tweak level parameters.
/// </summary>
[CreateAssetMenu(fileName = "Level_01", menuName = "ScrapMagnet/Level Data")]
public class LevelData : ScriptableObject
{
	[Header("Meta")]
	public int levelIndex;
	public string displayName;

	[Header("Level Content")]
	[Tooltip("Prefab containing the entire level layout (road, scraps, obstacles, gates, finish, titan)")]
	public GameObject levelPrefab;

	[Header("Titan")]
	[Tooltip("How many scraps needed to fully build the Titan")]
	public int titanCapacity = 50;

	[Header("Rewards")]
	public int baseCoinReward = 100;

	[Tooltip("Multiplier zones on the Titan ramp: e.g. reaching zone 3 = x3 reward")]
	public float[] multiplierZones = { 1f, 2f, 3f, 5f };
}
