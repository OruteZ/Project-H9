using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TownUI : UISystem
{
    [SerializeField] private GameObject _ammunitionWindow;
    [SerializeField] private GameObject _ammunitionText;
    [SerializeField] private GameObject _saloonWindow;
    [SerializeField] private GameObject _saloonText;
    [SerializeField] private GameObject _sheriffWindow;
    [SerializeField] private GameObject _sheriffText;

    private Vector3Int _currentInteractPosition = Vector3Int.zero;
    private int _currentTownIndex = 0;
    private Town.BuildingType _currentBuildingType = Town.BuildingType.NULL;

    private const int SALOON_REST_COST = 1;

    [SerializeField] private GameObject _bountyUIElements;
    private enum TownNameIndex 
    {
        Linsdale,
        Westville
    }

    public bool isTownUIOpened { get; private set; }

    private TownIconPool _iconPool = null;
    [SerializeField] private GameObject _iconContainer;
    [SerializeField] private GameObject _doorIcon;

    private void Awake()
    {
        _iconPool = new TownIconPool();
        _iconPool.Init("Prefab/Town Icon", _iconContainer.transform, 0);
        _doorIcon.SetActive(false);

        PlayerEvents.OnPlayerEnterTown.AddListener((p, i, t) => { SetCurrentTownInfo(p, i, t); });
        CloseUI();
    }
    private void Start()
    {
        Player p = FieldSystem.unitSystem.GetPlayer();
        PlayerEvents.OnMovedPlayer.AddListener((pos) => { /*if (p.GetSelectedAction().GetActionType() == ActionType.Move)*/ CheckPlayerInTown(); });
        UIManager.instance.onTSceneChanged.AddListener((gs) => { _doorIcon.SetActive(false); });
    }

    public void OpenAmmunitionWindow(int townIndex)
    {
        _ammunitionWindow.SetActive(true);
    }
    public void OpenSaloonWindow(int townIndex)
    {
        _saloonText.GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[1101];
        _saloonWindow.SetActive(true);
    }
    public void OpenSheriffWindow(int townIndex)
    {
        SetBountyUI(townIndex);
        _sheriffText.GetComponent<TextMeshProUGUI>().text = UIManager.instance.UILocalization[1102];
        _sheriffWindow.SetActive(true);
    }

    #region Ammunition

    #endregion

    #region Saloon
    public void ClickTakeARestButton() 
    {
        Inventory inven = GameManager.instance.playerInventory;
        Player player = FieldSystem.unitSystem.GetPlayer();
        if (player == null)
        {
            Debug.Log("player가 존재하지 않습니다...이럴 일이 있나?");
            CloseUI();
            return;
        }
        if (inven.GetGold() < SALOON_REST_COST) 
        {
            Debug.Log("재화가 부족합니다.");
            CloseUI();
            return;
        }

        inven.AddGold(-1);
        player.stat.Recover(StatType.CurHp, player.stat.GetStat(StatType.MaxHp), out var appliedValue);
        FieldSystem.turnSystem.EndTurn();
        CloseUI();
    }
    #endregion

    #region Sherrif
    public void SetBountyUI(int townIndex) 
    {
        List<QuestInfo> subQuests = new();

        foreach (var i in GameManager.instance.Quests)
        {
            if (i.QuestSheriff == ((TownNameIndex)townIndex).ToString())
            {
                subQuests.Add(i);
            }
        }
        if (subQuests.Count != _bountyUIElements.transform.childCount)
        {
            Debug.LogError("Unexpected number of selectable quests");
            return;
        }

        for (int i = 0; i < _bountyUIElements.transform.childCount; i++)
        {
            _bountyUIElements.transform.GetChild(i).gameObject.SetActive(!subQuests[i].IsCleared);
            if (!subQuests[i].IsCleared)
            {
                _bountyUIElements.transform.GetChild(i).GetComponent<BountyUIElement>().SetBountyUIElement(subQuests[i]);
            }
        }
    }
    public void StartBounty(QuestInfo qInfo) 
    {
        foreach (var q in GameManager.instance.Quests) 
        {
            if (q.Index == qInfo.Index) 
            {
                q.OnConditionEventOccured();
            }
        }
        SetBountyUI(_currentTownIndex);
    }
    #endregion

    //Invoke when player stop(start to move)
    private void CheckPlayerInTown()
    {
        Player player = FieldSystem.unitSystem.GetPlayer();
        if (player == null)
        {
            Debug.Log("player가 존재하지 않습니다.");
            CloseUI();
            return;
        }

        if (player.hexPosition != _currentInteractPosition)
        {
            _doorIcon.SetActive(false);
            UIManager.instance.SetUILayer(1);
            CloseUI();
            return;
        }
        _doorIcon.SetActive(true);

    }

    //Invoke when player collide with town tile
    private void SetCurrentTownInfo(Vector3Int pos, int index, Town.BuildingType type) 
    {
        _currentInteractPosition = pos;
        _currentTownIndex = index;
        _currentBuildingType = type;
    }

    public override void CloseUI()
    {
        isTownUIOpened = false;
        _ammunitionWindow.SetActive(false);
        _saloonWindow.SetActive(false);
        _sheriffWindow.SetActive(false);
        base.CloseUI();
    }

    public void AddTownIcon(Vector3Int pos, Town.BuildingType type) 
    {
        Vector3Int iconHexPos = new Vector3Int(pos.x, pos.y - 2, pos.z + 2);

        if (_iconPool.Find(iconHexPos) != null) return;
        var target = _iconPool.Set();
        target.Init(iconHexPos, type);

    }
    void Update()
    {
        bool isWorldScene = SceneManager.GetActiveScene().name == "WorldScene" || SceneManager.GetActiveScene().name == "UITestScene";
        _iconContainer.SetActive(isWorldScene);
        if (!isWorldScene) return;

        _iconPool.Update();

        Tile townTile = FieldSystem.tileSystem.GetTile(_currentInteractPosition);
        Vector3Int doorTileHexPos = new Vector3Int(_currentInteractPosition.x, _currentInteractPosition.y - 1, _currentInteractPosition.z + 1);
        Tile doorTile = FieldSystem.tileSystem.GetTile(doorTileHexPos);
        if (townTile == null || doorTile == null) return;
        if (!townTile.inSight || !doorTile.inSight) return;
        Vector3 screenPos = Camera.main.WorldToScreenPoint((townTile.transform.position + doorTile.transform.position) / 2);
        _doorIcon.GetComponent<RectTransform>().position = screenPos;
    }

    public void OnClickDoorIcon()
    {
        Player player = FieldSystem.unitSystem.GetPlayer();
        if (player == null)
        {
            Debug.LogError("player가 존재하지 않습니다.");
            return;
        }
        if (player.IsBusy() || isTownUIOpened)
        {
            CloseUI();
            return;
        }
        UIManager.instance.SetUILayer(2);
        isTownUIOpened = true;
        switch (_currentBuildingType)
        {
            case Town.BuildingType.Ammunition:
                {
                    OpenAmmunitionWindow(_currentTownIndex);
                    break;
                }
            case Town.BuildingType.Saloon:
                {
                    OpenSaloonWindow(_currentTownIndex);
                    break;
                }
            case Town.BuildingType.Sheriff:
                {
                    OpenSheriffWindow(_currentTownIndex);
                    break;
                }

        }
    }
}
