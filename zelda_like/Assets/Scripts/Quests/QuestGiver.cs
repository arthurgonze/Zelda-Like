using ZL.Core;
using ZL.Interactives;
using ZL.Quest;
using System.Collections.Generic;
using UnityEngine;

namespace ZL.Quests
{
    public class QuestGiver : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _questName = string.Empty;
        [SerializeField] private TextBalloon _textBalloon;
        //[SerializeField] private string[][] _captionText;// 4 Quest.QuestStatus avaiable
        [SerializeField] private int _questPayment = 0;
        [SerializeField] private QuestItem[] _itemsOnMap;

        [SerializeField] private List<string> _unassignedText;
        [SerializeField] private List<string> _assignedText;
        [SerializeField] private List<string> _completeText;
        [SerializeField] private List<string> _finishedText;

        private bool _interacting = false;
        private bool _alreadyPayed = false;


        // Use this for initialization
        void Start()
        {
            _textBalloon = GetComponentInChildren<TextBalloon>();
        }

        public void InteractWithNPC()
        {
            if (!_interacting)
                Interact();
            else if (_interacting && !_textBalloon.Ended())
                NextLine();
            else if (_interacting && _textBalloon.Ended())
                Close();
        }

        public void CloseInteractionWithNPC()
        {
            if (_interacting)
                Interrupt();
        }


        public void Close()
        {
            _interacting = false;
            _textBalloon.ToggleEnded(false);

            Quest.QuestStatus status = QuestManager.GetQuestStatus(_questName);
            if (status != Quest.QuestStatus.UNASSIGNED) return;
            QuestManager.SetQuestStatus(_questName, Quest.QuestStatus.ASSIGNED);

            if (_itemsOnMap == null) return;
            foreach (QuestItem item in _itemsOnMap)
                item.gameObject.SetActive(true);
        }

        public void Interrupt()
        {
            _textBalloon.ResetText();
            _textBalloon.Close();
            _interacting = false;
            _textBalloon.ToggleEnded(false);
        }

        public void Interact()
        {
            //if (_alreadyPayed) return;
            _interacting = true;
            _textBalloon.ToggleAutomatic(false);

            Quest.QuestStatus status = QuestManager.GetQuestStatus(_questName);
            //if(_displayedText.Count > 0) 
            //_displayedText.Clear();

            SetDialogueTest(status);
            //foreach (string text in _captionText[(int)status])
            //    _displayedText.Add(text);
            //_textBalloon.SetDialogo(_displayedText);
            _textBalloon.PlayText();

            if (!_alreadyPayed && status.Equals(Quest.QuestStatus.COMPLETE))
            {
                GameObject player = GameObject.FindWithTag("Player");
                player.GetComponent<Inventory.Inventory>().AddCoins(_questPayment);
                QuestManager.SetQuestStatus(_questName, Quest.QuestStatus.FINISHED);
                _alreadyPayed = true;
            }
        }

        private void SetDialogueTest(Quest.QuestStatus questStatus)
        {
            switch (questStatus)
            {
                case Quest.QuestStatus.UNASSIGNED:
                    _textBalloon.SetDialogo(_unassignedText);
                    break;
                case Quest.QuestStatus.ASSIGNED:
                    _textBalloon.SetDialogo(_assignedText);
                    break;
                case Quest.QuestStatus.COMPLETE:
                    _textBalloon.SetDialogo(_completeText);
                    break;
                case Quest.QuestStatus.FINISHED:
                    _textBalloon.SetDialogo(_finishedText);
                    break;
                default:
                    Debug.Log("Quest Status Not Found");
                    break;
            }
        }

        private void NextLine()
        {
            _textBalloon.NextLine();
        }
    }
}