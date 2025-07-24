using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

namespace RedstoneinventeGameStudio
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager instance;

        public TMP_Text title;
        public TMP_Text content;

        public float characterDelay = 0.1f;
        public float punctuationDelay = 0.5f;

        public int maxWords = 10;

        public bool moveNext;
        public GameObject moveNextButt;

        public Canvas dialogueCanvas;

        private void Awake()
        {
            instance = this;
        }

        public void MoveNext()
        {
            moveNext = true;
        }

        public void ShowDialogue(NPCManager nPCManager)
        {
            StartCoroutine(ShowDialogueC(nPCManager));
        }

        IEnumerator ShowDialogueC(NPCManager nPCManager)
        {
            title.text = nPCManager.dialogues[nPCManager.currentDialogueIndex].title;
            content.text = "";

            dialogueCanvas.enabled = true;

            foreach (char character in nPCManager.dialogues[nPCManager.currentDialogueIndex].lines)
            {
                content.text += character;
                yield return new WaitForSeconds(IsPunctuation(character) ? punctuationDelay : characterDelay);

                if (content.textInfo.wordCount >= maxWords && (character == ' ' || IsPunctuation(character) || character == ','))
                {
                    moveNextButt.SetActive(true);
                    yield return new WaitUntil(() => moveNext);

                    content.text = "";
                    moveNextButt.SetActive(false);
                    moveNext = false;
                }
            }

            moveNextButt.SetActive(true);
            yield return new WaitUntil(() => moveNext);

            content.text = "";
            moveNextButt.SetActive(false);
            moveNext = false;

            dialogueCanvas.enabled = false;

            nPCManager.MoveNext();
        }

        bool IsPunctuation(char character)
        {
            return character == '.' || character == '?' || character == '!';
        }
    }

}