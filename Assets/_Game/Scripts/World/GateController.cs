using UnityEngine;
using TMPro;

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
		vortex.ApplyGateOperation(operation, value);

		gameObject.SetActive(false);
	}
}
