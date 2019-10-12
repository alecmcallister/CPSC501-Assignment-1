using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ModelPhotoBooth : Singleton<ModelPhotoBooth>
{
	public Dictionary<int, Color> ModelColors = new Dictionary<int, Color>();
	public Dictionary<int, Texture2D> ModelTextures = new Dictionary<int, Texture2D>();
	static bool busy = false;

	GameObject StagePrefab;

	void Awake()
	{
		LocatableItem.ItemAddedEvent += ItemAddedEvent;

		StagePrefab = Resources.Load<GameObject>("PictureStage");
	}

	public void ItemAddedEvent(LocatableItem item)
	{
		int id = item.Object_ID;

		if (!ModelColors.ContainsKey(id))
		{
			ModelColors.Add(id, Color.black);
			ModelTextures.Add(id, null);
			StartCoroutine(AssignThumbnail(item));
		}
	}

	public Texture2D ModelTexture(GameObject item)
	{
		int id = LocatableItem.GetItemID(item);
		return (id > -1) ? ModelTextures[id] : null;
	}

	public Color ModelColor(GameObject item)
	{
		int id = LocatableItem.GetItemID(item);
		return (id > -1) ? ModelColors[id] : Color.black;
	}

	
	// Simplified method
	IEnumerator AssignThumbnail(LocatableItem item)
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
		ModelTextures[item.Object_ID] = tex;
		ModelColors[item.Object_ID] = tex.AverageColor();

		renderTexture.Release();

		Destroy(stage.gameObject);
		Destroy(copy);

		busy = false;
	}

	#region Refactored

	// Extracted method
	GameObject CreateCopyOfGameObject(GameObject copyFrom)
	{
		GameObject copy = new GameObject("Item");

		copy.AddComponent<MeshFilter>().mesh = copyFrom.GetComponent<MeshFilter>().mesh;
		copy.AddComponent<MeshRenderer>().materials = copyFrom.GetComponent<MeshRenderer>().materials;

		return copy;
	}

	// Extracted method
	void PositionCopyOnStage(GameObject copy)
	{
		MeshRenderer meshRenderer = copy.GetComponent<MeshRenderer>();

		float max = Mathf.Max(meshRenderer.bounds.extents.x, meshRenderer.bounds.extents.y, meshRenderer.bounds.extents.z);

		if (max < 0.5f)
			copy.transform.localScale *= (0.5f / max);

		copy.transform.position = Vector3.zero;
		copy.layer = 8;
	}

	// Extracted method
	byte[] GetTextureBytes(RenderTexture renderTexture)
	{
		Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height);
		tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
		return tex.EncodeToPNG();
	}

	#endregion
}
