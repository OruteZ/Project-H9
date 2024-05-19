using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TownUI : UISystem
{
    [SerializeField] private GameObject _AmmunitionWindow;
    [SerializeField] private GameObject _SaloonWindow;
    [SerializeField] private GameObject _SheriffWindow;

    private Vector3Int _currentInteractPosition = Vector3Int.zero;
    private int _currentTownIndex = 0;
    private Town.BuildingType _currentBuildingType = Town.BuildingType.NULL;

    private Town.BuildingType _previousBuildingType = Town.BuildingType.NULL;

    private const int SALOON_REST_COST = 1;

    public bool isTownUIOpened { get; private set; }

    private TownIconPool _iconPool = null;
    [SerializeField] private GameObject _iconContainer;
    [SerializeField] private GameObject _doorIcon;

    private void Awake()
    {
        _iconPool = new TownIconPool();
        _iconPool.Init("Prefab/Town Icon", _iconContainer.transform, 0);
        _doorIcon.SetActive(false);

        UIManager.instance.onPlayerEnterTown.AddListener((p, i, t) => { SetCurrentTownInfo(p, i, t); });
        CloseUI();
    }
    private void Start()
    {
        Player p = FieldSystem.unitSystem.GetPlayer();
        PlayerEvents.OnMovedPlayer.AddListener((pos) => { /*if (p.GetSelectedAction().GetActionType() == ActionType.Move)*/ CheckPlayerInTown(); });
        UIManager.instance.onTSceneChanged.AddListener((gs) => { _doorIcon.SetActive(gs == GameState.World); });
    }

    public void OpenAmmunitionWindow(int townIndex)
    {
        _AmmunitionWindow.SetActive(true);
    }
    public void OpenSaloonWindow(int townIndex)
    {
        _SaloonWindow.SetActive(true);
    }
    public void OpenSheriffWindow(int townIndex)
    {
        _SheriffWindow.SetActive(true);
    }

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
            _previousBuildingType = Town.BuildingType.NULL;
            return;
        }
        _doorIcon.SetActive(true);
        //if (_currentBuildingType == _previousBuildingType) return;
        //if (UIManager.instance.gameSystemUI.conversationUI.isConverstating) return;

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
        _AmmunitionWindow.SetActive(false);
        _SaloonWindow.SetActive(false);
        _SheriffWindow.SetActive(false);
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
