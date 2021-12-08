using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static QuestHandler;

public class UIQuest : MonoBehaviour
{

    public Quest Quest;
    public TextMeshProUGUI Text;
    public RectTransform RectTransform;
    public readonly List<UIObjective> uiObjectives = new List<UIObjective>();

    private void Update()
    {
        UpdateQuestPositions();
        UpdateHeight();
    }

    private void UpdateQuestPositions()
    {
        foreach (UIObjective uiObjective in uiObjectives)
        {
            float y = -Text.rectTransform.rect.height;
            for (int i = 0; i < uiObjectives.IndexOf(uiObjective); i++)
                y -= uiObjectives[i].RectTransform.rect.height;
            Vector2 position = uiObjective.RectTransform.anchoredPosition;
            position.y = y;
            uiObjective.RectTransform.anchoredPosition = position;
        }
    }

    private void UpdateHeight()
    {
        float height = Text.rectTransform.rect.height;
        foreach (UIObjective uiObjective in uiObjectives)
            height += uiObjective.RectTransform.rect.height;
        RectTransform.sizeDelta = new Vector2(0f, height);
    }

}

