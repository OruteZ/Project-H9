using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoPopup : Generic.Singleton<InfoPopup>
{
    public enum MESSAGE { DEFAULT = 0
                        , DO_MOVE = 1
                        , IT_IS_MOVE_GAGE = 2
                        , IT_IS_TURN_END = 3
                        , COMBAT_TURN = 4
                        , COMBAT_ACTION = 5
                        , COMBAT_HP = 6
                        , IT_IS_HIT_RATE = 7
                        , INCREASED_SP = 8
                        , INCREASED_STAT = 9
    };

    private Vector2[] prePosition = { new Vector2(0, 0) // default 0
                                    , new Vector2(-140, -35) // 플레이어 위치 옆 1
                                    , new Vector2(79, -360) // 행동력 위 2
                                    , new Vector2(447, -360) // 턴종 위 3
                                    , new Vector2(-494, 376) // 전투 턴 ui(상단) 왼쪽아래 4
                                    , new Vector2(-140, -35) // 플레이어 위치 옆 5
                                    , new Vector2(79, -360) // 체력 위
                                    , new Vector2(0, 0) 
                                    , new Vector2(-615, -456) // SP 화면  -426, 368
                                    , new Vector2(-542, -289) // Stat
    };

    [SerializeReference]
    private GameObject _infoMessagePrefab;

    [SerializeReference]
    private Transform _canvas;
    
    private readonly string INFO_POPUP_LOCALIZATION_PATH = $"InfoPopupLocalizationTable";
    private Dictionary<int, string> _infoMessage;
    private GameObject _curInfo;

    protected override void Awake()
    {
        base.Awake();
        FileRead.ParseLocalization(INFO_POPUP_LOCALIZATION_PATH, out _infoMessage);
    }

    public void InitCallback(UserData user)
    {
        if (!user.Events.TryGetValue("INFO_POPUP_MESSAGE_DO_MOVE", out var value) || value == 0)
        {
            Show(MESSAGE.DO_MOVE);
            user.Events.TryAdd("INFO_POPUP_MESSAGE_DO_MOVE", 1);
        }

        PlayerEvents.OnChangedStat.AddListener((stat, type) =>
        {
            if (!user.Events.TryGetValue("INFO_POPUP_MOVE_GAGE", out var value) || value == 0)
            {
                if (type == StatType.CurActionPoint && stat.GetStat(type) != stat.GetStat(StatType.MaxActionPoint))
                {
                    Show(MESSAGE.IT_IS_MOVE_GAGE);
                    user.Events.TryAdd("INFO_POPUP_MOVE_GAGE", 1);
                }
            }
        });

        PlayerEvents.OnChangedStat.AddListener((stat, type) =>
        {
            if (!user.Events.TryGetValue("INFO_POPUP_TURN_END", out var value) || value == 0)
            {
                if (type == StatType.CurActionPoint && stat.GetStat(type) == 0)
                {
                    Show(MESSAGE.IT_IS_TURN_END);
                    user.Events.TryAdd("INFO_POPUP_TURN_END", 1);
                }
            }
        });

        PlayerEvents.OnChangedStat.AddListener((stat, type) =>
        {
            if (!user.Events.TryGetValue("INFO_POPUP_COMBAT_HP", out var value) || value == 0)
            {
                if (type == StatType.CurHp && stat.GetStat(type) != stat.GetStat(StatType.MaxHp))
                {
                    Show(MESSAGE.COMBAT_HP);
                    user.Events.TryAdd("INFO_POPUP_COMBAT_HP", 1);
                }
            }
        });

        UIManager.instance.onTSceneChanged.AddListener((scene) =>
        {
            if (!user.Events.TryGetValue("INFO_POPUP_COMBAT_TURN", out var value) || value == 0)
            {
                if (scene == GameState.Combat)
                {
                    Show(MESSAGE.COMBAT_TURN);
                    user.Events.TryAdd("INFO_POPUP_COMBAT_TURN", 1);
                }
            }
        });

        UIManager.instance.onTSceneChanged.AddListener((scene) =>
        {
            if (!user.Events.TryGetValue("INFO_POPUP_COMBAT_ACTION", out var value) || value == 0)
            {
                if (scene == GameState.Combat)
                {
                    Show(MESSAGE.COMBAT_ACTION);
                    user.Events.TryAdd("INFO_POPUP_COMBAT_ACTION", 1);
                }
            }
        });


        PlayerEvents.OnIncSkillPoint.AddListener(() =>
        {
            if (!user.Events.TryGetValue("INFO_POPUP_INCREASED_SP", out var value) || value == 0)
            {
                Show(MESSAGE.INCREASED_SP);
                user.Events.TryAdd("INFO_POPUP_INCREASED_SP", 1);
            }
        });

        PlayerEvents.OnIncStatPoint.AddListener(() =>
        {
            if (!user.Events.TryGetValue("INFO_POPUP_INCREASED_STAT", out var value) || value == 0)
            {
                Show(MESSAGE.INCREASED_STAT);
                user.Events.TryAdd("INFO_POPUP_INCREASED_STAT", 1);
            }
        });
    }

    public void Show(MESSAGE message)
    {
        if(_infoMessagePrefab == null)
        {
            Debug.LogError("InfoMessagePrefab is null");
            return; 
        }
        
        _curInfo = Instantiate(_infoMessagePrefab, _canvas);
        //_curInfo.transform.parent = _canvas;
        var anchorPos = prePosition[(int)message];
        _curInfo.GetComponent<RectTransform>().anchoredPosition = anchorPos;
        _curInfo.GetComponentInChildren<TMPro.TMP_Text>().text = _infoMessage[((int)message)];

        // delete는 버튼으로 할당
    }
}
