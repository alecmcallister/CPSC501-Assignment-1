using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArrowDirectionIndicator : MonoBehaviour
{
	Image ArrowColor;
	Image ArrowBase;
	ArrowPath Path;

	LocatableItem Target;

	void Awake()
	{
		ArrowColor = transform.Find("ArrowColor").GetComponent<Image>();
		ArrowBase = transform.Find("ArrowBase").GetComponent<Image>();
		Path = transform.Find("ArrowPath").GetComponent<ArrowPath>();
	}

	void Update()
	{
		if (Target == null)
			return;

		UpdateRotation();
	}

	void UpdateRotation()
	{
		Vector3 toFocusedFromArrow = (Target.transform.position - transform.position).normalized;
		Vector3 flatVector = Vector3.ProjectOnPlane(toFocusedFromArrow, transform.parent.forward).normalized;
		Vector3 cross = Vector3.Cross(transform.up, flatVector).normalized;

		float angle = Vector3.Angle(transform.up, flatVector);
		float dot = Vector3.Dot(cross, transform.parent.forward);

		if (dot < 0)
			angle = -angle;

		Quaternion originalRot = transform.rotation;
		Quaternion newRotation = originalRot * Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = Quaternion.Lerp(originalRot, newRotation, Time.smoothDeltaTime * 5f);
	}

	public void Init(LocatableItem target, float duration = 15f)
	{
		Target = target;

		ArrowColor.color = target.Color;

		MenuUtility.LerpFromTransparent(ArrowBase, 1/4f, 0f);
		MenuUtility.LerpFromTransparent(ArrowColor, 1/2f, 0.3f);

		MenuUtility.LerpInFillAmount(ArrowColor, 1/2f, 0f);

		Path.Init(Target, duration, 0.4f);

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
