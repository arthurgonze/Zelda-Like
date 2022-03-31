using ZL.Core;
using ZL.Quests;
using UnityEngine;

namespace ZL.Quest
{
    public class QuestItem : MonoBehaviour
    {
        [SerializeField] private string _questName = string.Empty;

        // cached references
        private SpriteRenderer _renderer;
        private Collider2D _collider;

        void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _renderer = GetComponentInChildren<SpriteRenderer>();
        }

        // Use this for initialization
        void Start()
        {
            if (QuestManager.GetQuestStatus(_questName) == Quests.Quest.QuestStatus.ASSIGNED)
                gameObject.SetActive(true);
            else
                gameObject.SetActive(false);
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (!gameObject.activeSelf) return;
            _renderer.enabled = _collider.enabled = false;
            QuestManager.SubtractQuestObjective(_questName);

            if (QuestManager.ReachedTheGoalNumber(_questName))
                QuestManager.SetQuestStatus(_questName, Quests.Quest.QuestStatus.COMPLETE);
            FindObjectOfType<AudioController>().PlayPickupQuestSound();
        }

        public string GetQuestName()
        {
            return _questName;
        }
    }
}