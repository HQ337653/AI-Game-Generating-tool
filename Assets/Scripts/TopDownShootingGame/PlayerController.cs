using UnityEngine;

/// <summary>
/// 玩家角色的协调中枢，负责整合输入与具体功能
/// </summary>
public class PlayerController : MonoBehaviour
{
	/// <var>
	/// 输入处理器引用
	/// </var>
	private PlayerInputHandler inputHandler;

	/// <var>
	/// 移动组件引用
	/// </var>
	private PlayerMovement movement;

	/// <var>
	/// 射击组件引用
	/// </var>
	private PlayerShooting shooting;

	private void Awake()
	{
		// 获取组件引用
		inputHandler = GetComponent<PlayerInputHandler>();
		movement = GetComponent<PlayerMovement>();
		shooting = GetComponent<PlayerShooting>();

		// 验证必需组件
		if (inputHandler == null)
		{
			Debug.LogError("{gameObject.name}: 缺少PlayerInputHandler组件");
		}

		if (movement == null)
		{
			Debug.LogError("{gameObject.name}: 缺少PlayerMovement组件");
		}

		if (shooting == null)
		{
			Debug.LogError($"{gameObject.name}: 缺少PlayerShooting组件");
		}
	}

	private void Update()
	{
		if (inputHandler == null || movement == null || shooting == null) return;

		// 获取移动输入并移动
		Vector2 moveInput = inputHandler.MoveInput;
		movement.Move(moveInput);

		// 处理射击输入
		if (inputHandler.ShootRequested)
		{
			shooting.HandleShootInput(inputHandler.ShootTargetPoint);
		}
	}
}
