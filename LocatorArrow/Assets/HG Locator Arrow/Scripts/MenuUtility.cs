using System;
using UnityEngine;
using UnityEngine.UI;

public static class MenuUtility
{
	#region Refactored

	public static Color AverageColor(this Texture2D tex)
	{
		Color[] texColors = tex.GetPixels();

		int total = texColors.Length;
		int runningTotal = 0;

		float r = 0;
		float g = 0;
		float b = 0;

		float rMax = 0;
		float gMax = 0;
		float bMax = 0;

		float combinedMax = 0;
		float combinedMin = 0;

		for (int i = 0; i < total; i++)
		{
			rMax = texColors[i].r;
			gMax = texColors[i].g;
			bMax = texColors[i].b;

			combinedMax = Mathf.Max(rMax, gMax, bMax);
			combinedMin = Mathf.Min(rMax, gMax, bMax);

			if (combinedMax > 0.80f && combinedMin > 0.05f)
			{
				r += rMax;
				g += gMax;
				b += bMax;

				runningTotal++;
			}
		}

		runningTotal = Mathf.Max(1, runningTotal);

		return new Color(r / runningTotal, g / runningTotal, b / runningTotal, 1);
	}

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

	// Extracted method
	public static void LerpVisibility<T>(this T element, bool visible, float duration = 0.5f, float delay = 0f, bool destroyOnComplete = false) where T : Graphic
	{
		element.color = new Color(element.color.r, element.color.g, element.color.b, visible ? 0f : 1f);

		LeanTween.color(element.GetComponent<RectTransform>(),
			new Color(element.color.r, element.color.g, element.color.b, visible ? 1f : 0f), duration).setEase(LeanTweenType.easeInOutSine).setDelay(delay).setDestroyOnComplete(destroyOnComplete);
	}

	// Simplified method
	[Obsolete("LerpInFillAmount is deprecated, please use LerpFillVisibility instead")]
	public static void LerpInFillAmount<T>(T element, float duration, float delay) where T : Image
	{
		element.LerpFillVisibility(true, duration, delay);
	}

	// Simplified method
	[Obsolete("LerpOutFillAmount is deprecated, please use LerpFillVisibility instead")]
	public static void LerpOutFillAmount<T>(T element, float duration, float delay) where T : Image
	{
		element.LerpFillVisibility(false, duration, delay);
	}

	// Extracted method
	public static void LerpFillVisibility<T>(this T element, bool visible, float duration = 0.5f, float delay = 0f) where T : Image
	{
		LeanTween.value(element.gameObject, visible ? 0f : 1f, visible ? 1f : 0f, duration).setOnUpdate((float val) =>
		{
			element.fillAmount = val;
		}).setEase(LeanTweenType.easeInOutSine).setDelay(delay);
	}

	#endregion
}
