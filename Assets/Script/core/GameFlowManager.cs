using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;
    private void Start()
    {
        DecideStartScene();
    }

    private void DecideStartScene()
    {
        if (!saveManager.SaveData.tutorialCleared)
        {
            Debug.Log("Tutorialへ移動");
        }
        else
        {
            Debug.Log("Titleへ移動");
        }
    }
}
