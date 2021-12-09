using System;
using UnityEngine;

public class ExperienceGain : MonoBehaviour
{
    public static event Action<float> OnExperienceGain;
    public static event Action OnUpdateLevel;
    
    public void GainExperience(float experience)
    {
        OnExperienceGain?.Invoke(experience);
        OnUpdateLevel?.Invoke();
    }

    public static void UpdateLevel()
    {
        OnUpdateLevel?.Invoke();
    }
}
