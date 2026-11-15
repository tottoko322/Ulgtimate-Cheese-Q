using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private SignScript sS;

    [SerializeField] private RectTransform mainCharacter;
    [SerializeField] private RectTransform enemyCharacter;

    [System.Serializable]
    public class DialogueEvent
    {
        public string[] sentences;  // セリフ
        public Sprite[] characterSprites; // 変更するキャラ画像
        public bool[] isMainCharacter; // 主人公キャラかどうか (true:左, false:右)
    }

    [SerializeField] private float nextLetterTime; //文字が出てくる時間(値が小さいと早い)

    [Header("AppearanceNotDialogue")]
    [SerializeField] private Color tint = Color.white; //相手が話しているときの自分の色
    [SerializeField, Range(0f,1f)] private float opacity; //相手が話しているときの自分の透明度

    [Header("Sprite")]
    public Image MainCharacterImage;  // 主人公キャラの画像
    public Image EnemyCharacterImage; // 敵キャラの画像

    [Header("PositionInDialogue")]
    [SerializeField] private float xPosition; //話すときどれくらいx座標が増えるか
    [SerializeField] private float yPosition; //話すときどれくらいy座標が増えるか


    public TMP_Text dialogueText;  // テキスト表示
    public GameObject dialogueUI; // UI全体

    public DialogueEvent[] dialogueEvents; // 会話データ
    private int currentDialogueIndex = 0; // 現在の会話イベント
    private int currentSentenceIndex = 0; // 現在のセリフ
    private bool isTyping = false; // 文字表示中フラグ

    private bool isDialogue; //会話中
    private Vector2 beginningEnemyPosition;
    private Vector2 beginningMainPosition;

    [SerializeField] private InputActionAsset inputActions; //キーコンフィグ
    private InputActionMap keyControl;

    private InputAction startDialogue;
    private InputAction nextDialogue;

    void Start()
    {
        dialogueUI.SetActive(false);

        beginningEnemyPosition = enemyCharacter.anchoredPosition;
        beginningMainPosition = mainCharacter.anchoredPosition;

        keyControl = inputActions.FindActionMap("KeyControl", true);
        startDialogue = keyControl.FindAction("StartDialogue", true);
        nextDialogue = keyControl.FindAction("NextDialogue", true);
    }

    void Update()
    {
        if (sS.isFrontSign && !isDialogue)
        {
            if (startDialogue.WasPressedThisFrame())
            {
                StartDialogue();
            }
        }

        if (nextDialogue.WasPressedThisFrame())
        {
            if (!isDialogue) //会話が開始されていないとダメ
            {
                return;
            }
            NextSentence();
            Debug.Log("次の会話");
        }
    }

    void StartDialogue()
    {
        isDialogue = true;

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
            yield return new WaitForSeconds(nextLetterTime); // 文字送り速度
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

                tint.a = opacity;
                EnemyCharacterImage.color = tint; //敵を暗く

                mainCharacter.anchoredPosition = beginningMainPosition + new Vector2(xPosition, yPosition);
                enemyCharacter.anchoredPosition = beginningEnemyPosition;
            }
            else
            {
                EnemyCharacterImage.sprite = charSprite;
                EnemyCharacterImage.color = new Color(1f, 1f, 1f, 1f); // 敵を明るく

                tint.a = opacity;
                MainCharacterImage.color = tint; //主人公を暗く

                enemyCharacter.anchoredPosition = beginningEnemyPosition + new Vector2(-xPosition, yPosition);
                mainCharacter.anchoredPosition = beginningMainPosition;
            }
        }
    }

    void CloseDialogue() //会話終了
    {
        dialogueUI.SetActive(false);
        isDialogue = false;
        
    }
}
