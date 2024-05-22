using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    public void BackToTitle()
    {
        SceneManager.LoadScene($"Title Scene");
    }
}