using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonVR : MonoBehaviour
{
	public GameObject button;
	public UnityEvent onPress;
	public UnityEvent onRelease;
	GameObject presser;
	AudioSource sound;
	bool isPressed;

	private Vector3 startPos;

    void Start()
    {
		startPos = button.transform.localPosition;
		sound = GetComponent<AudioSource>();
		isPressed = false;
    }

	private void OnTriggerEnter(Collider other)
	{
		if (!isPressed)
		{
			button.transform.localPosition = new Vector3(0, 0.02f, 0);
			presser = other.gameObject;
			onPress.Invoke();
			sound.Play();
			isPressed = true;
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == presser)
		{
			button.transform.localPosition = startPos;
			onRelease.Invoke();
			isPressed = false;
		}
	}
}
