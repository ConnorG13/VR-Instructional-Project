using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
		public bool requiresLookatDuration;
		public float durationRequired;

		[Space(10)]
		public bool oneTime;

		public UnityEngine.Events.UnityEvent OnTaskStart;
		public UnityEngine.Events.UnityEvent OnTaskComplete;
	}
	[SerializeField] private List<Task> tasks;

	[SerializeField] private int introTextStartIndex = 1;
	[SerializeField] private int introTextEndIndex = 2;
	[SerializeField] private int randomTaskStartIndex = 5;
	[SerializeField] private int randomTaskEndIndex = 10;

	[Space(10)]
	[SerializeField]
	[TextArea]
	private string gameOverStory;
	[SerializeField]
	[TextArea]
	private string taskCompleteStory;

	private enum TaskState { gameStart, finishTask, lastTask, failTask }
	private TaskState taskState = TaskState.gameStart;

	private int activeTask;
	private int taskCount = 0;
	private bool lookingAtInstructions = false;
	private bool lookingAtTask = false;
	private bool textOver = false;

    void Awake()
    {
		if (Instance == null) 
			Instance = this;
		else 
			Destroy(gameObject);
    }

	void OnEnable()
	{
		//start on first intro task
		activeTask = introTextStartIndex;
		text.StartTyping(tasks[activeTask].story);
	}

	public void OnTextComplete()
	{
		StartCoroutine(TextComplete());
	}
	IEnumerator TextComplete()
	{
		textOver = true;

		//case for game start (intro texts)
		if (taskState == TaskState.gameStart)
		{
			//while intro texts are still going on
			if (activeTask < introTextEndIndex)
			{
				//wait for eye contact
				yield return new WaitForSeconds(1f);
				yield return new WaitUntil(() => lookingAtInstructions);

				//increment active task
				activeTask++;

				//start next intro text
				textOver = false;
				text.StartTyping(tasks[activeTask].story);
				yield break;
			}
			//on last intro text
			if (activeTask == introTextEndIndex)
			{
				//wait for eye contact
				yield return new WaitForSeconds(1f);
				yield return new WaitUntil(() => lookingAtInstructions);

				//increment active task
				activeTask++;

				//start first task after intro
				StartCoroutine(DoTask(tasks[activeTask]));
				yield break;
			}
		}

		if (taskState == TaskState.finishTask)
		{
			//wait for eye contact
			yield return new WaitForSeconds(1f);
			yield return new WaitUntil(() => lookingAtInstructions);

			//choose new task
			ChooseTask();
		}

		if (taskState == TaskState.failTask)
		{
			//wait a bit
			yield return new WaitForSeconds(1f);

			//load game over scene
			SceneManager.LoadScene(1);
		}

		yield return null;
	}

	IEnumerator DoTask(Task task)
	{
		//reset called task completion in case of random selection
		task.complete = false;

		//print out task story text 
		textOver = false;
		text.StartTyping(task.story);
		yield return new WaitUntil(() => textOver);

		//call task start event for anything that needs it
		task.OnTaskStart.Invoke();

		//check if task has time limit
		if (task.isTimed)
		{
			float timeElapsed = 0;
			float durationElapsed = 0;

			//loop while task has yet to be complete, or while the time elapsed has yet to reach the time limit
			while (!task.complete && timeElapsed < task.timeLimit)
			{

				//completion check for a task that requires looking at something for a duration
				if (task.requiresLookatDuration)
				{
					if (lookingAtTask)
					{
						if (durationElapsed >= task.durationRequired)
							CompleteTask(task.name);

						durationElapsed += Time.deltaTime;
					}
					else
						durationElapsed = 0;
				}

				//count up the time gone by
				timeElapsed += Time.deltaTime;
				yield return null;
			}
			//if you went over the time limit before completeing the task, start game over
			if (timeElapsed > task.timeLimit)
			{
				GameOver();
				yield break;
			}
		}
		//if task has no time limit
		else
		{
			//completion check for a task that requires looking at something for a duration
			if (task.requiresLookatDuration)
			{
				float durationElapsed = 0;

				//loop while task has yet to be complete
				while (!task.complete)
				{
					if (lookingAtTask)
					{
						if (durationElapsed >= task.durationRequired)
							CompleteTask(task.name);

						durationElapsed += Time.deltaTime;
					}
					else
						durationElapsed = 0;

					yield return null;
				}
			}
		}

		//exit coroutine
		yield break;
	}

	public void CompleteTask(string name)
	{
		if (name == tasks[activeTask].name)
		{
			//if the completed task called is the current task, set it to complete
			tasks[activeTask].complete = true;

			//invoke the complete event
			tasks[activeTask].OnTaskComplete.Invoke();

			//remove it from the list if should only be done once
			//if (tasks[activeTask].oneTime)
			//	tasks.Remove(tasks[activeTask]);

			//increment completed task count
			taskCount++;

			//set task state to finished task and print task complete message
			taskState = TaskState.finishTask;
			textOver = false;
			text.StartTyping(taskCompleteStory);
		}
	}

	void ChooseTask()
	{

	}

	public void ToggleLookatInstructions()
	{
		lookingAtInstructions = !lookingAtInstructions;
	}

	public void ToggleLookatTask(string name)
	{
		if (tasks[activeTask].name == name)
		{
			lookingAtTask = !lookingAtTask;
		}
	}

	void GameOver()
	{
		//set task state to failed task and print game over message
		taskState = TaskState.failTask;
		textOver = false;
		text.StartTyping(gameOverStory);
	}
}
