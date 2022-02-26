using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StudyTimer : MonoBehaviour
{
    [SerializeField] private GameObject _startTimerButton;
    [SerializeField] private GameObject _stopTimerButton;
    [SerializeField] private ExperienceGain _expGain;
    [SerializeField] private Image _5MinTimerBar;
    [SerializeField] private Image _10MinTimerBar;
    [SerializeField] private Image _30MinTimerBar;
    [SerializeField] private float _experienceUpdateFrequency = 100f;
 
    public static float TenMinTimer { get; private set; }
    public static float ThirtyMinTimer { get; private set; }
    public static float FiveMinTimer { get; private set; }
    
    private float _5Minutes = 300f;
    private float _10Minutes = 600f;
    private float _30Minutes = 1800f;

    private bool _timerOn;

    private WaitForSeconds _delay;
    
    private void Start()
    {
        _delay = new WaitForSeconds(_experienceUpdateFrequency);
        UpdateTimerBars();
    }

    private void Update()
    {
        if (_timerOn)
        {
            FiveMinTimer += Time.deltaTime;
            TenMinTimer += Time.deltaTime;
            ThirtyMinTimer += Time.deltaTime;

            UpdateTimerBars();
        }

        AddExperienceOnFull();
    }

    private void AddExperienceOnFull()
    {
        if (_5MinTimerBar.fillAmount == 1f)
        {
            _expGain.GainExperience(50);
            FiveMinTimer = 0f;
        }

        if (_10MinTimerBar.fillAmount == 1f)
        {
            _expGain.GainExperience(100);
            TenMinTimer = 0f;
        }

        if (_30MinTimerBar.fillAmount == 1f)
        {
            _expGain.GainExperience(300);
            ThirtyMinTimer = 0f;
        }
    }

    public void UpdateTimerBars()
    {
        _5MinTimerBar.fillAmount = FiveMinTimer / _5Minutes;
        _10MinTimerBar.fillAmount = TenMinTimer / _10Minutes;
        _30MinTimerBar.fillAmount = ThirtyMinTimer / _30Minutes;
    }

    public void StartStudyTimer()
    {
        _timerOn = true;
        _startTimerButton.SetActive(false);
        _stopTimerButton.SetActive(true);
    }

    public void StopStudyTimer()
    {
        _timerOn = false;
        _startTimerButton.SetActive(true);
        _stopTimerButton.SetActive(false);
    }

    public static void LoadProgressFromSaveFile(SaveProgress progress)
    {
        FiveMinTimer = progress.SavedFiveMinuteTimer;
        TenMinTimer = progress.SavedTenMinuteTimer;
        ThirtyMinTimer = progress.SavedThirtyMinuteTimer;
    }
}