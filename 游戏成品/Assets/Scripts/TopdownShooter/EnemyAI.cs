using UnityEngine;

/// <summary>
/// EnemyAI 控制敌人朝玩家移动并接触造成伤害，挂载在敌人 prefab 上
/// 使用 Rigidbody/Collider 处理物理移动，依赖 HealthSystem 接口处理受击与死亡
/// 死亡时直接生成金币并通过 UIManager 更新
/// </summary>
public class EnemyAI : MonoBehaviour
{
    /// <var>
    /// 移动速度，需要在 Inspector 中赋值
    /// </var>
    public float MoveSpeed = 3f;

    /// <var>
    /// 玩家 Transform，需要在 Inspector 中赋值
    /// </var>
    public Transform PlayerTransform;

    /// <var>
    /// 敌人攻击伤害，需要在 Inspector 中赋值
    /// </var>
    public int Damage = 10;

    /// <var>
    /// 敌人 Rigidbody，需要在 Inspector 中赋值，用于物理移动
    /// </var>
    public Rigidbody rb;

    /// <var>
    /// 敌人生命系统，需要在 Inspector 中赋值
    /// </var>
    public HealthSystem healthSystem;

    /// <var>
    /// 金币 prefab，需要在 Inspector 中赋值
    /// </var>
    public GameObject CoinPrefab;

    /// <var>
    /// UIManager 单例实例，不需要赋值
    /// </var>
    private UIManager uiManager;

    // 添加实际速度变量，用于存储随机后的速度
    private float actualMoveSpeed;

    private void Start()
    {
        uiManager = UIManager.Instance;
        healthSystem.OnDeath += HandleDeath;

        // 在基础速度的±20%范围内随机
        float speedVariation = Random.Range(0.8f, 1.2f);
        actualMoveSpeed = MoveSpeed * speedVariation;
    }

    private void FixedUpdate()
    {
        if (PlayerTransform != null)
        {
            Vector3 dir = (PlayerTransform.position - transform.position).normalized;
            rb.MovePosition(transform.position + dir * actualMoveSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        HealthSystem playerHealth = collision.collider.GetComponent<HealthSystem>();
        if (playerHealth != null)
        {
            playerHealth.ReduceHealth(Damage);
            Debug.Log($"[EnemyAI] {gameObject.name} 攻击 {collision.gameObject.name} 造成 {Damage} 伤害");
        }
    }

    /// <method>
    /// 死亡处理，生成金币并通知 UIManager
    /// </method>
    private void HandleDeath()
    {
        if (CoinPrefab != null && PlayerTransform != null)
        {
            var g = Instantiate(CoinPrefab, transform.position, Quaternion.identity);
            g.SetActive(true);
            uiManager.UpdateCoinUI(1);
            Debug.Log($"[EnemyAI] {gameObject.name} 死亡，生成金币");
        }
    }
}