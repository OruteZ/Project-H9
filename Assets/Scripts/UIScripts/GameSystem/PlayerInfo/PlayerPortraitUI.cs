using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPortraitUI : UIElement
{
    [SerializeField] private Sprite _defaultPlayerImage;
    public string playerModelName = "";
    public void SetPortraitUI() 
    {
        if (playerModelName == "") 
        {
            Debug.LogError("Please Enter Player Model Name in Portrait UI");
        }
        Texture2D playerTexture = Resources.Load("UnitCapture/" + playerModelName) as Texture2D;
        Sprite spr = Sprite.Create(playerTexture, new Rect(0, 0, playerTexture.width, playerTexture.height), new Vector2(0.5f, 0.5f));
        if (spr == null) spr = _defaultPlayerImage;
        transform.GetChild(0).GetComponent<Image>().sprite = spr;
    }
    public void ClickPortrait()
    {
        CameraManager.instance.worldCamera.SetPosition(FieldSystem.unitSystem.GetPlayer().transform.position);
    }
}
