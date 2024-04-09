using System;
using UnityEngine;

public static class UICustomColor
{
    private static readonly Color32 whiteColor = new Color32(251, 251, 251, 255);
    private static readonly Color32 grayColor = new Color32(128, 128, 128, 255);
    private static readonly Color32 blackColor = new Color32(0, 0, 0, 255);
    private static readonly Color32 redColor = new Color32(160, 32, 64, 255);
    private static readonly Color32 orangeColor = new Color32(199, 94, 8, 255);
    private static readonly Color32 yellowColor = new Color32(240, 240, 0, 255);
    private static readonly Color32 greenColor = new Color32(18, 219, 36, 255);
    private static readonly Color32 blueColor = new Color32(64, 192, 224, 255);

    public static readonly Color32 TransparentColor = new Color32(255, 255, 255, 0);
    public static readonly Color32 Invisible = new Color32(0, 0, 0, 0);
    public static readonly Color32 Opaque = new Color32(0, 0, 0, 200);

    public static readonly Color32 PlayerColor = new Color32(0, 0, 0, 0);
    public static readonly Color32 EnemyColor = new Color32(0, 0, 0, 0);
    public static readonly Color32 NeutralColor = new Color32(0, 0, 0, 0);

    public static readonly Color32 PlayerTurnColor = new Color32(64, 192, 224, 255);    //blue
    public static readonly Color32 EnemyTurnColor = new Color32(160, 32, 64, 255);      //red'

    public static readonly Color32 ActionAPColor = new Color32(0, 224, 128, 255);       //green'
    public static readonly Color32 ActionAmmoColor = new Color32(255, 128, 0, 255);     //orange'

    public static readonly Color32 NormalStateColor = whiteColor;                       //white
    public static readonly Color32 HighlightStateColor = yellowColor;                   //yellow
    public static readonly Color32 DisableStateColor = blackColor;                      //black

    public static readonly Color32 PlayerStatColor = new Color32(237, 146, 0, 255);     //orange
    public static readonly Color32 WeaponStatColor = new Color32(18, 219, 36, 255);     //green
    public static readonly Color32 SkillStatColor = new Color32(219, 36, 18, 255);      //red

    public static readonly Color32 TextHighlightColor = yellowColor;

    public static readonly Color32 SkillIconNotLearnedColor = new Color32(199, 94, 8, 255);     //brown
    public static readonly Color32 SkillIconLearnableColor = new Color32(255, 201, 18, 255);    //orange
    public static readonly Color32 SkillIconLearnedColor = new Color32(72, 219, 18, 255);       //green

    public static readonly Color32 BuffColor = greenColor;
    public static readonly Color32 DebuffColor = redColor;

    public static readonly Color32 QuestNameColor = new Color32(237, 146, 0, 255);
    public static readonly Color32 ItemTextColor = QuestNameColor;
    public static readonly Color32 SkillTextColor = QuestNameColor;

    public static string GetColorHexCode(Color32 color)
    {
        string r = string.Format("{0:X2}", color.r);
        string g = string.Format("{0:X2}", color.g);
        string b = string.Format("{0:X2}", color.b);

        string result = (r + g + b).Replace("0x", "");
        return result.ToUpper();
    }
    public static string ChangeTextColor(string str, Color32 color) 
    {
        return string.Format("<color=#{0}>{1}</color>", GetColorHexCode(color), str);
    }
}