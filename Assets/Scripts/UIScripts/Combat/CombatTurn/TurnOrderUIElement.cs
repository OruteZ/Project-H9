using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TurnOrderUIElement : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    public Unit unit;// { get; private set; }
    private Image _frame;
    private Image _characterIcon;

    private readonly Vector3 TURN_ORDER_UI_INIT_POSITION = new Vector3(-440, 0, 0);
    private const int TURN_ORDER_UI_INTERVAL = 80;
    private Vector3 _targetPosition;

    private const float TURN_ORDER_UI_INIT_SIZE = 60;
    private const float TURN_ORDER_UI_SIZE_SCALE = 1.25f;
    private bool _isTurnOwner;

    [SerializeField] private Texture2D _textures;
    
    [SerializeField] private EnemyDatabase _enemyDatabase;
    [SerializeField] private Sprite playerIcon;

    void Awake()
    {
        _frame = GetComponent<Image>();
        _characterIcon = transform.GetChild(0).gameObject.GetComponent<Image>();
        _targetPosition = TURN_ORDER_UI_INIT_POSITION;
        _isTurnOwner = false;
    }

    void Update()
    {
        //Position Setting
        Vector3 currentPosition = GetComponent<RectTransform>().localPosition;
        if (currentPosition != _targetPosition)
        {
            float threshold = 0.01f;
            LerpCalculation.CalculateLerpValue(ref currentPosition, _targetPosition, Time.deltaTime * 2, threshold);
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
        this.unit = unit;
        if (unit is Player)
        {
            _frame.color = UICustomColor.PlayerTurnColor;
            _characterIcon.sprite = playerIcon;
        }
        else 
        {
            _frame.color = UICustomColor.EnemyTurnColor;

            //Debug.Log(_enemyDatabase.GetInfo(unit.Index).model.name);
            _characterIcon.sprite = Resources.Load<Sprite>(
                "UnitCapture/" + 
                _enemyDatabase.GetInfo(unit.Index).model.name);
            
            //Assets/Resources/UnitCapture/Character_Bandit_Male_01.png
        }

        //_characterIcon.sprite = unit.icon;

        Vector3 pos = TURN_ORDER_UI_INIT_POSITION;
        pos.x += TURN_ORDER_UI_INTERVAL * order;
        GetComponent<RectTransform>().localPosition = pos;
        GetComponent<RectTransform>().sizeDelta = new Vector2(TURN_ORDER_UI_INIT_SIZE, TURN_ORDER_UI_INIT_SIZE);

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

    public void EffectTurnOrderUIElement(bool isEffectOn)
    {

        if (unit is Player)
        {
            _frame.color = UICustomColor.PlayerTurnColor;
        }
        else if (isEffectOn)
        {
            _frame.color = UICustomColor.NormalStateColor;
        }
        else
        {
            _frame.color = UICustomColor.EnemyTurnColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (unit is Player) return;
        unit.TryGetComponent(out CustomOutline outline);
        if (outline is null)
        {
            outline = unit.gameObject.AddComponent<CustomOutline>();
            outline.OutlineColor = Color.red;
        }
        outline.OutlineMode = CustomOutline.Mode.OutlineAll;

        UIManager.instance.combatUI.turnOrderUI.EffectMouseOverEnemy((Enemy)unit);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (unit is Player) return;
        unit.TryGetComponent(out CustomOutline outline);
        if (outline is null)
        {
            outline = unit.gameObject.AddComponent<CustomOutline>();
            outline.OutlineColor = Color.red;
        }
        outline.OutlineMode = CustomOutline.Mode.NULL;

        UIManager.instance.combatUI.turnOrderUI.EffectMouseOverEnemy(null);
    }
}