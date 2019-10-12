using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public static class MenuUtility
{
	public static Color AverageColorFromTexture(Texture2D tex)
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

		return new Color(r / runningTotal, g / runningTotal, b / runningTotal, 1);
	}

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

	public static void LerpToTransparent<T>(T element, float duration, float delay, bool destroy = false)
	{
		Color newColor;

		if (element is Image)
		{
			Image image = element as Image;
			newColor = new Color(image.color.r, image.color.g, image.color.b, 0f);
			LeanTween.color(image.GetComponent<RectTransform>(), newColor, duration).setEase(LeanTweenType.easeInOutSine).setDelay(delay).setDestroyOnComplete(destroy);
		}
		else if (element is RawImage)
		{
			RawImage image = element as RawImage;
			newColor = new Color(image.color.r, image.color.g, image.color.b, 0f);
			LeanTween.color(image.GetComponent<RectTransform>(), newColor, duration).setEase(LeanTweenType.easeInOutSine).setDelay(delay).setDestroyOnComplete(destroy);
		}
		else if (element is Text)
		{
			Text image = element as Text;
			newColor = new Color(image.color.r, image.color.g, image.color.b, 0f);
			LeanTween.colorText(image.GetComponent<RectTransform>(), newColor, duration).setEase(LeanTweenType.easeInOutSine).setDelay(delay).setDestroyOnComplete(destroy);
		}
	}

	public static void LerpInFillAmount<T>(T element, float duration, float delay)
	{
		if (element is Image)
		{
			Image image = element as Image;

			LeanTween.value(image.gameObject, 0, 1, duration).setOnUpdate((float val) =>
			{
				image.fillAmount = val;
			}).setEase(LeanTweenType.easeInOutSine).setDelay(delay);
		}
	}

	public static void LerpOutFillAmount<T>(T element, float duration, float delay)
	{
		if (element is Image)
		{
			Image image = element as Image;

			LeanTween.value(image.gameObject, 1, 0, duration).setOnUpdate((float val) =>
			{
				image.fillAmount = val;
			}).setEase(LeanTweenType.easeInOutSine).setDelay(delay);
		}
	}
}
