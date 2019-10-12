using System;
using System.Collections.Generic;
using UnityEngine;

public class LocatableItem : MonoBehaviour
{
	public static event Action<LocatableItem> ItemAddedEvent = new Action<LocatableItem>(e => { });

	static int itemCount = 0;
	public int Object_ID { get; private set; }

	public LocatorArrow ArrowPrefab; // BAD. USE A MANAGER.

	Texture2D _thumbnail;
	public Texture2D Thumbnail
	{
		get => _thumbnail;
		set
		{
			_thumbnail = value;
			Color = value?.AverageColor() ?? Color.grey;
		}
	}

	public Color Color { get; private set; }

	void Awake()
	{
		Object_ID = itemCount++;
	}

	void Start()
	{
		ItemAddedEvent.Invoke(this);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			LocatorArrow arrow = Instantiate(ArrowPrefab).GetComponent<LocatorArrow>();
			arrow.Init(this, Camera.main.transform.right * 0.2f * Object_ID);
		}
	}
}
