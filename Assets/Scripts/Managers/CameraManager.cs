using System.Collections.Generic;
using Cinemachine;
using Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    private Transform _cameraParentTransform => transform;
    private readonly Dictionary<Unit, UnitCamera> _unitCameras = new ();
    [SerializeField] private UnitCamera _currentUnitCamera;
    [SerializeField] private CinemachineBrain _brain;
    
    public WorldCamera worldCamera;
    public GameObject unitCameraPrefab;
    public QuestViewpoint questViewpoint;
    
    public void CreateUnitCamera(Unit target)
    {
        if (target is null)
        {
            Debug.LogWarning("Target is null");
            return;
        }
        
        UnitCamera uCam = Instantiate(unitCameraPrefab, _cameraParentTransform).GetComponent<UnitCamera>();
        _unitCameras.Add(target, uCam);
        uCam.SetOwner(target);
    }

    public void LookAtForcely(Unit target)
    {
        LookAt(target);
    }
    private void LookAt(Unit target)
    {
        if (_unitCameras.TryGetValue(target, out UnitCamera unitCamera))
        {
            if (_currentUnitCamera != null)
            {
                _currentUnitCamera.SetPriority(0);
            }
            unitCamera.SetPriority(10);
            _currentUnitCamera = unitCamera;
            worldCamera.SetPosition( _currentUnitCamera.GetUnit().transform.position);
        }
    }

    public UnitCamera GetCamera(Unit unit)
    {
        UnitCamera ret = _unitCameras.GetValueOrDefault(unit);
        return ret;
    }

    protected override void Awake()
    {
        base.Awake();
        if (this == null) return;
        
        FieldSystem.onStageAwake.AddListener(OnStageAwake);
    }
    
    public void LookWorldCamera()
    {
        if (_currentUnitCamera != null)
        {
            worldCamera.SetPosition(_currentUnitCamera.GetUnit().transform.position);
            _currentUnitCamera.SetPriority(0);
        }
        _currentUnitCamera = null;
    }
    
    private void Init()
    {
        //set all camera priority to 0
        foreach (var unitCamera in _unitCameras.Values)
        {
            unitCamera.SetPriority(0);
        }
        worldCamera.SetPriority(5);
        _currentUnitCamera = null;

        if (GameManager.instance.CompareState(GameState.COMBAT))
        {
            SetCombatCamOption();
        }
        else if (GameManager.instance.CompareState(GameState.WORLD))
        {
            SetWorldCamOption();
        }
    }

    private void SetWorldCamOption()
    {
        //set orthographic camera
        if (Camera.main != null) Camera.main.orthographic = true;
        worldCamera.SetPosition(FieldSystem.unitSystem.GetPlayer().transform.position);
    }

    private void SetCombatCamOption()
    {
        if (Camera.main != null) Camera.main.orthographic = false;
        
        UnitCamera playerCam= GetCamera(FieldSystem.unitSystem.GetPlayer());
        if(playerCam == null) return;
        
        worldCamera.SetPosition(playerCam.transform.position);
    }
    
    public void ShakeCamera(float amplitude, float frequency, float duration)
    {
        //유닛 카메라일 때 흔들림이 안되는 것 같음
        if (_currentUnitCamera != null)
        {
            _currentUnitCamera.ShakeCamera(amplitude, frequency, duration);
        }
        else
        {
            worldCamera.ShakeCamera(amplitude, frequency, duration);
        }
    }
    
    #region EVENTS
    private void OnStageAwake()
    {
        Init();
        
        FieldSystem.turnSystem.onTurnChanged.AddListener(OnTurnStarted);
        FieldSystem.onCombatFinish.AddListener(OnCombatFinished);
        
        Unit player = FieldSystem.unitSystem.GetPlayer();
        if (player != null)
        {
            player.onBusyChanged.AddListener(OnPlayerBusyChanged);
            LookAt(player);
        }
        
    }

    private void OnTurnStarted()
    {
        if (FieldSystem.turnSystem.turnOwner is Player)
        {
            LookWorldCamera();
            return;
        }
        
        var turnOwner = FieldSystem.turnSystem.turnOwner;
        LookAt(turnOwner.meshVisible ? turnOwner : FieldSystem.unitSystem.GetPlayer());
    }
    private void OnCombatFinished(bool win)
    {
        FieldSystem.turnSystem.onTurnChanged.RemoveListener(OnTurnStarted);
        FieldSystem.onCombatFinish.RemoveListener(OnCombatFinished);
    }
    
    private void OnPlayerBusyChanged()
    {
        var player = FieldSystem.unitSystem.GetPlayer();
        if (player.IsBusy())
        {
            LookAt(player);
        }
        else
        {
            LookWorldCamera();
        }
    }

    #endregion
    
    #region QUEST_VIEWPOINT
    public void SetQuestViewpoint(Vector3Int hexPosition)
    {
        questViewpoint.SetPosition(hexPosition);
    }
    
    public void ViewQuest(float duration = -1)
    {
        questViewpoint.View(duration);
    }
    
    #endregion
}