using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SmileController : MonoBehaviour
{

	[SerializeField] private TextMeshProUGUI text;

	public void ToggleSmileVisibility()
	{
		text.gameObject.SetActive(!text.gameObject.activeSelf);
	}

	public void HappySmile()
	{
		text.text = ":)";
		text.color = Color.green;
	}

	public void SadSmile()
	{
		text.text = ":(";
		text.color = Color.red;
	}

}
