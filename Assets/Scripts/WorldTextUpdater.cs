using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WorldTextUpdater : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _worldText;
    public string message = "Hello, World!";
    
    [FormerlySerializedAs("hexPos")] public Vector3Int hexPosition;
    public float yOffset = 1.0f;

    void Start()
    {
        if (_worldText != null)
        {
            _worldText.text = message;
        }
        
        // 텍스트가 항상 위를 향하도록 함
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    void Update()
    {
        
        
        // 예시: 텍스트를 항상 특정 위치에 고정
        if (_worldText != null)
        {
            _worldText.transform.position = Hex.Hex2World(hexPosition);
            _worldText.transform.position += new Vector3(0, yOffset, 0);
        }
    }
    
    
}