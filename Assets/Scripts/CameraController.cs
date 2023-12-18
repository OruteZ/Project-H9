using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    private static CameraController _instance;
    
    public float normalSpeed;
    public float fastSpeed;
    
    public float movementSpeed;
    public float movementTime;

    private float _cameraDegree;

    public Vector3 newPosition;

    [Space(1)] public float lookAtOffset;

    private void Awake()
    {
        _instance = this;
        _cameraDegree = transform.GetChild(0).localRotation.eulerAngles.x;
        
        FieldSystem.onStageAwake.AddListener(OnCombatAwake);
        FieldSystem.turnSystem.onTurnChanged.AddListener(OnTurnChanged);
        
        var tsf = transform;
        newPosition = tsf.position;
    }
    private void Update()
    {
       HandleMoveEvent();
    }

    private void HandleMoveEvent()
    {
        Vector3 movePosition = MovingPosition();

        transform.position = movePosition + _shakeOffset;
    }

    private Vector3 MovingPosition()
    {
        Vector3 direction = Vector3.zero;

        movementSpeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : normalSpeed;

        if (Input.GetKey(KeyCode.W)) direction += transform.forward;
        if (Input.GetKey(KeyCode.A)) direction -= transform.right;
        if (Input.GetKey(KeyCode.D)) direction += transform.right;
        if (Input.GetKey(KeyCode.S)) direction -= transform.forward;
        direction.Normalize();
        newPosition += direction * (movementSpeed * Time.deltaTime);

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
        
        var movePosition = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        return movePosition;
    }

    private void LookAtUnit(Unit unit)
    {
        var worldPos = Hex.Hex2World(unit.hexPosition);
        worldPos.y = transform.position.y;
        worldPos.z += -10f;

        newPosition = worldPos;
    }

    private void LookAtPlayer()
    {
        var player = FieldSystem.unitSystem.GetPlayer();
        if(player is null) Debug.LogError("Player is null");
        
        LookAtUnit(player);
    }

    private void OnCombatAwake()
    {
        if (GameManager.instance.CompareState(GameState.Combat))
        {
            LookAtPlayer();
            FieldSystem.unitSystem.GetPlayer().onHit.AddListener((u, i) => ShakeCamera());
        }
    }

    private void OnTurnChanged()
    {
        //if combat state, look at turn owner
        if (GameManager.instance.CompareState(GameState.Combat))
            LookAtUnit(FieldSystem.turnSystem.turnOwner);   

    }

    #region SHAKE
    
    [Header("Damping")]
    public AnimationCurve dampingCurve;

    public float dampingValue, dampingDuration;
    private Vector3 _shakeOffset;
    public static void ShakeCamera()
    {
        Vector3 direction;
        
        IEnumerator Coroutine()
        {
            direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            direction.Normalize();

            float t = _instance.dampingDuration;
            while ((t -= Time.deltaTime) > 0)
            {
                _instance._shakeOffset = direction * (_instance.dampingCurve.Evaluate(t) * _instance.dampingValue);
                yield return true;
            }

            _instance._shakeOffset = Vector3.zero;
        }

        _instance.StartCoroutine(Coroutine());
    }
    
    #endregion
    
    #region LIMIT
    #if UNITY_EDITOR

    public float maxZ, minZ, maxX, minX;

    private void SetLeftUpLimit()
    {
        maxZ = transform.position.z;
        minX = transform.position.x;
    }
    
    private void SetRightDownLimit()
    {
        maxX = transform.position.x;
        minZ = transform.position.z;
    }

    #endif
    #endregion

    public static float GetCamDeg()
    {
        return _instance._cameraDegree;
    }
}
