using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QuestHandler;

[CreateAssetMenu(fileName = "Tutorial", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class Tutorial : QuestHolder
{

    public override Quest GetQuest()
    {
        List<Objective> objectives = new List<Objective>();
        objectives.Add(MoveObjective());
        objectives.Add(LookObjective());
        return new Quest("Tutorial", "Learn the Basics", objectives);
    }

    private Objective MoveObjective()
    {
        List<Task> tasks = new List<Task>();
        tasks.Add(new Task("W to move forward", new Axis("Vertical", true)));
        tasks.Add(new Task("S to move backward", new Axis("Vertical", false)));
        tasks.Add(new Task("A to move left", new Axis("Horizontal", false)));
        tasks.Add(new Task("D to move right", new Axis("Horizontal", true)));
        return new Objective("Move", "Use WASD to move.", tasks, new Completed(tasks));
    }

    private Objective LookObjective()
    {
        List<Task> tasks = new List<Task>();
        tasks.Add(new Task("Mouse Up to look up.", new Axis("Mouse Y", true)));
        tasks.Add(new Task("Mouse Down to look down.", new Axis("Mouse Y", false)));
        tasks.Add(new Task("Mouse Left to look left.", new Axis("Mouse X", false)));
        tasks.Add(new Task("Mouse Right to look right.", new Axis("Mouse X", true)));
        return new Objective("Look", "Use Mouse to look.", tasks, new Completed(tasks));
    }

    private class Axis : Condition
    {
        private string axis;
        private bool positive;
        private bool met;

        public Axis(string axis, bool positive)
        {
            this.axis = axis;
            this.positive = positive;
        }

        public bool Met()
        {
            float input = Input.GetAxis(axis);
            if (positive && input > 0f || !positive && input < 0f)
                met = true;
            return met;
        }

    }

    private class WCondition : Condition
    {
        private bool wPressed = false;

        public bool Met()
        {
            if (Input.GetAxis("Vertical") > 0f)
                wPressed = true;
            return wPressed;
        }
    }

    private class ACondition : Condition
    {
        private bool aPressed = false;

        public bool Met()
        {
            if (Input.GetAxis("Horizontal") < 0f)
                aPressed = true;
            return aPressed;
        }
    }

    private class SCondition : Condition
    {
        private bool sPressed = false;

        public bool Met()
        {
            if (Input.GetAxis("Vertical") < 0f)
                sPressed = true;
            return sPressed;
        }
    }

    private class DCondition : Condition
    {
        private bool dPressed = false;

        public bool Met()
        {
            if (Input.GetAxis("Horizontal") > 0f)
                dPressed = true;
            return dPressed;
        }
    }

    private class Completed : Condition
    {
        private List<Task> tasks;

        public Completed(List<Task> tasks)
        {
            this.tasks = tasks;
        }

        public bool Met()
        {
            foreach (Task task in tasks)
                if (!task.Condition.Met())
                    return false;
            return true;
        }

    }

}
