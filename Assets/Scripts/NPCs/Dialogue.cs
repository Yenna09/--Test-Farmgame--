using System.Collections;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

// 1. Creamos la clase que contiene los datos de cada línea de diálogo
[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public Sprite speakerPhoto;
    [TextArea(4, 6)]
    public string text;
}

public class Dialogue : MonoBehaviour
{
    [Header("UI References (Canvas)")]
    [SerializeField] private GameObject dialogueMark; 
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image npcPhotoUI;   
    [SerializeField] private TMP_Text npcNameUI; 

    [Header("Dialogue Data")]
    // 2. Reemplazamos los datos sueltos por un array de nuestra nueva clase
    [SerializeField] private DialogueLine[] conversation;

    private bool isPlayerInRange;
    private bool didDialogueStart;
    private int lineIndex;
    private float typingTime = 0.05f;

    void Update()
    {
        if(isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (!didDialogueStart)
            {
                StartDialogue();
            }
            // Comparamos contra el texto de la línea actual
            else if(dialogueText.text == conversation[lineIndex].text)
            {
                NextDialogueLine();
            }
            else
            {
                StopAllCoroutines();
                dialogueText.text = conversation[lineIndex].text;
            }
        }
    }

    private void StartDialogue()
    {
        didDialogueStart = true;
        dialoguePanel.SetActive(true);
        dialogueMark.SetActive(false);
        
        lineIndex = 0;
        Time.timeScale = 0f; 
        StartCoroutine(ShowLine());
    }

    private void NextDialogueLine()
    {
        lineIndex++;
        if (lineIndex < conversation.Length)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            didDialogueStart = false;
            dialoguePanel.SetActive(false);
            dialogueMark.SetActive(true);
            Time.timeScale = 1f; 
        }
    }

    private IEnumerator ShowLine()
    {
        // 3. Obtenemos la información de la línea actual
        DialogueLine currentLine = conversation[lineIndex];

        // 4. Actualizamos la UI con la foto y nombre del que habla AHORA
        if (npcNameUI != null) npcNameUI.text = currentLine.speakerName;
        if (npcPhotoUI != null) npcPhotoUI.sprite = currentLine.speakerPhoto;

        // Limpiamos el texto principal
        dialogueText.text = string.Empty;

        // Escribimos el texto letra por letra
        foreach (char ch in currentLine.text)
        {
            dialogueText.text += ch;
            yield return new WaitForSecondsRealtime(typingTime);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            dialogueMark.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
            dialogueMark.SetActive(false);
        }
    }
}