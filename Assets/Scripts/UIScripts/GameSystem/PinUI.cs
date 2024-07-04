using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PinUI : UISystem
{
    [SerializeField] private GameObject _pinImage;
    private WorldCamera worldCamera;

    private bool _isTracking = false;
    private Vector3 _targetPos = Vector3.zero;

    private const int PIN_POSITION_Y_CORRECTION = 30;
    public Vector3Int targetHexPos { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        _pinImage.SetActive(false);

        worldCamera = CameraManager.instance.worldCamera;
    }
    void Update()
    {
        if (_isTracking)
        {
            _pinImage.SetActive(SceneManager.GetActiveScene().name == "WorldScene" || SceneManager.GetActiveScene().name == "UITestScene");
            Vector3 screenPos = Camera.main.WorldToScreenPoint(_targetPos);
            var tileObjects = FieldSystem.tileSystem.GetTileObject(targetHexPos);
            foreach (var obj in tileObjects)
            {
                if (obj is Link)
                {
                    screenPos.y += PIN_POSITION_Y_CORRECTION;
                    break;
                }
            }
            Vector3 correctedPos = ScreenOverCorrector.GetCorrectedUIPositionWithoutConsideringUISize(GetComponent<Canvas>(), screenPos, _pinImage.GetComponent<RectTransform>().sizeDelta, _pinImage.GetComponent<RectTransform>().pivot);

            if (screenPos != correctedPos)
            {
                _pinImage.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, Quaternion.FromToRotation(Vector3.up, new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2) - screenPos).eulerAngles.z);
            }
            else
            {
                _pinImage.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
            }
            _pinImage.GetComponent<RectTransform>().position = correctedPos;
        }
    }

    public void SetPinUI(Vector3Int dest) 
    {
        Tile target = FieldSystem.tileSystem.GetTile(dest);
        if (FieldSystem.tileSystem == null) Debug.LogError("??");
        if (target is null) 
        {
            Debug.LogError("해당 Hex 좌표의 Tile은 존재하지 않습니다. Hex 좌표: " + dest);
            return;
        }
        _targetPos = target.gameObject.transform.position;
        targetHexPos = dest;
        _isTracking = true;
        _pinImage.SetActive(_isTracking);
    }
    public void ClearPinUI()
    {
        _targetPos = Vector3.zero;
        targetHexPos = Vector3Int.zero;
        _isTracking = false;
        _pinImage.SetActive(_isTracking);
    }

    public void OnClickPin()
    {
        worldCamera.SetPosition(_targetPos);
    }
}
