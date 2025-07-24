using System.Collections.Generic;
using UnityEngine;

namespace RedstoneinventeGameStudio
{
    public class NPCManager : MonoBehaviour
    {
        public List<NPCDialogueSO> dialogues;
        public int currentDialogueIndex = 0;

        public void MoveNext()
        {
            currentDialogueIndex++;
            if (currentDialogueIndex >= dialogues.Count)
            {
                currentDialogueIndex = 0;
            }
        }

        private void OnMouseUp()
        {
            DialogueManager.instance.ShowDialogue(this);
        }
    }
}