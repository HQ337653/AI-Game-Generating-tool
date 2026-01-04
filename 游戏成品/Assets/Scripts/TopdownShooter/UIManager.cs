using UnityEngine;

/// <summary>
/// UIManager 提供统一接口给其他脚本更新 UI（金币数量、生存时间），不直接引用具体 UI 元素，支持未来扩展
/// 该类为单例模式，挂载在独立 UI 管理对象上
/// </summary>
public class UIManager : MonoBehaviour
{
	/// <var>
	/// 单例实例
	/// </var>
	public static UIManager Instance;

	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	/// <method>
	/// 更新金币数量显示
	/// </method>
	/// <param name="amount">玩家当前金币数量</param>
	public void UpdateCoinUI(int amount)
	{
		//Debug.Log($"[UIManager] 更新金币数量: {amount}");
		// 具体 UI 更新逻辑由后续实现
	}

	/// <method>
	/// 更新生存时间显示
	/// </method>
	/// <param name="time">当前生存时间（秒）</param>
	public void UpdateTimeUI(float time)
	{
		//Debug.Log($"[UIManager] 更新生存时间: {time:F2} 秒");
		// 具体 UI 更新逻辑由后续实现
	}
}