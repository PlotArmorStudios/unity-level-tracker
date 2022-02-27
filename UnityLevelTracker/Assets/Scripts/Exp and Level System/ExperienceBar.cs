//#define DEBUG_LOG
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    [SerializeField] private Image _expImage;
    [SerializeField] private Image _expGlowEffect;
    [SerializeField] private float _experienceFillSpeed = .1f;
    
    [SerializeField] private TMP_Text _expText;
    [SerializeField] private TMP_Text _lvlUpText;
    [SerializeField] private AudioSource _lvlAudio;
    [SerializeField] private AudioSource _expAudio;

    public static ExperienceBar Instance;
    
    public float CurrentExperience;
    public float MAXExperience = 1000f;
    
    private Animator _expAnimator;
    private Button[] _buttons;

    private float _experienceChange;
    private float _carryOverExp;
    private float _MINexperienceFillSpeed = .0013f;
    private float _MAXexperienceFillSpeed = .01f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ExperienceGain.OnExperienceGain += OnExperienceUpdate;
        _expAnimator = GetComponent<Animator>();
        _buttons = FindObjectsOfType<Button>();

        UpdateAfterLoadSaveFile();
    }

    private void CalculateMaxExp()
    {
        MAXExperience = 1000f * Mathf.Pow(1.1f, LevelManager.CurrentLevel);
    }

    private void CalculateFillSpeed(float gain)
    {
        var gainPercentage = gain / MAXExperience;

        //Scale fill speed for application start up
        if (gain == 0)
        {
            _experienceFillSpeed = .008f * Mathf.Pow(10, gainPercentage);
            return;
        }

        //Scale fill speed to slow down the higher the percentage gain
        _experienceFillSpeed = (.008f / (gainPercentage * (gainPercentage * 70) * 10)) * Mathf.Pow(10, gainPercentage);

        //Limit how fast and slow exp fill speed can be
        if (_experienceFillSpeed < _MINexperienceFillSpeed)
            _experienceFillSpeed = _MINexperienceFillSpeed;

        if (_experienceFillSpeed > gainPercentage)
            _experienceFillSpeed = gainPercentage;

        #region DebugLogs

#if DEBUG_LOG
        Debug.Log("Fill speed: " + _experienceFillSpeed);
#endif

        #endregion
    }

    private void OnExperienceUpdate(float experienceGain = 0)
    {
        if (experienceGain > 0)
        {
            AnimateExpText(experienceGain);
            _expAudio.PlayOneShot(_expAudio.clip);
        }

        CurrentExperience += experienceGain;

        CalculateFillSpeed(experienceGain);

        StartCoroutine(SmoothExperienceIncrease());
    }

    private void AnimateExpText(float gain)
    {
        _expText.text = $"+{gain.ToString()}XP";
        _expAnimator.SetTrigger("Add Exp");
    }

    private IEnumerator LevelUp()
    {
        LevelManager.CurrentLevel++;
        ExperienceGain.UpdateLevel();

        PlayLevelUpEffects();

        foreach (var button in _buttons)
            button.interactable = false;

        CalculateCarryOverExp();
        CalculateMaxExp();

        yield return new WaitForSeconds(1.2f);

        _expImage.fillAmount = 0f;

        yield return new WaitForSeconds(.2f);

        CalculateFillSpeed(_carryOverExp);
        StartCoroutine(SmoothExperienceIncrease());

        yield return _expImage.fillAmount == CurrentExperience / MAXExperience;

        foreach (var button in _buttons)
            button.interactable = true;
    }

    private void PlayLevelUpEffects()
    {
        _lvlAudio.Play();
        _lvlUpText.GetComponent<Animator>().SetTrigger("Level Up");

        StartCoroutine(TriggerGlowEffect());
    }

    private IEnumerator TriggerGlowEffect()
    {
        _expGlowEffect.color = Color.white;

        var glowAlpha = 1f;
        while (glowAlpha > 0)
        {
            _expGlowEffect.color = new Color(1, 1, 1, glowAlpha);
            glowAlpha -= .01f;
            yield return null;
        }
    }

    private void CalculateCarryOverExp()
    {
        if (CurrentExperience > MAXExperience)
            _carryOverExp = CurrentExperience - MAXExperience;

        CurrentExperience = _carryOverExp;
    }

    IEnumerator SmoothExperienceIncrease()
    {
        while (_expImage.fillAmount != 1f)
        {
            if (_expImage.fillAmount != CurrentExperience / MAXExperience)
                if (_expImage.fillAmount < CurrentExperience / MAXExperience)
                    _expImage.fillAmount += _experienceFillSpeed;

            if (_expImage.fillAmount == 1f)
            {
                StartCoroutine(LevelUp());
                yield break;
            }

            yield return null;
        }
    }

    public static void LoadProgressFromSaveFile(SaveProgress progress)
    {
        Instance.CurrentExperience = progress.SavedExperience;
    }

    public void UpdateAfterLoadSaveFile()
    {
        OnExperienceUpdate();
    }
}