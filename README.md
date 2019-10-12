#CPSC 501 Assignment 1

Refactoring

Locator arrow had:
	- Bad class names
	- A lot of unused code 
	- Duplicate code
	- Lazy class (ModelEventArgs)
	- Long method
	- Inappropriate intimacy

Formal Report:

====================================================================================================================

Changed class names:
	- 2461d34cbff2c7380d7d49dd932947e4c5d07659
	- Item -> LocatableItem 
	- LocatorPath -> ArrowPath
	- Arrow -> ArrowDirectionIndicator

Removed Unused Code (Speculative Generality):
	- b0847929605f82dd00a4785f2ee21a45de3add0e
	- Removed a lot of unused methods within MenuUtility

Can't really test these changes, but I'm sure it works properly!

The class names were super convoluted (even for me), so changing them helped a lot with the overall cohesion of the project.
The unused code was just there from another project (where it was actually being used), but since this project is meant to be a plugin, they weren't needed.



====================================================================================================================

Duplicate Code
	- 8ef6d45cae9b94ef290d5bfc66a28275c89f8355
	- Changed lerping (alpha, as well as fill amount)

Original
```
	public static void LerpFromTransparent<T>(T element, float duration, float delay)
	{
		Color originalColor;

		if (element is Image)
		{
			Image image = element as Image;
			originalColor = new Color(image.color.r, image.color.g, image.color.b, image.color.a);
			image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
			LeanTween.color(image.GetComponent<RectTransform>(), originalColor, duration).setEase(LeanTweenType.easeInOutSine).setDelay(delay);
		}
		else if (element is RawImage)
		{
			RawImage image = element as RawImage;
			originalColor = new Color(image.color.r, image.color.g, image.color.b, image.color.a);
			image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
			LeanTween.color(image.GetComponent<RectTransform>(), originalColor, duration).setEase(LeanTweenType.easeInOutSine).setDelay(delay);
		}
		else if (element is Text)
		{
			Text image = element as Text;
			originalColor = new Color(image.color.r, image.color.g, image.color.b, image.color.a);
			image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
			LeanTween.colorText(image.GetComponent<RectTransform>(), originalColor, duration).setEase(LeanTweenType.easeInOutSine).setDelay(delay);
		}
	}

```
Step 1
```
	// Simplified method
	public static void LerpFromTransparent<T>(T element, float duration, float delay) where T : Graphic
	{
		element.LerpVisibility(true, duration, delay);
	}

	// Simplified method
	public static void LerpToTransparent<T>(T element, float duration, float delay, bool destroy = false) where T : Graphic
	{
		element.LerpVisibility(false, duration, delay, destroy);
	}

	// Extracted method
	public static void LerpVisibility<T>(this T element, bool visible, float duration = 0.5f, float delay = 0f, bool destroyOnComplete = false) where T : Graphic
	{
		element.color = new Color(element.color.r, element.color.g, element.color.b, visible ? 0f : 1f);

		LeanTween.color(element.GetComponent<RectTransform>(),
			new Color(element.color.r, element.color.g, element.color.b, visible ? 1f : 0f), duration).setEase(LeanTweenType.easeInOutSine).setDelay(delay).setDestroyOnComplete(destroyOnComplete);
	}
```
Step 2 
```
	// Simplified method
	[Obsolete("LerpFromTransparent is deprecated, please use LerpVisibility instead")]
	public static void LerpFromTransparent<T>(T element, float duration, float delay) where T : Graphic
	{
		element.LerpVisibility(true, duration, delay);
	}

	// Simplified method
	[Obsolete("LerpToTransparent is deprecated, please use LerpVisibility instead")]
	public static void LerpToTransparent<T>(T element, float duration, float delay, bool destroy = false) where T : Graphic
	{
		element.LerpVisibility(false, duration, delay, destroy);
	}
```
Step 3
Actually change the references to use the updated method (I just marked them as deprecated for now).

This was a big section of duplicated code (and was written poorly), and needed to be changed.
I extracted/ combined methods into one general purpose method (lerpIn & lerpOut -> lerpTo). 
I kind of removed a pseudo-switch statement as well (don't know how I messed that one up in the first place...).

Tested via:
TestLerp
TestColor



====================================================================================================================

Lazy Class (ModelEventArgs, and Stage)
	- 8dd9907318847cf1ace23addad8efc531e7932f1

Removed ModelEventArgs entirely
	- Changed parameters that accepted ModeEventArgs to accept LocatableItem instead
	- Can do the same thing with a reference to LocatableItem (easier too)

Made Stage better
	- Made ThumbnailCamera lazy-get
	- Changed convoluted 'out' method to a more conventional 'get' method

Original
```
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
```
After
```
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
```

ModelEventArgs was a super small class, and essentially copied the data from LocatableItem.
Stage had a strange 'out' parameter (I'm not sure why I used this), and some other minor things to change.



====================================================================================================================

Long method
	- 6fab85cecddd6fb1e34ba84fec2e6d2f3b7fba20

Turned AssignThumbnail into 4 methods (jeez..)

Original
```
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
```
After
```
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

```

As you can see, the original method was far too big. 
I extracted 3 different methods:
	1. Create a copy of the target
		- Don't want to duplicate the object itself (which would be the easier option), only the visual properties
	2. Position and scale the copy in front of the thumbnail camera
	3. Get the PNG byte array of the texture

Tested via:
TestCopy
TestColor
TestItemColor



====================================================================================================================

Inappropriate intimacy (Extract Method/ Preserve Object)
	- 4a30bc5f65a215ac24c8cf0c058b2d6e6689eeea
	- A lot of changes to parameters throughout the solution (from GameObject to LocatableItem)
	- Things that touched ModelPhotoBooth can now get their data directly from LocatableItem.

Original
```
        Texture2D texture = ModelPhotoBooth.Instance.ModelTexture(model);
        Thumbnail.texture = texture;
        Border.color = texture.AverageColor();
```
After
```
        Thumbnail.texture = target.Thumbnail;
        Border.color = target.Color;
```

A lot of the classes queried ModelPhotoBooth for textures/ colors, and that wasn't okay.
Instead of storing each target's thumbnail texture/ color inside a dictionary within ModelPhotoBooth, they are stored within LocatableItem directly.

```
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
```
This change allowed me to delete ModelPhotoBooth's corresponding texture/ color dictionaries.


Tested via:
TestItemColor
