using ZL.Core;
using ZL.Interactives;
using System.Collections.Generic;
using UnityEngine;

namespace ZL.Quests
{
    public class TalkGoal : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _questName = string.Empty;
        [SerializeField] private TextBalloon _textBalloon;
        //[SerializeField] private TMP_Text _captions = null;
        //[SerializeField] private string[][] _captionText;
        [SerializeField] private List<string> _unassignedText;
        [SerializeField] private List<string> _assignedText;
        [SerializeField] private List<string> _completeText;
        [SerializeField] private List<string> _finishedText;

        //private List<string> _displayedText;

        private bool _interacting = false;

        // Use this for initialization
        void Start()
        {
            _textBalloon = GetComponentInChildren<TextBalloon>();
            //_displayedText = new List<string>();
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

        public void Interrupt()
        {
            _textBalloon.ResetText();
            _textBalloon.Close();
            _interacting = false;
            _textBalloon.ToggleEnded(false);
        }

        public void Close()
        {
            _interacting = false;
            _textBalloon.ToggleEnded(false);

            Quest.QuestStatus status = QuestManager.GetQuestStatus(_questName);
            if (status == Quest.QuestStatus.ASSIGNED)
                QuestManager.SetQuestStatus(_questName, Quest.QuestStatus.COMPLETE);
        }

        public void Interact()
        {
            _interacting = true;
            _textBalloon.ToggleAutomatic(false);
            Quest.QuestStatus status = QuestManager.GetQuestStatus(_questName);
            SetDialogueTest(status);
            _textBalloon.PlayText();
        }

        private void NextLine()
        {
            _textBalloon.NextLine();
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
    }
}