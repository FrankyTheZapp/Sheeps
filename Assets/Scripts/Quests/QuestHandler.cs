using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestHandler : MonoBehaviour
{
    public RectTransform QuestsContainer;
    public RectTransform QuestContainer;
    public RectTransform ObjectiveContainer;
    public RectTransform TaskContainer;
    public RectTransform RectTransform;
    public List<QuestHolder> QuestHolders = new List<QuestHolder>();

    private QuestHandler instance;
    private readonly List<UIQuest> uiQuests = new List<UIQuest>();

    private void Start()
    {
        instance = this;
        foreach (QuestHolder questHolder in QuestHolders)
            NewUIQuest(questHolder.GetQuest());
    }

    private void Update()
    {
        foreach (UIQuest uiQuest in uiQuests)
        {
            float y = 0;
            for (int i = 0; i < uiQuests.IndexOf(uiQuest); i++)
                y -= uiQuests[i].RectTransform.rect.height;
            Vector2 position = uiQuest.RectTransform.anchoredPosition;
            position.y = y;
            uiQuest.RectTransform.anchoredPosition = position;
        }
    }

    private void NewUIQuest(Quest quest)
    {
        UIQuest uiQuest = Instantiate(QuestContainer, QuestsContainer).GetComponent<UIQuest>();
        uiQuest.Quest = quest;
        uiQuest.Text.text = quest.Title;
        foreach (Objective objective in quest.Objectives)
        {
            UIObjective uiObjective = Instantiate(ObjectiveContainer, uiQuest.transform).GetComponent<UIObjective>();
            uiObjective.Objective = objective;
            uiObjective.Text.text = objective.Title;
            foreach (Task task in objective.Tasks)
            {
                UITask uiTask = Instantiate(TaskContainer, uiObjective.transform).GetComponent<UITask>();
                uiTask.Task = task;
                uiTask.Text.text = task.Title;
                uiObjective.uiTasks.Add(uiTask);
            }
            uiQuest.uiObjectives.Add(uiObjective);
        }
        uiQuests.Add(uiQuest);
    }

    public abstract class QuestHolder : ScriptableObject
    {
        public abstract Quest GetQuest();
    }

    public class Quest
    {
        public string Title;
        public string Description;
        public List<Objective> Objectives;
        public List<Quest> FollowUpQuests;

        public Quest(string title, string description, List<Objective> objectives)
        {
            this.Title = title;
            this.Description = description;
            this.Objectives = objectives;
        }

        public Quest(string title, string description, Objective[] objectives) : this(title, description, new List<Objective>(objectives))
        {
        }

    }

    public class Objective
    {
        public string Title;
        public string Description;
        public List<Task> Tasks;
        public Condition Condition;
        public List<Objective> FollowUpObjectives;

        public Objective(string title, string description, List<Task> tasks, Condition condition)
        {
            this.Title = title;
            this.Description = description;
            this.Tasks = tasks;
            this.Condition = condition;
        }

        public Objective(string title, string description, Task[] tasks, Condition condition) : this(title, description, new List<Task>(tasks), condition)
        {
        }

    }

    public class Task
    {
        public string Title;
        public Condition Condition;

        public Task(string title, Condition condition)
        {
            this.Title = title;
            this.Condition = condition;
        }

    }

    public interface Condition
    {

        public bool Met();

    }

}
