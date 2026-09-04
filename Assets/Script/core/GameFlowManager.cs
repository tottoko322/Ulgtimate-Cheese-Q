using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [SerializeField] private SaveManager saveManager;
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
    public void GoToTitle()
    {
        SceneManager.LoadScene("Title");
    }
    public void GoToStageSelect()
    {
        SceneManager.LoadScene("StageSelect");
    }
}
