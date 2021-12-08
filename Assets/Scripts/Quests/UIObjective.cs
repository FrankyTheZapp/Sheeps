using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static QuestHandler;

public class UIObjective : MonoBehaviour
{

    public Objective Objective;
    public TextMeshProUGUI Text;
    public RectTransform RectTransform;
    public readonly List<UITask> uiTasks = new List<UITask>();

    private void Update()
    {
        UpdateTaskPositions();
        UpdateHeight();
        CheckIfCompleted();
    }

    private void UpdateTaskPositions()
    {
        foreach (UITask uiTask in uiTasks)
        {
            float y = -Text.rectTransform.rect.height;
            for (int i = 0; i < uiTasks.IndexOf(uiTask); i++)
                y -= uiTasks[i].RectTransform.rect.height;
            Vector2 position = uiTask.RectTransform.anchoredPosition;
            position.y = y;
            uiTask.RectTransform.anchoredPosition = position;
        }
    }

    private void UpdateHeight()
    {
        float height = Text.rectTransform.rect.height;
        foreach (UITask uiTask in uiTasks)
            height += uiTask.RectTransform.rect.height;
        RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, height);
    }

    private void CheckIfCompleted()
    {
        if (Objective.Condition.Met())
            Text.color = Color.blue;
    }

}
