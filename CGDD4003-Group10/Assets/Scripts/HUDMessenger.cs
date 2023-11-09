using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDMessenger : MonoBehaviour
{
    struct Message
    {
        public string message;
        public float length;

        public Message(string message, float length)
        {
            this.message = message;
            this.length = length;
        }
    }

    [SerializeField] TMP_Text displayBox;
    [SerializeField] TMP_Text corruptedDisplayBox;
    [SerializeField] float displaySpeed = 30;
    [SerializeField] float corruptedDisplaySpeed = 15;

    Queue<Message> messages = new Queue<Message>();

    Coroutine displayCoroutine;

    public void Display(string message, float length)
    {
        messages.Enqueue(new Message(message, length));

        if (displayCoroutine == null)
            displayCoroutine = StartCoroutine(DisplayMessage(displayBox, displaySpeed));
    }

    public void CorruptedDisplay(string message, float length)
    {
        messages.Enqueue(new Message(message, length));

        if (displayCoroutine != null)
            StopCoroutine(displayCoroutine);

        displayBox.text = "";

        displayCoroutine = StartCoroutine(DisplayMessage(corruptedDisplayBox, corruptedDisplaySpeed));
    }


    IEnumerator DisplayMessage(TMP_Text displayBox, float displaySpeed)
    {
        WaitForSeconds displayTypeInterval = new WaitForSeconds(1 / displaySpeed);

        while(messages.Count > 0)
        {
            Message currentMessage = messages.Dequeue();
            float displayLength = currentMessage.length;
            string message = currentMessage.message;
            displayBox.text = "";

            for (int i = 0; i < message.Length; i++)
            {
                displayBox.text += message[i];
                yield return displayTypeInterval;
            }

            if(messages.Count > 0)
            {
                yield return new WaitForSeconds(displayLength / 2);
            } else
            {
                yield return new WaitForSeconds(displayLength);
            }
        }

        displayBox.text = "";
        displayCoroutine = null;
    }
}
