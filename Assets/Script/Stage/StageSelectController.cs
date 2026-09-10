using UnityEngine;

public class StageSelectController : MonoBehaviour
{
    public void OnPortButtonClicked()
    {
        GameFlowManager.Instance.GoToStage(StageId.Port);
    }

}