using UnityEngine;

public class Stage : MonoBehaviour
{
	Camera _thumbnailCamera;
	public Camera ThumbnailCamera
	{
		get => _thumbnailCamera ?? (_thumbnailCamera = GetComponentInChildren<Camera>());
	}

	public RenderTexture GetThumbnailRenderTexture()
	{
		return ThumbnailCamera.targetTexture;
	}
}
