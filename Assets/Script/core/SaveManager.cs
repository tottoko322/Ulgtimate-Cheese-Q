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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SaveData.tutorialCleared = true;
            Save();
            Debug.Log("保存しました");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("tutorialCleared = " + SaveData.tutorialCleared);
        }
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
