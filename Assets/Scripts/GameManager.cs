using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	[SerializeField] private TextCutsceneController text;

    void Awake()
    {
		if (Instance == null) 
			Instance = this;
		else 
			Destroy(gameObject);
    }

	void BeginTasks()
	{
		
	}
}
