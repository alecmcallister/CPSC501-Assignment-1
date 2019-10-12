using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ModelPhotoBooth : Singleton<ModelPhotoBooth>
{
	static bool busy = false;

	GameObject StagePrefab;

	void Awake()
	{
		LocatableItem.ItemAddedEvent += ItemAddedEvent;

		StagePrefab = Resources.Load<GameObject>("PictureStage");
	}

	public void ItemAddedEvent(LocatableItem item)
	{
		StartCoroutine(AssignThumbnail(item));
	}

	#region Refactored

	// Simplified method
	public IEnumerator AssignThumbnail(LocatableItem item)
	{
		if (!item)
			yield break;

		while (busy)
			yield return null;

		busy = true;

		Stage stage = Instantiate(StagePrefab).GetComponent<Stage>();
		RenderTexture renderTexture = stage.GetThumbnailRenderTexture();

		GameObject copy = CreateCopyOfGameObject(item.gameObject);

		PositionCopyOnStage(copy);

		renderTexture.Create();

		yield return new WaitForEndOfFrame();

		RenderTexture.active = renderTexture;

		Texture2D tex = new Texture2D(2, 2);
		tex.LoadImage(GetTextureBytes(renderTexture));

		item.Thumbnail = tex;

		renderTexture.Release();

		Destroy(stage.gameObject);
		Destroy(copy);

		busy = false;
	}

	// Extracted method
	public GameObject CreateCopyOfGameObject(GameObject copyFrom)
	{
		GameObject copy = new GameObject("Item");

		copy.AddComponent<MeshFilter>().mesh = copyFrom.GetComponent<MeshFilter>().mesh;
		copy.AddComponent<MeshRenderer>().materials = copyFrom.GetComponent<MeshRenderer>().materials;

		return copy;
	}

	// Extracted method
	public void PositionCopyOnStage(GameObject copy)
	{
		MeshRenderer meshRenderer = copy.GetComponent<MeshRenderer>();

		float max = Mathf.Max(meshRenderer.bounds.extents.x, meshRenderer.bounds.extents.y, meshRenderer.bounds.extents.z);

		if (max < 0.5f)
			copy.transform.localScale *= (0.5f / max);

		copy.transform.position = Vector3.zero;
		copy.layer = 8;
	}

	// Extracted method
	public byte[] GetTextureBytes(RenderTexture renderTexture)
	{
		Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height);
		tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
		return tex.EncodeToPNG();
	}

	#endregion
}
