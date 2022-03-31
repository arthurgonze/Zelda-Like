using UnityEngine;

namespace ZL.Quests
{
    public class KillGoal : MonoBehaviour
    {
        [SerializeField] private string _questName = string.Empty;

        public void SubtractFromKillGoal()
        {
            QuestManager.SubtractQuestObjective(_questName);
            if (QuestManager.ReachedTheGoalNumber(_questName))
                QuestManager.SetQuestStatus(_questName, Quests.Quest.QuestStatus.COMPLETE);
        }
    }
}