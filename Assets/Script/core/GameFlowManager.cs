using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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
}
