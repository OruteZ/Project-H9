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

    [Obsolete("HeinrichTrap은 SetArgs를 사용하지 않습니다.")]
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
            Debug.LogError("타일이 없는 곳으로 Tile Object 배치 : hexPosition = " + hexPosition);
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
            Debug.LogError("상태이상이 적용되지 않음");
        }
        
        // invoke
        onTrapTriggered.Invoke();
        
        // remove trap
        RemoveSelf();
    }
    
    private IEnumerator MoveTo(Vector3 targetPosition, Action callback)
    {
        //포물선을 그리며 targetPosition으로 이동
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
        
        // 3초에 걸쳐 투명해짐. Material의 alpha값을 조절하는 방식으로 구현
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