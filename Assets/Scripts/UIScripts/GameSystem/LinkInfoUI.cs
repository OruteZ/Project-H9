using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LinkInfoUI : UISystem
{
    [SerializeField] private GameObject _linkInfoWindow;

    [SerializeField] private GameObject _linkNameText;
    [SerializeField] private GameObject _linkInfoElements;
    [SerializeField] private GameObject _enemyStatTooltip;
    [SerializeField] private GameObject _enemyWeaponTooltip;

    [SerializeField] private GameObject _linkInfoElementPrefab;

    private LinkDatabase _linkDatabase;
    private Link _currentLink = null;

    private int _uiCloseCount = 0;

    private const int X_POSITION_CORRECTION = 40;

    private void Start()
    {
        _linkInfoWindow.SetActive(false);
           _linkDatabase ??= Resources.Load<LinkDatabase>($"Database/LinkDatabase");
        if (_linkDatabase == null)
        {
            Debug.LogError("LinkDatabase is null");
            return;
        }
    }

    private void Update()
    {
        if (!GameManager.instance.CompareState(GameState.World))
        {
            _uiCloseCount = 0;
            _currentLink = null;
            _linkInfoWindow.SetActive(false);
            return;
        }
        if (_linkInfoWindow.activeSelf && _currentLink != null) 
        {
            SetLinkInfoUIPosition(_currentLink.transform.position);
        }

        if (!IsMouseOverOnLink(out Vector3Int linkHexPos)) 
        {
            TryCloseLinkInfoUI();
            return;
        }

        Player player = FieldSystem.unitSystem.GetPlayer();
        List<TileObject> tObjects = FieldSystem.tileSystem.GetTileObject(linkHexPos);
        if (player is null || tObjects is null) return;
        Link link = (Link)tObjects.Find(obj => obj.gameObject.TryGetComponent(out Link l));
        if (link is null || link == _currentLink) return;

        if (link.IsVisible())
        {
            SetLinkInfoUI(link);
        }

    }
    public void SetLinkInfoUI(Link link)
    {
        _currentLink = link;
        _linkInfoWindow.SetActive(true);
        LinkData lData = _linkDatabase.GetData(link.linkIndex);

        SetLinkInfoUIPosition(link.transform.position);

        _linkNameText.GetComponent<TextMeshProUGUI>().text = "WANTED : " + _linkDatabase.GetLinkName(lData.groupNameIndex);

        List<Tuple<EnemyData, int>> data = new List<Tuple<EnemyData, int>>();
        foreach (int dataIndex in lData.combatEnemy)
        {
            EnemyData eData = FieldSystem.unitSystem.GetEnemyData(dataIndex);
            int search = data.FindIndex(obj => obj.Item1.index == eData.index);
            if (search < 0)
            {
                data.Add(new Tuple<EnemyData, int>(eData, 1));
            }
            else 
            {
                data[search] = new Tuple<EnemyData, int>(data[search].Item1, data[search].Item2 + 1);
            }
        }

        for (int i = 0; i < _linkInfoElements.transform.childCount - 1; i++)
        {
            if (i < data.Count)
            {
                _linkInfoElements.transform.GetChild(i + 1).gameObject.SetActive(true);
                _linkInfoElements.transform.GetChild(i + 1).GetComponent<EnemyInfoUI>().SetEnemyInfoUI(data[i].Item1, data[i].Item2, _enemyStatTooltip, _enemyWeaponTooltip);
            }
            else 
            {
                _linkInfoElements.transform.GetChild(i + 1).gameObject.SetActive(false);
            }
        }

    }
    private void SetLinkInfoUIPosition(Vector3 pos) 
    {
        Vector3 uiPos = Camera.main.WorldToScreenPoint(pos);
        float xCorrection = (X_POSITION_CORRECTION + _linkInfoWindow.GetComponent<RectTransform>().sizeDelta.x / 2);
        float uiRightEnd = uiPos.x + (xCorrection + _linkInfoWindow.GetComponent<RectTransform>().sizeDelta.x / 2) * UIManager.instance.GetCanvasScale();
        float screenRightEnd = GetComponent<RectTransform>().sizeDelta.x * GetComponent<RectTransform>().localScale.x;
        if (uiRightEnd > screenRightEnd) 
        {
            xCorrection = -xCorrection;
        }
        uiPos.x += xCorrection * UIManager.instance.GetCanvasScale();
        _linkInfoWindow.GetComponent<RectTransform>().position = uiPos;
    }

    private static bool IsMouseOverOnLink(out Vector3Int pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool isSuccessRaycast = Physics.Raycast(ray, out var hit, float.MaxValue, layerMask: LayerMask.GetMask("Link"));
        if (isSuccessRaycast)
        {
            Link link = hit.collider.GetComponent<Link>();
            pos = link.hexPosition;
            return true;
        }

        pos = Vector3Int.zero;
        return false;
    }
    private void TryCloseLinkInfoUI()
    {
        GraphicRaycaster gr = GetComponent<GraphicRaycaster>();
        PointerEventData ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);
        foreach (var r in results)
        {
            if (r.gameObject.layer == LayerMask.NameToLayer("UI3")) return;
        }

        _uiCloseCount++;
        if (_uiCloseCount < 120) return;

        _uiCloseCount = 0;
        _currentLink = null;
        _linkInfoWindow.SetActive(false);
    }
}
