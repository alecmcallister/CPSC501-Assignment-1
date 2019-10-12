using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LocatorArrow : MonoBehaviour
{
	GameObject DirectionPrefab;
	GameObject ArrowPanel;

	RawImage Thumbnail;
	Image Border;
	Image OuterBorder;
	Image Background;

	public LocatableItem Target { get; private set; }
	const float LocatorArrowDuration = 15f;

	void Awake()
	{
		Thumbnail = transform.Find("LocatorPanel/ThumbnailMask/Thumbnail").GetComponent<RawImage>();
		Border = transform.Find("LocatorPanel/Border").GetComponent<Image>();
		OuterBorder = transform.Find("LocatorPanel/OuterBorder").GetComponent<Image>();
		Background = transform.Find("LocatorPanel/Background").GetComponent<Image>();
		ArrowPanel = transform.Find("ArrowPanel").gameObject;

		DirectionPrefab = Resources.Load<GameObject>("ArrowDirectionIndicator");
	}

	void Update()
	{
		transform.LookAt(Camera.main.transform);
	}

	public void Init(LocatableItem target, Vector3 offset)
	{
		Target = target;

		transform.position += offset;

		Thumbnail.texture = target.Thumbnail;
		Border.color = target.Color;

		MenuUtility.LerpFromTransparent(Background, 0.5f, 0f);
		MenuUtility.LerpFromTransparent(Thumbnail, 0.5f, 0.2f);
		MenuUtility.LerpFromTransparent(OuterBorder, 0.125f, 0f);
		MenuUtility.LerpFromTransparent(Border, 0.125f, 0.2f);
		MenuUtility.LerpOutFillAmount(OuterBorder, 0.333f, 0.2f);

		StartCoroutine(End());

		AddDirectionIndicator(target);
	}

	public void AddDirectionIndicator(LocatableItem target)
	{
		GameObject direction = Instantiate(DirectionPrefab);
		direction.transform.SetParent(ArrowPanel.transform, false);
		direction.GetComponent<ArrowDirectionIndicator>().Init(target, LocatorArrowDuration);
	}

	public IEnumerator End()
	{
		yield return new WaitForSeconds(LocatorArrowDuration);

		MenuUtility.LerpInFillAmount(OuterBorder, 1 / 3f, 0f);
		MenuUtility.LerpToTransparent(Thumbnail, 1 / 2f, 0.4f);
		MenuUtility.LerpToTransparent(Border, 1 / 32f, 0.5f);
		MenuUtility.LerpToTransparent(OuterBorder, 1 / 8f, 0.5f);
		MenuUtility.LerpToTransparent(Background, 1 / 8f, 0.6f);

		while (LeanTween.isTweening(Background.gameObject))
			yield return null;

		yield return new WaitForEndOfFrame();

		Destroy(gameObject);
	}
}
