using UnityEngine;

/// <summary>
/// 处理玩家移动逻辑
/// </summary>
public class PlayerMovement : MonoBehaviour
{
	/// <var>
	/// 玩家移动速度
	/// </var>
	public float moveSpeed = 5f;

	private Rigidbody2D rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		if (rb ==null)
		{
			rb = gameObject.AddComponent<Rigidbody2D>();
			rb.gravityScale = 0f; // 2D俯视游戏，不需要重力
		}
	}

	/// <method>
	/// 根据输入方向移动玩家
	/// </method>
	/// <param name=\"direction\">移动方向向量</param>
	public void Move(Vector2 direction)
	{
		if (rb==null) return;
		
		Vector2 normalizedDirection = direction.normalized;
		Vector2 velocity = normalizedDirection * moveSpeed;
		
		rb.velocity = velocity;
	}
}
