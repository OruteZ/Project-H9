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
        
        // �ؽ�Ʈ�� �׻� ���� ���ϵ��� ��
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    void Update()
    {
        // ����: �ؽ�Ʈ�� �׻� Ư�� ��ġ�� ����
        if (_worldText != null)
        {
            _worldText.transform.position = Hex.Hex2World(hexPosition);
            _worldText.transform.position += new Vector3(0, yOffset, 0);
        }
    }
}