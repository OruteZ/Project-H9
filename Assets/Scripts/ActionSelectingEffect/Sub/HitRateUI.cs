using TMPro;
using UnityEngine;

public class HitRateUI : MonoBehaviour
{
    public RectTransform combatCanvas;
    public RectTransform outLineUI;
    public RectTransform circleUI;
    public TMP_Text hitRateText;
    
    public void SetupEffect(Canvas canvas)
    {
        combatCanvas = canvas.GetComponent<RectTransform>();
    }

    public void SetTarget(IDamageable target, float hitRate)
    {
        hitRateText.text = $"{hitRate}%";
        outLineUI.gameObject.SetActive(true);
        circleUI.gameObject.SetActive(true);
        
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(Hex.Hex2World(target.GetHex()) + 
                                                                    Vector3.up * (target.GetTsf.localScale.y * 0.5f));
        var sizeDelta = combatCanvas.sizeDelta;
            
        Vector2 worldObjectScreenPosition = new Vector2(
            ((viewportPosition.x * sizeDelta.x) - (sizeDelta.x * 0.5f)),
            ((viewportPosition.y * sizeDelta.y) - (sizeDelta.y * 0.5f)));
        outLineUI.anchoredPosition = worldObjectScreenPosition;
        
        float size = hitRate * 0.01f;
        circleUI.localScale = new Vector3(size, size, 1);
    }
    
    public void OffTarget()
    {
        outLineUI.gameObject.SetActive(false);
        circleUI.gameObject.SetActive(false);
    }
}