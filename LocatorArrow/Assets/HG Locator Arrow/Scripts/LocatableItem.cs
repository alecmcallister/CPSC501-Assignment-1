using System;
using System.Collections.Generic;
using UnityEngine;

public class LocatableItem : MonoBehaviour
{
	public static Dictionary<int, List<GameObject>> ObjectInstances = new Dictionary<int, List<GameObject>>();
	public static event Action<LocatableItem> ItemAddedEvent = new Action<LocatableItem>(e => { });

	public int Object_ID;

	public LocatorArrow ArrowPrefab; // BAD. USE A MANAGER.

	void Start()
	{
		if (!ObjectInstances.ContainsKey(Object_ID))
			ObjectInstances.Add(Object_ID, new List<GameObject> { gameObject });

		else
			ObjectInstances[Object_ID].Add(gameObject);

		ItemAddedEvent.Invoke(this);
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
	}

	public static int GetItemID(GameObject item)
	{
		foreach (int id in ObjectInstances.Keys)
			if (ObjectInstances[id]?.Contains(item) ?? false)
				return id;

		return -1;
	}
}
