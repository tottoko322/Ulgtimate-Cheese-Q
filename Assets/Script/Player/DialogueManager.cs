using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private SignScript sS;
    private bool wasFrontSign;

    [System.Serializable]
    public class DialogueEvent
    {
        public string[] sentences;  // セリフ
        public Sprite[] characterSprites; // 変更するキャラ画像
        public bool[] isMainCharacter; // 主人公キャラかどうか (true:左, false:右)
    }

    public TMP_Text dialogueText;  // テキスト表示
    public Image MainCharacterImage;  // 主人公キャラの画像
    public Image EnCharacterImage; // 敵キャラの画像
    public Button checkButton; // 会話開始ボタン
    public GameObject dialogueUI; // UI全体

    public DialogueEvent[] dialogueEvents; // 会話データ
    private int currentDialogueIndex = 0; // 現在の会話イベント
    private int currentSentenceIndex = 0; // 現在のセリフ
    private bool isTyping = false; // 文字表示中フラグ

    private bool isDialogue;

    void Start()
    {
        dialogueUI.SetActive(false);
        checkButton.gameObject.SetActive(false);
        checkButton.onClick.AddListener(StartDialogue);
    }

    void Update()
    {
        if (sS.isFrontSign != wasFrontSign)
        {
            if (sS.isFrontSign && !isDialogue) //会話中は看板に近づいてもチェックボタンを表示しない
            {
                DisplayCheckButton();
            }
            else
            {
                HideCheckButton();
            }
            wasFrontSign = sS.isFrontSign;
        }

        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            if (!isDialogue) //会話が開始されていないとダメ
            {
                return;
            }
            NextSentence();
            Debug.Log("次の会話");
        }
    }

    void DisplayCheckButton()
    {
        checkButton.gameObject.SetActive(true);
    }

    void HideCheckButton()
    {
        checkButton.gameObject.SetActive(false);
    }

    void StartDialogue()
    {
        isDialogue = true;

        checkButton.gameObject.SetActive(false);
        dialogueUI.SetActive(true);
        currentDialogueIndex = 0;
        currentSentenceIndex = 0;
        StartCoroutine(TypeSentence(dialogueEvents[currentDialogueIndex].sentences[currentSentenceIndex]));
        UpdateCharacterImage();
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        // **\n を正しく改行に変換**
        sentence = sentence.Replace("\\n", "\n");

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f); // 文字送り速度
        }

        isTyping = false;
    }

    void NextSentence()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = dialogueEvents[currentDialogueIndex].sentences[currentSentenceIndex].Replace("\\n", "\n");
            isTyping = false;
        }
        else
        {
            currentSentenceIndex++;

            if (currentSentenceIndex < dialogueEvents[currentDialogueIndex].sentences.Length)
            {
                StartCoroutine(TypeSentence(dialogueEvents[currentDialogueIndex].sentences[currentSentenceIndex]));
                UpdateCharacterImage();
            }
            else
            {
                currentSentenceIndex = 0;
                currentDialogueIndex++;

                if (currentDialogueIndex < dialogueEvents.Length)
                {
                    StartCoroutine(TypeSentence(dialogueEvents[currentDialogueIndex].sentences[currentSentenceIndex]));
                    UpdateCharacterImage();
                }
                else
                {
                    CloseDialogue();
                }
            }
        }
    }

    void UpdateCharacterImage()
    {
        // インデックスが範囲内かチェック
        if (currentDialogueIndex < dialogueEvents.Length && currentSentenceIndex < dialogueEvents[currentDialogueIndex].isMainCharacter.Length)
        {
            bool isLeft = dialogueEvents[currentDialogueIndex].isMainCharacter[currentSentenceIndex];
            Sprite charSprite = dialogueEvents[currentDialogueIndex].characterSprites[currentSentenceIndex];

            if (isLeft)
            {
                MainCharacterImage.sprite = charSprite;
                MainCharacterImage.color = new Color(1f, 1f, 1f, 1f); // 主人公を明るく
                EnCharacterImage.color = new Color(1f, 1f, 1f, 0.5f); // 敵を薄く
            }
            else
            {
                EnCharacterImage.sprite = charSprite;
                EnCharacterImage.color = new Color(1f, 1f, 1f, 1f); // 敵を明るく
                MainCharacterImage.color = new Color(1f, 1f, 1f, 0.5f); // 主人公を薄く
            }
        }
    }

    void CloseDialogue() //会話終了
    {
        dialogueUI.SetActive(false);
        isDialogue = false;
        
    }
}
