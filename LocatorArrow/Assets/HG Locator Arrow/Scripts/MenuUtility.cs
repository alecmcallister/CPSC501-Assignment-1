using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public static class MenuUtility
{
	public static bool isAlphaNumeric(string strToCheck)
	{
		Regex rg = new Regex(@"^[a-zA-Z0-9\s,]*$");
		return (rg.IsMatch(strToCheck) || strToCheck == " " || strToCheck == "*" || strToCheck == "," || strToCheck == "." || strToCheck == "#") && (strToCheck.Length == 1);
	}

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

	public static Color ChangeColorBrightness(Color color, float correctionFactor)
	{
		float red = color.r;
		float green = color.g;
		float blue = color.b;

		if (correctionFactor < 0)
		{
			correctionFactor = 1 + correctionFactor;
			red *= correctionFactor;
			green *= correctionFactor;
			blue *= correctionFactor;
		}
		else
		{
			red = (1 - red) * correctionFactor + red;
			green = (1 - green) * correctionFactor + green;
			blue = (1 - blue) * correctionFactor + blue;
		}

		return new Color(red, green, blue, color.a);
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

	public static void LerpFillAmount(RectTransform rect, float from, float to, float duration, float delay)
	{
		Image image = rect.GetComponent<Image>();
		if (image == null)
			return;

		LeanTween.value(image.gameObject, from, to, duration).setOnUpdate((float val) =>
		{
			image.fillAmount = val;
		}).setEase(LeanTweenType.easeInOutSine).setDelay(delay);
	}

	public static void LerpToPosition(Transform t, Vector3 newPosition, float duration)
	{
		LeanTween.move(t.gameObject, newPosition, duration).setEase(LeanTweenType.easeInOutSine);
	}

	public static void LerpAllFromTransparent(GameObject parent, float duration, float delay)
	{
		foreach (RectTransform rect in parent.GetComponentsInChildren<RectTransform>())
		{
			Image image = rect.GetComponent<Image>();
			if (image != null)
				LerpFromTransparent(image, duration, delay);

			RawImage rawImage = rect.GetComponent<RawImage>();
			if (rawImage != null)
				LerpFromTransparent(rawImage, duration, delay);

			Text text = rect.GetComponent<Text>();
			if (text != null)
				LerpFromTransparent(text, duration, delay);
		}
	}

	public static void LerpAllToTransparent(GameObject parent, float duration, float delay, bool destroyOnComplete)
	{
		foreach (RectTransform rect in parent.GetComponentsInChildren<RectTransform>())
		{
			Image image = rect.GetComponent<Image>();
			if (image != null)
				LerpToTransparent(image, duration, delay, destroyOnComplete);

			RawImage rawImage = rect.GetComponent<RawImage>();
			if (rawImage != null)
				LerpToTransparent(rawImage, duration, delay, destroyOnComplete);

			Text text = rect.GetComponent<Text>();
			if (text != null)
				LerpToTransparent(text, duration, delay, destroyOnComplete);
		}
	}

	public static bool IsTweening(GameObject model)
	{
		foreach (Transform t in model.GetComponentsInChildren<Transform>())
			if (LeanTween.isTweening(t.gameObject))
				return true;

		foreach (Transform t in model.GetComponentsInParent<Transform>())
			if (LeanTween.isTweening(t.gameObject))
				return true;

		return false;
	}

	public static void StopTweening(GameObject model)
	{
		foreach (Transform t in model.GetComponentsInChildren<Transform>())
			if (LeanTween.isTweening(t.gameObject))
				LeanTween.cancel(t.gameObject);
	}

	public static void Blur(GameObject BlurPane, bool blur)
	{
		Renderer r = BlurPane.GetComponent<Renderer>();
		MeshCollider c = BlurPane.GetComponent<MeshCollider>();

		if ((r.enabled && blur) || (!r.enabled && !blur))
			return;

		float from = (blur) ? 0f : 3.5f;
		float to = (blur) ? 3.5f : 0f;

		LeanTween.value(BlurPane, from, to, 0.5f).setOnStart(() =>
		{
			r.enabled = true;
			c.enabled = true;
		}).setOnUpdate((float val) =>
		{
			r.material.SetFloat("_blurSizeXY", val);
		}).setOnComplete(() =>
		{
			r.enabled = blur;
			c.enabled = blur;
		}).setEase(LeanTweenType.easeInOutSine);
	}
}
