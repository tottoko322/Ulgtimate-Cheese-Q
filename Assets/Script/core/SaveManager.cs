using UnityEngine;
using System.IO;
using UnityEngine.InputSystem;

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
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            SaveData.tutorialCleared = true;
            Save();
            Debug.Log("保存しました");
        }

        if (Keyboard.current.lKey.wasPressedThisFrame)
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
