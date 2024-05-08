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

    private void Awake()
    {
        _iconPool = new TownIconPool();
        _iconPool.Init("Prefab/Town Icon", _iconContainer.transform, 0);

        UIManager.instance.onPlayerEnterTown.AddListener((p, i, t) => { SetCurrentTownInfo(p, i, t); });
        CloseUI();
    }
    private void Start()
    {
        Player p = FieldSystem.unitSystem.GetPlayer();
        PlayerEvents.OnMovedPlayer.AddListener((pos) => { if (p.GetSelectedAction().GetActionType() == ActionType.Move) CheckPlayerInTown(); });
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
    //Invoke when player start idle action
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
            UIManager.instance.SetUILayer(1);
            CloseUI();
            _previousBuildingType = Town.BuildingType.NULL;
            return;
        }
        if (_currentBuildingType == _previousBuildingType) return;
        if (UIManager.instance.gameSystemUI.conversationUI.isConverstating) return;
        IUnitAction selectedAction = player.GetSelectedAction();
        if (selectedAction is not MoveAction || ((MoveAction)selectedAction).isThereAPathLeft()) return;

        //player.GetSelectedAction().ForceFinish();

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
        Vector3Int iconPos = new Vector3Int(pos.x, pos.y - 2, pos.z + 2);
        Tile t = FieldSystem.tileSystem.GetTile(iconPos);

        if (t == null) 
        {
            Debug.LogError("Town Icon을 띄울 타일을 찾을 수 없습니다.");
        }
        var target = _iconPool.Set();
        if (target == null)
        {
            Debug.LogError("Town Icon Pool is Empty");
            return;
        }
        target.Init(t, type);
    }
    
    void Update()
    {
        bool isWorldScene = SceneManager.GetActiveScene().name == "WorldScene" || SceneManager.GetActiveScene().name == "UITestScene";
        _iconContainer.SetActive(isWorldScene);
        if (!isWorldScene) return;

        _iconPool.Update();
    }
}
