using UnityEngine;

public class SaveLoadProgressButton : MonoBehaviour
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
