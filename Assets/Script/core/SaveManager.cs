using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public SaveData SaveData { get; private set; }
    private string saveFilePath;
    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
    }
    public void Save()
    {
        string json = JsonUtility.ToJson(SaveData);
        File.WriteAllText(saveFilePath, json);
    }

    private void CreateNewSave()
    {
        SaveData = new SaveData();
    }

}
