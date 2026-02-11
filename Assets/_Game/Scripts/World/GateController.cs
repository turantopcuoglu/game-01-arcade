using UnityEngine;
using TMPro;

/// <summary>
/// Interactive gate that modifies the player's scrap count.
/// Two gates are placed side by side — player chooses one.
///
/// Positive gates (Add, Multiply) = green.
/// Negative gates (Subtract, Divide) = red.
///
/// Event firing moved to VortexManager.ApplyGateOperation()
/// so GateController stays focused on trigger detection + visuals.
/// </summary>
public class GateController : MonoBehaviour
{
	[Header("Gate Settings")]
	[SerializeField] private GateOperation operation = GateOperation.Add;
	[SerializeField] private int value = 10;

	[Header("Visuals")]
	[SerializeField] private TMP_Text labelText;
	[SerializeField] private MeshRenderer meshRenderer;
	[SerializeField] private Color positiveColor = new Color(0.2f, 0.8f, 0.4f);
	[SerializeField] private Color negativeColor = new Color(0.9f, 0.2f, 0.2f);

	private bool _used;

	private void Start()
	{
		UpdateVisual();
	}

	private void UpdateVisual()
	{
		bool isPositive = operation == GateOperation.Add || operation == GateOperation.Multiply;

		if (labelText != null)
		{
			string symbol = operation switch
			{
				GateOperation.Add => $"+{value}",
				GateOperation.Subtract => $"-{value}",
				GateOperation.Multiply => $"x{value}",
				GateOperation.Divide => $"/{value}",
				_ => ""
			};
			labelText.text = symbol;
		}

		if (meshRenderer != null)
		{
			var block = new MaterialPropertyBlock();
			block.SetColor("_BaseColor", isPositive ? positiveColor : negativeColor);
			meshRenderer.SetPropertyBlock(block);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_used) return;

		var vortex = other.GetComponent<VortexManager>();
		if (vortex == null) vortex = other.GetComponentInChildren<VortexManager>();
		if (vortex == null) return;

		_used = true;

		// ApplyGateOperation handles both logic and event firing
		vortex.ApplyGateOperation(operation, value);

		gameObject.SetActive(false);
	}
}
