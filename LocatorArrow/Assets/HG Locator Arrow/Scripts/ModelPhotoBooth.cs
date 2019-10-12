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

	IEnumerator AssignThumbnail(LocatableItem item)
	{
		if (item == null)
			yield break;

		while (busy)
			yield return null;

		busy = true;

		string name = item.name;
		int id = item.Object_ID;

		Stage stage = Instantiate(StagePrefab).GetComponent<Stage>();
		RenderTexture renderTexture = stage.GetThumbnailRenderTexture();
		//stage.AssignTexture(out renderTexture);

		GameObject temp = new GameObject("Item");
		MeshFilter meshFilter = temp.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = temp.AddComponent<MeshRenderer>();

		meshFilter.mesh = item.GetComponent<MeshFilter>().mesh;
		meshRenderer.materials = item.GetComponent<MeshRenderer>().materials;

		float max = Mathf.Max(meshRenderer.bounds.extents.x, meshRenderer.bounds.extents.y, meshRenderer.bounds.extents.z);

		if (max < 0.5f)
		{
			float ratio = 0.5f / max;
			temp.transform.localScale *= ratio;
		}

		temp.layer = stage.gameObject.layer;
		temp.transform.position = Vector3.zero;

		renderTexture.Create();
		yield return new WaitForEndOfFrame();
		RenderTexture currentActiveRT = RenderTexture.active;
		RenderTexture.active = renderTexture;

		Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height);
		tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
		var bytes = tex.EncodeToPNG();

		Texture2D tex2 = new Texture2D(2, 2);
		tex2.LoadImage(bytes);
		ModelTextures[id] = tex2;
		ModelColors[id] = tex2.AverageColor();

		RenderTexture.active = currentActiveRT;
		renderTexture.Release();

		Destroy(stage.gameObject);
		Destroy(temp);

		busy = false;
	}
}
