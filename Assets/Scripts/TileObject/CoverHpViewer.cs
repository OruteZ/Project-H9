using System;
using TMPro;
using UnityEngine;

public class CoverHpViewer : MonoBehaviour
{
    [SerializeField]
    private CoverableObj coverableObj;

    [SerializeField] 
    private TMP_Text hpText;

    private void Awake()
    {
        if (coverableObj == null)
        {
            throw new ArgumentNullException($"CoverableObj is null : object name {gameObject.name}");
        }
        if (hpText == null)
        {
            throw new ArgumentNullException($"HpText is null : object name {gameObject.name}");
        }
        
        coverableObj.OnHpChanged.AddListener(OnHpChanged);
        
        ResetPosition();
        Reload();
    }

    private void OnHpChanged(int arg0, int arg1)
    {
        Reload();
    }

    private void Reload()
    {
        if (TryGetHpInfo(out int hp, out int maxHp))
        {
            hpText.text = $"{hp}/{maxHp}";
        }
    }

    [ContextMenu("Reset Position")]
    private void ResetPosition()
    {
        transform.SetParent(GameManager.instance.worldCanvas.transform);
        
        if(coverableObj is null) return;
        if(hpText is null) return;
        
        Vector3 textPos = coverableObj.transform.position;
        textPos += Vector3.up * 0.5f;
        hpText.transform.position = textPos;
    }
    
    private bool TryGetHpInfo(out int hp, out int maxHp)
    {
        hp = 0;
        maxHp = 0;
        
        if (coverableObj is null)
        {
            Debug.LogError("CoverableObj is null");
            return false;
        }

        hp = coverableObj.GetCurrentHp();
        maxHp = coverableObj.GetMaxHp();
        return true;
    }
}
