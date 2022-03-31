using ZL.Saving;
using ZL.SceneManagement;
using UnityEngine;

namespace ZL.Quests
{
    public class QuestManager : MonoBehaviour, ISavable
    {
        private static QuestManager thisInstance = null;
        [SerializeField] private Quest[] _quests;

        void Awake()
        {
            if (thisInstance == null)
            {
                //DontDestroyOnLoad(this);
                thisInstance = this;
            }
            //else
            //{
            //    DestroyImmediate(gameObject);
            //}
        }

        public static Quest.QuestStatus GetQuestStatus(string questName)
        {
            foreach (Quest quest in thisInstance._quests)
            {
                if (quest.GetQuestName().Equals(questName))
                    return quest.GetQuestStatus();
            }

            return Quest.QuestStatus.UNASSIGNED;
        }

        public static void SetQuestStatus(string questName, Quest.QuestStatus newStatus)
        {
            foreach (Quest quest in thisInstance._quests)
            {
                if (quest.GetQuestName().Equals(questName))
                {
                    quest.SetQuestStatus(newStatus);
                    return;
                }
            }
            FindObjectOfType<SavingWrapper>().Save();
        }

        public static void SubtractQuestObjective(string questName)
        {
            foreach (Quest quest in thisInstance._quests)
            {
                if (quest.GetQuestName().Equals(questName))
                {
                    quest.SubtractObjectiveToComplete();
                    return;
                }
            }
            FindObjectOfType<SavingWrapper>().Save();
        }

        public static bool ReachedTheGoalNumber(string questName)
        {
            foreach (Quest quest in thisInstance._quests)
            {
                if (quest.GetQuestName().Equals(questName))
                {
                    return quest.GetHowManyToComplete() == 0;
                }
            }

            return false;
        }

        public static void Reset()
        {
            foreach (Quest quest in thisInstance._quests)
            {
                quest.SetQuestStatus(Quest.QuestStatus.UNASSIGNED);
            }
            FindObjectOfType<SavingWrapper>().Save();
        }

        public object CaptureState()
        {
            return _quests;
        }

        public void RestoreState(object state)
        {
            this._quests = (Quest[])state;
        }
    }
}