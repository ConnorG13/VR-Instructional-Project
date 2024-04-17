using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEvent : MonoBehaviour
{
	[SerializeField] private Transform doorOne;
	[SerializeField] private Transform doorTwo;
	private Vector3 doorStartPos;

	[SerializeField] private Transform lever;

	[SerializeField] private float totalOpenDuration;

	AudioSource sound;

	public UnityEngine.Events.UnityEvent OnDoorClosed;

	private void Start()
	{
		doorStartPos = doorOne.localPosition;
		sound = GetComponent<AudioSource>();
	}

	public void StartOpening()
	{
		sound.Play();
		StartCoroutine(Opening());
	}
	IEnumerator Opening()
	{
		float destination;
		while (lever.localRotation.eulerAngles.x < 150)
		{
			destination = Mathf.Lerp(doorOne.transform.localPosition.y, 0, totalOpenDuration * Time.deltaTime);
			doorOne.transform.localPosition = new Vector3(doorOne.transform.localPosition.x, destination, doorOne.transform.localPosition.z);

			yield return null;
		}
		if (lever.rotation.eulerAngles.x >= 150)
		{
			StartCoroutine(Opening());
			yield break;
		}
	}

	IEnumerator Closing()
	{
		float destination;
		while (lever.localRotation.eulerAngles.x >= 150)
		{
			destination = Mathf.Lerp(doorOne.transform.localPosition.y, doorStartPos.y, totalOpenDuration * Time.deltaTime);
			doorOne.transform.localPosition = new Vector3(doorOne.transform.localPosition.x, destination, doorOne.transform.localPosition.z);

			yield return null;
		}
		if (lever.rotation.eulerAngles.x < 150)
		{
			StartCoroutine(Closing());
			yield break;
		}
	}



    void Update()
    {
        
    }
}
