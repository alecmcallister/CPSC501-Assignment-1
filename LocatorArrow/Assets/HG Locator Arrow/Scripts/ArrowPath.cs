using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Not going to attempt to refactor this...
public class ArrowPath : MonoBehaviour
{
	public GameObject ParticlePrefab;

	LocatableItem Target;

	Vector3[] pointList;
	int steps = 10;
	float[] stepList;

	void Awake()
	{
		pointList = new Vector3[steps];
		stepList = new float[steps];

		float dt = 1 / (float)steps;
		float t = 0;

		for (int i = 0; i < steps; i++)
		{
			stepList[i] = t;
			t += dt;
		}
	}

	public void Init(LocatableItem target, float duration, float delay)
	{
		Target = target;

		CreatePathToObject(Target.gameObject, duration, delay);
	}

	void Update()
	{
		if (Target != null)
		{
			UpdateBezierPathStart(transform.position, transform.up);
			UpdateBezierPathEnd(Target.transform.position);
		}
	}

	Vector3 P0;
	Vector3 P1;
	Vector3 P2;
	Vector3 P3;
	LTBezier bezierPath;
	Vector3 previousPathForward;

	float P1Magnitude = 1f;
	float P2Magnitude = 0.25f;

	public void CreatePathToObject(GameObject to, float duration, float delay)
	{
		pointList = new Vector3[steps];

		Vector3 start = transform.position;
		Vector3 end = to.transform.position;
		Vector3 forward = transform.up;
		previousPathForward = forward;

		P0 = start;
		P1 = P0 + (forward * P1Magnitude);
		P3 = end;
		P2 = P3 + ((P0 - P3).normalized * P2Magnitude);

		bezierPath = new LTBezier(P0, P1, P2, P3, 1f);

		for (int i = 0; i < steps; i++)
			pointList[i] = bezierPath.point(stepList[i]);

		StartCoroutine(ParticlePath(duration, delay));
	}

	IEnumerator ParticlePath(float duration, float delay)
	{
		Color c = Target.Color;
		Color originalColor = new Color(c.r, c.g, c.b, c.a);
		Color originalNoAlpha = new Color(c.r, c.g, c.b, 0f);
		Vector3 originalScale = Vector3.one * 0.5f * 0.25f;
		Vector3 startScale = originalScale * 0.25f;

		yield return new WaitForSeconds(delay);

		for (int i = 0; i < 30; i++)
		{
			GameObject arrow = Instantiate(ParticlePrefab, transform.position, transform.rotation);

			RectTransform rect = arrow.transform.Find("Canvas/Arrow").GetComponent<RectTransform>();
			Image image = arrow.transform.Find("Canvas/Arrow").GetComponent<Image>();

			image.color = originalNoAlpha;
			LeanTween.color(rect, originalColor, 0.3f).setEase(LeanTweenType.easeInOutSine);

			arrow.transform.localScale = startScale;
			LeanTween.scale(arrow, originalScale, 2f).setEase(LeanTweenType.easeInSine);

			arrow.transform.LookAt(transform.position + transform.up);
			LeanTween.value(arrow, 0f, 1f, 5f).setOnUpdate((float val) =>
			{
				Vector3 pos = bezierPath.point(val);
				Vector3 next = bezierPath.point(val + 0.05f);
				arrow.transform.position = pos;
				arrow.transform.LookAt(next);

			}).setOnComplete(() =>
			{
				Destroy(arrow);

			}).setEase(LeanTweenType.easeInSine);

			yield return new WaitForSeconds(duration / 30f);
		}
	}

	public void UpdateBezierPathStart(Vector3 newPosition, Vector3 newForward)
	{
		if (newPosition == P0 && newForward == previousPathForward)
			return;

		P0 = newPosition;
		P1 = P0 + (newForward * P1Magnitude);

		bezierPath = new LTBezier(P0, P1, P2, P3, 1f);

		for (int i = 0; i < steps; i++)
			pointList[i] = bezierPath.point(stepList[i]);

		previousPathForward = newForward;
	}

	public void UpdateBezierPathEnd(Vector3 newPosition)
	{
		if (newPosition == P3)
			return;

		P3 = newPosition;
		P2 = P3 + ((P0 - P3).normalized * P2Magnitude);

		bezierPath = new LTBezier(P0, P1, P2, P3, 1f);

		for (int i = 0; i < steps; i++)
			pointList[i] = bezierPath.point(stepList[i]);
	}
}
