using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    public void BackToTitle()
    {
        // Find all GameObjects with DontDestroyOnLoad
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            Destroy(obj);
        }
        
        SceneManager.LoadScene($"TitleScene");
    }
}