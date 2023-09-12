using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderUIElement : MonoBehaviour
{
    public Unit _unit { get; private set; }
    private Image _frame;
    private Image _characterIcon;

    private readonly Vector3 TURN_ORDER_UI_INIT_POSITION = new Vector3(-440, 0, 0);
    private const int TURN_ORDER_UI_INTERVAL = 80;
    private Vector3 _targetPosition;

    private const float TURN_ORDER_UI_INIT_SIZE = 50;
    private const float TURN_ORDER_UI_SIZE_SCALE = 1.2f;
    private bool _isTurnOwner;

    [SerializeField] private Texture2D _textures;
    private Sprite[] _sprites;

    void Awake()
    {
        _frame = GetComponent<Image>();
        _characterIcon = transform.GetChild(0).gameObject.GetComponent<Image>();
        _targetPosition = TURN_ORDER_UI_INIT_POSITION;
        _isTurnOwner = false;

        _sprites = Resources.LoadAll<Sprite>("Sprite/" + _textures.name);
    }

    void Update()
    {
        //Position Setting
        Vector3 currentPosition = GetComponent<RectTransform>().localPosition;
        if (currentPosition != _targetPosition)
        {
            float threshold = 0.01f;
            if (Mathf.Abs(currentPosition.x - _targetPosition.x) > threshold)
            {
                currentPosition = Vector3.Lerp(currentPosition, _targetPosition, Time.deltaTime * 2);
            }
            else
            {
                currentPosition = _targetPosition;
            }
            GetComponent<RectTransform>().localPosition = currentPosition;
        }

        //Size Setting
        Vector2 currentSize = GetComponent<RectTransform>().sizeDelta;
        Vector2 targetSize = new Vector2(TURN_ORDER_UI_INIT_SIZE, TURN_ORDER_UI_INIT_SIZE);
        if (_isTurnOwner) 
        {
            targetSize *= TURN_ORDER_UI_SIZE_SCALE;
        }

        if (currentSize != targetSize)
        {
            float threshold = 0.01f;
            if (Mathf.Abs(currentSize.x - targetSize.x) > threshold)
            {
                currentSize = Vector3.Lerp(currentSize, targetSize, Time.deltaTime * 2);
            }
            else
            {
                currentSize = targetSize;
            }
            GetComponent<RectTransform>().sizeDelta = currentSize;
        }
    }

    public void InitTurnOrderUIElement(Unit unit, int order)
    {
        if (order <= -1) return;
        _unit = unit;
        if (unit is Player)
        {
            _frame.color = Color.green;
            _characterIcon.sprite = _sprites[0];
        }
        else 
        {
            _frame.color = Color.red;
            _characterIcon.sprite = _sprites[1];
        }

        //_characterIcon.sprite = unit.icon;

        Vector3 pos = TURN_ORDER_UI_INIT_POSITION;
        pos.x += TURN_ORDER_UI_INTERVAL * order;
        GetComponent<RectTransform>().localPosition = pos;

        ChangeOrder(order);
    }
    public void ChangeOrder(int order)
    {
        if (order <= -1) return;
        Vector3 pos = TURN_ORDER_UI_INIT_POSITION;
        pos.x += TURN_ORDER_UI_INTERVAL * order;
        _targetPosition = pos;

        _isTurnOwner = false;
        if (order == 0)
        {
            _isTurnOwner = true;
        }
    }
}
