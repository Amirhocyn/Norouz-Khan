using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        IAction currentaAction;
        public void StartAction(IAction action)
        {
            if (currentaAction == action) return;
            if (currentaAction != null)
            {
                currentaAction.Cancel();              
            }
            currentaAction = action;
        }
    }
}