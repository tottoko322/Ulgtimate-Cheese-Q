using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public SaveData SaveData { get; private set; }
    private string saveFilePath;
    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
        Load();
    }
    public void Save()
    {
        string json = JsonUtility.ToJson(SaveData);
        File.WriteAllText(saveFilePath, json);
    }
    public void Load()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            CreateNewSave();
            Save();
        }
    }
    private void CreateNewSave()
    {
        SaveData = new SaveData();
    }

}
