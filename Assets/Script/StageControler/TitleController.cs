using UnityEngine;

public class TitleController : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        GameFlowManager.Instance.GoToStageSelect();
    }
}