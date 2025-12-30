using UnityEngine;

/// <summary>
/// 输入抽象层，负责监听Unity输入并将其转换为游戏逻辑可用的指令
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
	/// <summary>
	/// 当射击按键被按下时触发的事件，传递鼠标在世界空间中的位置
	/// </summary>
	public event System.Action<Vector3> OnShootPressed;

	/// <var>
	/// 获取当前移动输入的方向向量（已归一化）
	/// </var>
	public Vector2 MoveInput { get; private set; }

	/// <var>
	/// 获取当前射击输入是否被请求
	/// </var>
	public bool ShootRequested { get; private set; }

	/// <var>
	/// 获取鼠标在世界空间中的目标点（用于射击方向）
	/// </var>
	public Vector3 ShootTargetPoint { get; private set; }

	private void Update()
	{
		UpdateMoveInput();
		UpdateShootInput();
	}

	/// <method>
	/// 更新移动输入状态
	/// </method>
	private void UpdateMoveInput()
	{
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		
		Vector2 rawInput = new Vector2(horizontal, vertical);
		MoveInput = rawInput.normalized;
	}

	/// <method>
	/// 更新射击输入状态
	/// </method>
	private void UpdateShootInput()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			ShootRequested = true;
			
			// 将鼠标屏幕位置转换为世界位置（假设是2D游戏，z=0）
			Vector3 mouseScreenPos = Input.mousePosition;
			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
			mouseWorldPos.z = 0f; // 2D游戏，z轴固定为0
			
			ShootTargetPoint = mouseWorldPos;
			
			// 触发事件
			OnShootPressed?.Invoke(mouseWorldPos);
		}
		else
		{
			ShootRequested = false;
		}
	}
}
