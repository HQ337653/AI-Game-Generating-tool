using UnityEngine;

/// <summary>
/// Bullet 处理子弹飞行和碰撞检测
/// 挂载在子弹 prefab 上，使用 Rigidbody+Collider 处理物理移动
/// </summary>
public class Bullet : MonoBehaviour
{
	/// <var>
	/// 子弹速度，需要在 Inspector 中赋值
	/// </var>
	public float Speed = 10f;

	/// <var>
	/// 子弹伤害，需要在 Inspector 中赋值
	/// </var>
	public int Damage = 10;

	/// <var>
	/// 子弹 Rigidbody 组件，需要在 Inspector 中赋值
	/// </var>
	public Rigidbody rb;

	private void Start()
	{
		rb.velocity = transform.forward * Speed;
	}

    private void OnTriggerEnter(Collider other)
    {
        // 检查碰撞的物体是否有 EnemyAI 组件（只有敌人才有）
        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            HealthSystem health = other.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.ReduceHealth(Damage);
                Debug.Log($"[Bullet] 命中 {other.gameObject.name}, 造成 {Damage} 伤害");
                Destroy(gameObject);
            }
        }
    }
}