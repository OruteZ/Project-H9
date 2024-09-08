using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HeinrichTrap : TileObject
{
    [SerializeField] private int damage;
    [SerializeField] private int boundDuration;
    [SerializeField] private Unit owner;
    
    public UnityEvent onTrapTriggered = new UnityEvent();
    
    public override string[] GetArgs()
    {
        return new string[] {damage.ToString(), boundDuration.ToString()};
    }

    [Obsolete("HeinrichTrap�� SetArgs�� ������� �ʽ��ϴ�.")]
    public override void SetArgs(string[] args)
    {
        return;
    }

    public void SetUp(Unit owner, Vector3Int hexPosition, int damage, int boundDuration, UnityAction onFinishSetup = null)
    {
        this.owner = owner;
        this.hexPosition = hexPosition;
        this.damage = damage;
        this.boundDuration = boundDuration;
        
        base.SetUp();
        
        if (tile == null)
        {
            Debug.LogError("Ÿ���� ���� ������ Tile Object ��ġ : hexPosition = " + hexPosition);
            throw new Exception();
        }
        
        StartCoroutine(MoveTo(tile.transform.position, () =>
        {
            onFinishSetup?.Invoke();
        }));
    }
    
    public void SetOwner(Unit owner)
    {
        this.owner = owner;
    }
    
    public Unit GetOwner()
    {
        return owner;
    }

    public override void OnCollision(Unit other)
    {
        // create damage context
        Damage damageContext = new Damage(
            damage,
            damage,
            Damage.Type.DEFAULT,
            owner,
            other);
        
        // apply damage
        other.TakeDamage(damageContext);
        
        // create status effect : bound
        StatusEffect rooted = new Rooted(owner);
        
        // apply status effect
        if (!other.TryAddStatus(rooted))
        {
            Debug.LogError("�����̻��� ������� ����");
        }
        
        // invoke
        onTrapTriggered.Invoke();
        
        // remove trap
        RemoveSelf();
    }
    
    private IEnumerator MoveTo(Vector3 targetPosition, Action callback)
    {
        //�������� �׸��� targetPosition���� �̵�
        const float duration = 1f;

        Vector3 startPosition = transform.position;
        
        int randomRotationDirection = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;

        float startTime = Time.time;
        float progress = 0;

        while (progress < 1)
        {
            progress = (Time.time - startTime) / duration;

            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, progress);
            currentPosition.y = Mathf.Sin(progress * Mathf.PI) * 2;

            transform.position = currentPosition;
            
            transform.Rotate(Vector3.up, randomRotationDirection * 360 * Time.deltaTime);

            yield return null;
        }
        
        // 3�ʿ� ���� ��������. Material�� alpha���� �����ϴ� ������� ����
        float durationReciprocal = 1 / 3f;
        float t = 0;
        Material material = GetComponentInChildren<MeshRenderer>().material;
        Color originalColor = material.color;
        
        while (t < 3f)
        {
            yield return null;
            
            t += Time.deltaTime;
            var percentage = t * durationReciprocal;
            
            material.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - percentage);
        }

        callback();
    }
}