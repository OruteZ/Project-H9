using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using TMPro;
using UnityEngine;

public class WorldTileCombatIndexEditor : MonoBehaviour
{
    [SerializeField] private Canvas _editorCanvas;
    [SerializeField] private TMP_InputField _combatMapIndexInputField;
    
    private EditCombatStage _editCmd;
    private List<TileCombatStageInfo> _stageInfos;

    private void Awake()
    {
        _editorCanvas = GetComponent<Canvas>();
        //disable canvas
        _editorCanvas.enabled = false;

        _stageInfos = new();
    }

    public void ShowEditorUI(IEnumerable<TileCombatStageInfo> links)
    {
        var e = links as TileCombatStageInfo[] ?? links.ToArray();
        if(e.IsNullOrEmpty())
        {
            Debug.LogError("No link object");
            return;
        }
        
        //enable canvas
        _editorCanvas.enabled = true;

        LoadStageInfo(e);
    }

    public void SetEditCommand(EditCombatStage cmd)
    {
        _editCmd = cmd;
    }

    public void ApplyStageInfo()
    {
        string combatMapIndex = _combatMapIndexInputField.text;
        
        //if input field text is '-', return;
        if (combatMapIndex == "-")
        {
            Debug.Log("Link index or combat map index is not set");
            return;
        }
        
        if(int.TryParse(combatMapIndex, out int combatMapIndexInt) == false)
        {
            Debug.Log("Combat map index is not integer");
            return;
        }
        
        //apply link index and combat map index
        _editCmd.ApplyIndexes(combatMapIndexInt);
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
    
    private void LoadStageInfo(IEnumerable<TileCombatStageInfo> links)
    {
        _stageInfos = new List<TileCombatStageInfo>(links);
        
        List<int> linkIndexes = new List<int>();
        List<int> combatMapIndexes = new List<int>();
        
        //get link index and combat map index
        foreach (var info in _stageInfos)
        {
            linkIndexes.Add(info.combatStageIndex);
            combatMapIndexes.Add(
                FieldSystem.
                tileSystem.
                GetTile(info.hexPosition).
                combatStageIndex
                );
        }
        
        string combatMapIndex = "";
        if (combatMapIndexes.TrueForAll(x => x == combatMapIndexes[0]))
        {
            combatMapIndex = combatMapIndexes[0].ToString();
        }
        else
        {
            combatMapIndex = "-";
        }
        
        //set input field text
        _combatMapIndexInputField.text = combatMapIndex;
    }
}