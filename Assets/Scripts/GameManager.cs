using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	[SerializeField] private TextCutsceneController text;

	[System.Serializable]
	private class Task
	{
		public string name;
		public bool complete;

		[Space(10)]
		[TextArea]
		public string story;

		[Space(10)]
		public bool isTimed;
		public float timeLimit;

		[Space(10)]
		public bool oneTime;
	}
	[SerializeField] private List<Task> tasks;

	private enum TaskState {}
	private TaskState taskState;

	private Task activeTask;
	private int taskCount = 0;

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

	void CompleteTask(string name)
	{
		if (name == activeTask.name)
		{
			activeTask.complete = true;
			taskCount++;
			ChooseTask();
		}
	}

	void ChooseTask()
	{

	}
}
