using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    public static int CurrentLevel;
    private int _lastLevel;

    private void Awake()
    {
        SaveManager.Load();
    }

    private void Start()
    {
        _lastLevel = CurrentLevel;
        ExperienceGain.OnUpdateLevel += OnUpdateLevelText;
        UpdateText();
    }

    void OnUpdateLevelText()
    {
        if (CurrentLevel > _lastLevel)
        {
            _text.text = $"Unity Skill Level {CurrentLevel.ToString()}";
            _lastLevel = CurrentLevel;
        }
    }

    public static void LoadProgressFromSaveFile(SaveProgress progress)
    {
        CurrentLevel = progress.SavedLevel;
    }

    public void UpdateText()
    {
        _text.text = $"Unity Skill Level {CurrentLevel.ToString()}";
    }
}