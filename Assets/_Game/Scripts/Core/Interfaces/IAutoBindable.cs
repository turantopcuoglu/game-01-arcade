using UnityEngine;

/// <summary>
/// Implement on any MonoBehaviour inside a level prefab that needs
/// references to objects outside the prefab (scene-level cameras, managers, etc.).
///
/// LevelManager calls AutoBind on every IAutoBindable in the newly
/// instantiated level prefab right after Instantiate.
/// Each component resolves its own external dependencies here.
/// </summary>
public interface IAutoBindable
{
	/// <param name="levelRoot">Root GameObject of the instantiated level prefab.</param>
	void AutoBind(GameObject levelRoot);
}
