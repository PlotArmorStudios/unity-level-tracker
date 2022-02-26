using System;
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
    [SerializeField] private AudioSource _lvlAudio;
    [SerializeField] private AudioSource _expAudio;
    [SerializeField] private Image _expGlowEffect;

    public static ExperienceBar Instance;
    public float CurrentExperience;

    public float _MAXExperience = 1000f;
    private float _experienceChange;
    private float _carryOverExp;

    private Animator _expAnimator;
    private Button[] _buttons;

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
        _MAXExperience = 1000f * Mathf.Pow(1.1f, LevelManager.CurrentLevel);
    }

    private void CalculateFillSpeed(float gain)
    {
        var gainPercentage = gain / _MAXExperience;

        if (gain == 0)
        {
            _experienceFillSpeed = .008f * Mathf.Pow(10, gainPercentage);
            return;
        }

        _experienceFillSpeed = (.008f / (gainPercentage * (gainPercentage * 70) * 10)) * Mathf.Pow(10, gainPercentage);

        Debug.Log("Fill speed: " + _experienceFillSpeed);
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
        StartCoroutine(TriggerGlowEffect());
        CalculateCarryOverExp();
        CalculateMaxExp();
        CurrentExperience = _carryOverExp;

        yield return new WaitForSeconds(.2f);

        _expImage.fillAmount = 0f;

        yield return new WaitForSeconds(.2f);

        StartCoroutine(SmoothExperienceIncrease());

        yield return _expImage.fillAmount == CurrentExperience / _MAXExperience;

        foreach (var button in _buttons)
        {
            button.interactable = true;
        }
    }

    private IEnumerator TriggerGlowEffect()
    {
        _expGlowEffect.color = Color.white;

        var glowAlpha = 1f;
        while (glowAlpha > 0)
        {
            _expGlowEffect.color = new Color(1, 1, 1, glowAlpha);
            glowAlpha -= .001f;
            yield return null;
        }
    }

    private void CalculateCarryOverExp()
    {
        if (CurrentExperience > _MAXExperience)
            _carryOverExp = CurrentExperience - _MAXExperience;
        Debug.Log("Carry over exp is: " + _carryOverExp);
    }

    IEnumerator SmoothExperienceIncrease()
    {
        while (_expImage.fillAmount != CurrentExperience / _MAXExperience)
        {
            ///if (_expImage.fillAmount > CurrentExperience / _MAXExperience)
            //  _expImage.fillAmount -= _experienceFillSpeed;

            if (_expImage.fillAmount < CurrentExperience / _MAXExperience)
                _expImage.fillAmount += _experienceFillSpeed;

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