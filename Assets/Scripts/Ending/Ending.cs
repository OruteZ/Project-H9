using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    public void BackToTitle()
    {
        // Find all GameObjects with DontDestroyOnLoad
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.scene.name == null)
            {
                // Destroy the GameObject
                Destroy(obj);
            }
        }
        
        SceneManager.LoadScene($"TitleScene");
    }
}