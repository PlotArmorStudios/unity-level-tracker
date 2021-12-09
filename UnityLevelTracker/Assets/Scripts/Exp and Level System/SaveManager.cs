using System.IO;
using UnityEngine;

public class SaveManager
{
     private static SaveProgress _progress;
    private static string _filePath;

    static public bool LOCK { get; private set; }

    static SaveManager()
    {
        LOCK = false;
        _filePath = Application.persistentDataPath + "/CodingConceptPracticeProject.saveprogress";
        _progress = new SaveProgress();
    }

    public static void Save()
    {
        if (LOCK) return;

        _progress.SavedLevel = LevelManager.CurrentLevel;
        _progress.SavedExperience = ExperienceBar.CurrentExperience;
        _progress.SavedFiveMinuteTimer = StudyTimer._5MinTimer;
        _progress.SavedTenMinuteTimer = StudyTimer._10MinTimer;
        _progress.SavedThirtyMinuteTimer = StudyTimer._30MinTimer;
        
        string jsonSaveFile = JsonUtility.ToJson(_progress, true);
        
        File.WriteAllText(_filePath, jsonSaveFile);

        Debug.Log("SaveManager: Save() - Path: " + _filePath);
        Debug.Log("SaveManager: Save() - JSON: " + jsonSaveFile);
    }

    public static void Load()
    {
        if (File.Exists(_filePath))
        {
            string dataAsJson = File.ReadAllText(_filePath);

            try
            {
                _progress = JsonUtility.FromJson<SaveProgress>(dataAsJson);
                Debug.Log("Loaded Save Progress File.");
            }
            catch
            {
                Debug.LogWarning("SaveManager:Load() – SaveFile was malformed.\n" + dataAsJson);
                return;
            }

            //Lock is set to true to protect save file while loading code
            LOCK = true;
            //here is where you would assign data from _saveInfo to the fields of, for example, an Achievement Manager,
            //A Game Manager, a UI Manager, character cosmetics, etc
            //This is what "Loading" is, assigning variables from a save file to the fields
            //of the current game state
            ExperienceBar.LoadProgressFromSaveFile(_progress);
            LevelManager.LoadProgressFromSaveFile(_progress);
            StudyTimer.LoadProgressFromSaveFile(_progress);
            
            LOCK = false; //makes it so you can save again
        }
    }
    
    static public void DeleteSave()
    {
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
            _progress = new SaveProgress();
            Debug.Log("SaveGameManager:DeleteSave() – Successfully deleted save file.");
        }
        else
        {
            Debug.LogWarning("SaveGameManager:DeleteSave() – Unable to find and delete save file!"
                             + " This is absolutely fine if you've never saved or have just deleted the file.");
        }

        // Lock the file to prevent any saving
        LOCK = true;

       //Here is where you would RESET all values and states in various scripts

        // Unlock the file
        LOCK = false;
    }
}
