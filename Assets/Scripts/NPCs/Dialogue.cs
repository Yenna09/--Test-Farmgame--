using System.Collections;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

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
    [SerializeField] public DialogueLine[] conversation;

    private bool isPlayerInRange;
    private bool didDialogueStart;
    private int lineIndex;
    private float typingTime = 0.05f;

    private GameObject hotbarUI;

    [Header("Progression")]
    [SerializeField] private GameObject[] objectDisable;


    void Start()
    {
        hotbarUI = GameObject.FindGameObjectWithTag("HotBar");
        if (hotbarUI == null)
        {
            Debug.LogWarning("No se encontró la HotBar. Asegúrate de que el Tag esté bien escrito y asignado en el Inspector.");
        }

        if (dialoguePanel != null)
    {
        dialoguePanel.SetActive(false);
    }
    
        // Asegura que la marca flotante empiece desactivada 
        // (se activará cuando el Player entre al Trigger)
        if (dialogueMark != null)
        {
            dialogueMark.SetActive(false);
        }
    }

    void Update()
    {
        if(isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!didDialogueStart)
            {
                StartDialogue();
            }
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

        if (hotbarUI != null)
        {
            hotbarUI.SetActive(false);
        }
        
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
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        StopAllCoroutines(); 
        didDialogueStart = false;
        dialoguePanel.SetActive(false);
        dialogueMark.SetActive(isPlayerInRange); 
        Time.timeScale = 1f; 

        if (hotbarUI != null)
        {
            hotbarUI.SetActive(true);
        }

        foreach (var g in objectDisable)
        {
            g.SetActive(false);
        }
    }

    private IEnumerator ShowLine()
    {
        DialogueLine currentLine = conversation[lineIndex];

        if (npcNameUI != null) npcNameUI.text = currentLine.speakerName;
        if (npcPhotoUI != null) npcPhotoUI.sprite = currentLine.speakerPhoto;

        dialogueText.text = string.Empty;

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
            if (!didDialogueStart)
            {
                dialogueMark.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
            dialogueMark.SetActive(false);

            if (didDialogueStart)
            {
                EndDialogue();
            }
        }
    }
}