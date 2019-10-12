using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ModelEventArgs : EventArgs
{
	public int Object_ID;
	public string Name;
	public GameObject Object_Data;

	public enum Type
	{
		Instantiated,
		Deleted
	}
	public Type EventType;
}

