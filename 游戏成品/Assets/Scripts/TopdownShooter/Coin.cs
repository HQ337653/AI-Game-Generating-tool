using UnityEngine;
using DG.Tweening;

/// <summary>
/// Coin 处理金币飞向玩家和收集
/// 挂载在金币 prefab 上
/// </summary>
public class Coin : MonoBehaviour
{
	/// <var>
	/// 飞向玩家的速度，需要在 Inspector 中赋值
	/// </var>
	public float FlySpeed = 5f;

	/// <var>
	/// 玩家 Transform，需要在 Inspector 中赋值
	/// </var>
	public Transform PlayerTransform;

	private void Update()
	{
		if(PlayerTransform != null)
		{
			transform.position = Vector3.MoveTowards(transform.position, PlayerTransform.position, FlySpeed * Time.deltaTime);
			if(Vector3.Distance(transform.position, PlayerTransform.position) < 0.5f)
			{
				Collect();
			}
		}
	}

	/// <method>
	/// 收集金币，通知 UIManager 并销毁自身
	/// </method>
	private void Collect()
	{
		UIManager.Instance.UpdateCoinUI(1); // 这里示意增加 1 个金币，可根据管理器修改
		Debug.Log($"[Coin] 收集金币");
		Destroy(gameObject);
	}
}