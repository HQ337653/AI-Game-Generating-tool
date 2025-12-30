using UnityEngine;

/// <summary>
/// 子弹实体的行为控制器
/// </summary>
public class Bullet : MonoBehaviour
{
	/// <var>
	/// 子弹的飞行速度
	/// </var>
	public float Speed = 10f;

	/// <var>
	/// 子弹的伤害值（阶段1暂不使用）
	/// </var>
	public float Damage = 1f;

	private Rigidbody2D rb;
	private Vector2 direction;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		if (rb == null)
		{
			rb = gameObject.AddComponent<Rigidbody2D>();
			rb.gravityScale = 0f;
		}
	}

	/// <method>
	/// 初始化子弹的方向
	/// </method>
	/// <param name="direction">子弹的飞行方向</param>
	public void Initialize(Vector2 direction)
	{
		this.direction = direction.normalized;
		if (rb != null)
		{
			rb.velocity = this.direction * Speed;
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Wall"))
		{
			Destroy(gameObject);
		}
	}
}
