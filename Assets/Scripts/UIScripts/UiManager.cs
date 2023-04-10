using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{

    [SerializeField] private Canvas WorldCanvas;
    [SerializeField] private Canvas BattleCanvas;
    [SerializeField] private Canvas CharacterCanvas;
    [SerializeField] private Canvas SkillCanvas;
    [SerializeField] private Canvas OptionCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenCharacterWindow() 
    {
        CharacterCanvas.enabled = true;
    }
    public void CloseCharacterWindow()
    {
        CharacterCanvas.enabled = false;
    }
    public void OpenSkillWindow()
    {
        SkillCanvas.enabled = true;
    }
    public void CloseSkillWindow()
    {
        SkillCanvas.enabled = false;
    }
    public void OpenOptionWindow()
    {
        OptionCanvas.enabled = true;
    }
    public void CloseOptionWindow()
    {
        OptionCanvas.enabled = false;
    }
}
