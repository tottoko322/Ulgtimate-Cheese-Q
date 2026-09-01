using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public SaveData SaveData { get; private set; }

    private void CreateNewSave()
    {
        SaveData = new SaveData();
    }
}
