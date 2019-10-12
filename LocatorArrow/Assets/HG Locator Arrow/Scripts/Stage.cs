using UnityEngine;

public class Stage : MonoBehaviour
{
	public Camera thumbCamera { get; private set; }

	void Awake()
	{
		thumbCamera = GetComponentInChildren<Camera>();
	}

	public void AssignTexture(out RenderTexture renderTexture)
	{
		renderTexture = thumbCamera.targetTexture;
	}
}
