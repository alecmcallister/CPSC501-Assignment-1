using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArrowDirectionIndicator : MonoBehaviour
{
	Image ArrowColor;
	Image ArrowBase;
	ArrowPath Path;

	GameObject Focused;

	float lerpSpeed = 5f;

	void Awake()
	{
		ArrowColor = transform.Find("ArrowColor").GetComponent<Image>();
		ArrowBase = transform.Find("ArrowBase").GetComponent<Image>();
		Path = transform.Find("ArrowPath").GetComponent<ArrowPath>();
	}

	void Update()
	{
		if (Focused == null)
			return;

		Vector3 toFocusedFromArrow = (Focused.transform.position - transform.position).normalized;
		Vector3 flatVector = Vector3.ProjectOnPlane(toFocusedFromArrow, transform.parent.forward).normalized;
		Vector3 cross = Vector3.Cross(transform.up, flatVector).normalized;

		float angle = Vector3.Angle(transform.up, flatVector);
		float dot = Vector3.Dot(cross, transform.parent.forward);

		if (dot < 0)
			angle = -angle;

		Quaternion originalRot = transform.rotation;
		Quaternion newRotation = originalRot * Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = Quaternion.Lerp(originalRot, newRotation, Time.smoothDeltaTime * lerpSpeed);
	}

	public void Init(GameObject focused, float duration)
	{
		Focused = focused;

		ArrowColor.color = ModelPhotoBooth.Instance.ModelColor(focused);

		MenuUtility.LerpFromTransparent(ArrowBase, 1/4f, 0f);
		MenuUtility.LerpFromTransparent(ArrowColor, 1/2f, 0.3f);

		MenuUtility.LerpInFillAmount(ArrowColor, 1/2f, 0f);

		Path.Init(Focused, duration, 0.4f);

		StartCoroutine(End(duration));
	}

	public IEnumerator End(float duration)
	{
		yield return new WaitForSeconds(duration);
		MenuUtility.LerpOutFillAmount(ArrowColor, 1f, 0.1f);
		MenuUtility.LerpToTransparent(ArrowColor, 1/2f, 0f);
		MenuUtility.LerpToTransparent(ArrowBase, 1f, 0f);
	}

}
