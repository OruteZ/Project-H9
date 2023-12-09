using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WorldLinkInfoEditor : MonoBehaviour
{
    [SerializeField] private Canvas _editorCanvas;
    [SerializeField] private TMP_InputField _linkIndexInputField;
    [SerializeField] private TMP_InputField _combatMapIndexInputField;
    
    private EditLinkInfo _editCmd;
    private List<Link> _linkObjects;

    private void Awake()
    {
        _editorCanvas = GetComponent<Canvas>();
        //disable canvas
        _editorCanvas.enabled = false;

        _linkObjects = new();
    }

    public void ShowEditorUI(IEnumerable<Link> links)
    {
        
        if(links == null || !links.Any())
        {
            Debug.Log("No link object");
            return;
        }
        
        //enable canvas
        _editorCanvas.enabled = true;

        LoadLinkInfo(links);
    }

    public void SetEditCommand(EditLinkInfo cmd)
    {
        _editCmd = cmd;
    }

    public void ApplyLinkInfo()
    {
        //read input field text
        string linkIndex = _linkIndexInputField.text;
        string combatMapIndex = _combatMapIndexInputField.text;
        
        //if input field text is '-', return;
        if (linkIndex == "-" || combatMapIndex == "-")
        {
            Debug.Log("Link index or combat map index is not set");
            return;
        }
        
        //input값 정수인지 체크 + 파싱
        if (!int.TryParse(linkIndex, out int linkIndexInt) || !int.TryParse(combatMapIndex, out int combatMapIndexInt))
        {
            Debug.Log("Link index or combat map index is not integer");
            return;
        }
        
        //apply link index and combat map index
        _editCmd.ApplyIndexes(linkIndexInt, combatMapIndexInt);
        DisableUI();
    }
    
    public void Cancel()
    {
        DisableUI();
    }

    private void DisableUI()
    {
        //disable canvas
        _editorCanvas.enabled = false;
    }
    
    private void LoadLinkInfo(IEnumerable<Link> links)
    {
        _linkObjects = new List<Link>(links);
        
        List<int> linkIndexes = new List<int>();
        List<int> combatMapIndexes = new List<int>();
        
        //get link index and combat map index
        foreach (var link in _linkObjects)
        {
            linkIndexes.Add(link.linkIndex);
            combatMapIndexes.Add(link.combatMapIndex);
        }
        
        string linkIndex = "";
        string combatMapIndex = "";
        
        //if all index is same, convert index to string
        //else set to '-'
        if (linkIndexes.TrueForAll(x => x == linkIndexes[0]))
        {
            linkIndex = linkIndexes[0].ToString();
        }
        else
        {
            linkIndex = "-";
        }
        
        if (combatMapIndexes.TrueForAll(x => x == combatMapIndexes[0]))
        {
            combatMapIndex = combatMapIndexes[0].ToString();
        }
        else
        {
            combatMapIndex = "-";
        }
        
        //set input field text
        _linkIndexInputField.text = linkIndex;
        _combatMapIndexInputField.text = combatMapIndex;
    }
}