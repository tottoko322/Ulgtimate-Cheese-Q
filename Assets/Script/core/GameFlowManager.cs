using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [SerializeField] private SaveManager saveManager;

    [SerializeField] private StageDefinition[] stageDefinitions;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        } 
   }
    private void Start()
    {
        DecideStartScene();
    }

    private void DecideStartScene()
    {
        if (!saveManager.SaveData.tutorialCleared)
        {
            GoToTutorial();
        }
        else
        {
            GoToTitle();
        }
    }
    public void GoToTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
    public void CompleteTutorial()
    {
        saveManager.SaveData.tutorialCleared = true;
        saveManager.Save();
        GoToPrologue();
    }
    public void GoToTitle()
    {
        SceneManager.LoadScene("Title");
    }
    public void GoToStageSelect()
    {
        SceneManager.LoadScene("StageSelect");
    }
    public void GoToPrologue()
    {
        GoToStage(StageId.Prologue);
    }

    public void GoToStage(StageId stageId)
    {
        foreach (StageDefinition definition in stageDefinitions)
        {
            if (definition.stageId == stageId)
            {
                SceneManager.LoadScene(definition.sceneName);
                return;
            }
        }
        Debug.LogError("Stage not found: " + stageId);
    }
}
