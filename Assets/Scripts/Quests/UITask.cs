using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static QuestHandler;

public class UITask : MonoBehaviour
{

    public Task Task;
    public TextMeshProUGUI Text;
    public RectTransform RectTransform;

    void Update()
    {
        if (Task.Condition.Met())
            Text.color = Color.blue;
    }
}
