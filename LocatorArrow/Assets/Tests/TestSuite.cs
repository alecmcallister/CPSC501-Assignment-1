using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests
{
	public class TestSuite
	{
		[SetUp]
		public void Setup()
		{
			go = new GameObject();
		}

		[TearDown]
		public void Teardown()
		{
			Object.Destroy(go);
		}

		GameObject go;

		[UnityTest]
		public IEnumerator TestLerp()
		{
			Image image = go.AddComponent<Image>();
			image.color = new Color(0f, 1f, 0f, 0f);
			image.LerpVisibility(true);
			yield return new WaitForSeconds(0.55f);
			Assert.AreEqual(1f, image.color.a, 0.00001f);

			image.LerpVisibility(false);
			yield return new WaitForSeconds(0.55f);
			Assert.AreEqual(0f, image.color.a, 0.00001f);
		}

		[UnityTest]
		public IEnumerator TestCopy()
		{
			go = GameObject.CreatePrimitive(PrimitiveType.Cube);

			GameObject copy = ModelPhotoBooth.Instance.CreateCopyOfGameObject(go);

			Assert.AreEqual(go.GetComponent<MeshRenderer>().material.color, copy.GetComponent<MeshRenderer>().material.color);
			Assert.AreEqual(go.GetComponent<MeshFilter>().mesh.vertices, copy.GetComponent<MeshFilter>().mesh.vertices);

			Object.Destroy(copy);
			yield return null;
		}

		[UnityTest]
		public IEnumerator TestColor()
		{
			yield return null;

			Assert.AreEqual(Color.black, Texture2D.blackTexture.AverageColor());
		}


		[UnityTest]
		public IEnumerator TestItemColor()
		{
			yield return null;

			LocatableItem item = go.AddComponent<LocatableItem>();
			item.Thumbnail = Texture2D.whiteTexture;

			Assert.AreEqual(Color.white, item.Color);
		}
	}
}
