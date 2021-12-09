using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    [SerializeField] private Image _expImage;
    [SerializeField] private float _experienceFillSpeed = .1f;
    [SerializeField] private TMP_Text _expText;
    [SerializeField] private TMP_Text _lvlUpText;
    [SerializeField] private AudioSource _expAudio;
    [SerializeField] private AudioSource _lvlAudio;

    public float _MAXExperience = 1000f;
    public static float CurrentExperience;
    private float _experienceChange;
    
    private Animator _expAnimator;
    private Button[] _buttons;


    private void Start()
    {
        ExperienceGain.OnExperienceGain += OnExperienceUpdate;
        _expAnimator = GetComponent<Animator>();
        _buttons = FindObjectsOfType<Button>();
        
        UpdateAfterLoadSaveFile();
    }

    private void CalculateMaxExp()
    {
        _MAXExperience = 1000f * Mathf.Pow(1.1f, LevelManager.CurrentLevel);
    }

    private void CalculateFillSpeed(float gain)
    {
        var gainPercentage = gain / _MAXExperience;
        _experienceFillSpeed = .008f * Mathf.Pow(10, gainPercentage);
    }

    private void OnExperienceUpdate(float experienceGain = 0)
    {
        if (experienceGain > 0)
        {
            AnimateExpText(experienceGain);
            _expAudio.PlayOneShot(_expAudio.clip);
        }

        CurrentExperience += experienceGain;
        CalculateMaxExp();
        CalculateFillSpeed(experienceGain);


        StartCoroutine(SmoothExperienceIncrease());
        
        if (CurrentExperience >= _MAXExperience)
        {
            StartCoroutine(LevelUp());
        }
    }

    private void AnimateExpText(float gain)
    {
        _expText.text = $"+{gain.ToString()}XP";
        _expAnimator.SetTrigger("Add Exp");
    }

    private IEnumerator LevelUp()
    {
        yield return _expImage.fillAmount == 1f;
        
        LevelManager.CurrentLevel++;
        ExperienceGain.UpdateLevel();
        
        _lvlAudio.Play();
        _lvlUpText.GetComponent<Animator>().SetTrigger("Level Up");

        
        foreach (var button in _buttons)
        {
            button.interactable = false;
        }
        
        yield return new WaitForSeconds(.5f);
        
        CalculateMaxExp();
        CurrentExperience = 0;
        
        yield return _expImage.fillAmount == 0f;
        
        foreach (var button in _buttons)
        {
            button.interactable = true;
        }
    }

    IEnumerator SmoothExperienceIncrease()
    {
        while (_expImage.fillAmount != CurrentExperience / _MAXExperience)
        {
            if (_expImage.fillAmount > CurrentExperience / _MAXExperience)
                _expImage.fillAmount -= _experienceFillSpeed;

            if (_expImage.fillAmount < CurrentExperience / _MAXExperience)
                _expImage.fillAmount += _experienceFillSpeed;
            
            yield return null;
        }
    }

    public static void LoadProgressFromSaveFile(SaveProgress progress)
    {
        CurrentExperience = progress.SavedExperience;
    }

    public void UpdateAfterLoadSaveFile()
    {
        OnExperienceUpdate();
    }
}