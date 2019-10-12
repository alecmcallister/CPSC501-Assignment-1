using System;
using System.Collections.Generic;
using UnityEngine;

public class LocatableItem : MonoBehaviour
{
	public static Dictionary<int, List<GameObject>> ObjectInstances = new Dictionary<int, List<GameObject>>();
	public static event Action<ModelEventArgs> ItemEvent = new Action<ModelEventArgs>(e => { });

	public int Object_ID;

	public LocatorArrow ArrowPrefab;

	void Start()
	{
		if (!ObjectInstances.ContainsKey(Object_ID))
			ObjectInstances.Add(Object_ID, new List<GameObject> { gameObject });

		else
			ObjectInstances[Object_ID].Add(gameObject);

		ItemEvent.Invoke(new ModelEventArgs { Name = name, Object_ID = Object_ID, Object_Data = gameObject, EventType = ModelEventArgs.Type.Instantiated });
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			LocatorArrow arrow = Instantiate(ArrowPrefab).GetComponent<LocatorArrow>();
			arrow.Init(gameObject, Camera.main.transform.right * 0.2f * Object_ID);
			arrow.AddArrow(gameObject);
		}
	}

	void OnDestroy()
	{
		ObjectInstances.Remove(Object_ID);
		ItemEvent.Invoke(new ModelEventArgs { Name = name, Object_ID = Object_ID, Object_Data = gameObject, EventType = ModelEventArgs.Type.Deleted });
	}

	public static int GetItemID(GameObject item)
	{
		foreach (int id in ObjectInstances.Keys)
			if (ObjectInstances[id]?.Contains(item) ?? false)
				return id;

		return -1;
	}
}
