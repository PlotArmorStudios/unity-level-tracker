using UnityEngine;

public class SaveProgressButton : MonoBehaviour
{
    public void Save()
    {
        SaveManager.Save();
    }
    public void Load()
    {
        SaveManager.Load();
    }
}
