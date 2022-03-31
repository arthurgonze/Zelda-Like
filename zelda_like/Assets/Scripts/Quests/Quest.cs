using UnityEngine;

namespace ZL.Quests
{
    [System.Serializable]
    public class Quest
    {
        public enum QuestStatus
        {
            UNASSIGNED,
            ASSIGNED,
            COMPLETE,
            FINISHED
        };

        [SerializeField] private QuestStatus _status = QuestStatus.UNASSIGNED;
        [SerializeField] private string _questName = string.Empty;
        [SerializeField] private int _howManyToComplete = 0;

        public string GetQuestName()
        {
            return _questName;
        }

        public QuestStatus GetQuestStatus()
        {
            return _status;
        }

        public int GetHowManyToComplete()
        {
            return _howManyToComplete;
        }

        public void SubtractObjectiveToComplete()
        {
            int newValue = _howManyToComplete - 1;
            _howManyToComplete = Mathf.Clamp(_howManyToComplete, 0, newValue);
        }

        public void SetQuestStatus(QuestStatus newQuestStatus)
        {
            _status = newQuestStatus;
        }
    }
}

