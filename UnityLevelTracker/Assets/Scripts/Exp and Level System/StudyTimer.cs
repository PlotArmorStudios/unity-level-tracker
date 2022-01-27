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
 
    public static float _10MinTimer { get; private set; }
    public static float _30MinTimer { get; private set; }
    public static float _5MinTimer { get; private set; }
    
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
            _5MinTimer += Time.deltaTime;
            _10MinTimer += Time.deltaTime;
            _30MinTimer += Time.deltaTime;

            UpdateTimerBars();
        }
    }

    public void UpdateTimerBars()
    {
        _5MinTimerBar.fillAmount = _5MinTimer / _5Minutes;
        _10MinTimerBar.fillAmount = _10MinTimer / _10Minutes;
        _30MinTimerBar.fillAmount = _30MinTimer / _30Minutes;
    }

    public void StartStudyTimer()
    {
        _timerOn = true;
        StartCoroutine(FiveMinuteTimer());
        StartCoroutine(TenMinuteTimer());
        StartCoroutine(ThirtyMinuteTimer());
        _startTimerButton.SetActive(false);
        _stopTimerButton.SetActive(true);
    }

    public void StopStudyTimer()
    {
        _timerOn = false;
        StopCoroutine(FiveMinuteTimer());
        StopCoroutine(TenMinuteTimer());
        StopCoroutine(ThirtyMinuteTimer());
        _startTimerButton.SetActive(true);
        _stopTimerButton.SetActive(false);
    }

    private IEnumerator FiveMinuteTimer()
    {
        while (_timerOn)
        {
            yield return _delay;

            if (_5MinTimerBar.fillAmount == 1f)
            {
                _expGain.GainExperience(50);
                _5MinTimer = 0f;
            }
        }
    }


    private IEnumerator TenMinuteTimer()
    {
        while (_timerOn)
        {
            yield return _delay;

            if (_10MinTimerBar.fillAmount == 1f)
            {
                _expGain.GainExperience(100);
                _10MinTimer = 0f;
            }
        }
    }

    private IEnumerator ThirtyMinuteTimer()
    {
        while (_timerOn)
        {
            yield return _delay;

            if (_30MinTimerBar.fillAmount == 1f)
            {
                _expGain.GainExperience(300);
                _30MinTimer = 0f;
            }
        }
    }

    public static void LoadProgressFromSaveFile(SaveProgress progress)
    {
        _5MinTimer = progress.SavedFiveMinuteTimer;
        _10MinTimer = progress.SavedTenMinuteTimer;
        _30MinTimer = progress.SavedThirtyMinuteTimer;
    }
}