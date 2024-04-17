using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinUI : UISystem
{
    [SerializeField] private GameObject _pinImage;
    public WorldCamera worldCamera;

    private bool _isTracking = false;
    private Vector3 _targetPos = Vector3.zero;

    // Start is called before the first frame update
    void Awake()
    {
        _pinImage.SetActive(false);
    }
    void Update()
    {
        if (_isTracking)
        {
            _pinImage.SetActive(GameManager.instance.CompareState(GameState.World));
            Vector3 screenPos = Camera.main.WorldToScreenPoint(_targetPos);
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
        if (target is null) 
        {
            Debug.LogError("해당 Hex 좌표의 Tile은 존재하지 않습니다. Hex 좌표: " + dest);
            return;
        }
            _targetPos = target.gameObject.transform.position;
        _isTracking = true;
        _pinImage.SetActive(_isTracking);
    }
    public void ClearPinUI()
    {
        _targetPos = Vector3.zero;
        _isTracking = false;
        _pinImage.SetActive(_isTracking);
    }

    public void OnClickPin()
    {
        worldCamera.SetPosition(_targetPos);
    }
}
