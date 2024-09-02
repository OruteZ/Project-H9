using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpeechUI : UISystem
{
    [SerializeField] private GameObject _enemyTextContainer;
    private UnitSpeechPool _pool = null;

    private void Start()
    {
        if (_pool == null)
        {
            _pool = new UnitSpeechPool();
            _pool.Init("Prefab/UI/Unit Speech Text", _enemyTextContainer.transform, 0);
        }
    }
    public void SetUnitSpeechText(Unit unit, string str)
    {
        List<string> s = new();
        s.Add(str);
        SetUnitSpeechText(unit, s);
    }
    public void SetUnitSpeechText(Unit unit, List<string> str)
    {
        if (_pool == null) return;
        var wrapper = _pool.Set();
        wrapper.Instance.GetComponent<UnitSpeechElement>().SetUnitSpeech(unit, str);
    }
    public void ClearUnitSpeechText(GameObject ui)
    {
        _pool.Reset(ui);
    }
}
