using UnityEngine;
using System;

/// <summary>
/// HealthSystem 管理玩家或敌人的生命值，包括受伤和死亡判定
/// 挂载在玩家或敌人对象上
/// </summary>
public class HealthSystem : MonoBehaviour
{
	/// <var>
	/// 当前生命值
	/// </var>
	public int CurrentHealth = 100;

	/// <var>
	/// 最大生命值
	/// </var>
	public int MaxHealth = 100;

	/// <var>
	/// 死亡事件，其他管理器订阅
	/// </var>
	public event Action OnDeath;

	/// <method>
	/// 对对象造成伤害
	/// </method>
	/// <param name="amount">伤害数值</param>
	public void ReduceHealth(int amount)
	{
		CurrentHealth -= amount;
		Debug.Log($"[HealthSystem] {gameObject.name} 受伤, 剩余生命: {CurrentHealth}");
		if(CurrentHealth <= 0)
		{
			CurrentHealth = 0;
			Die();
		}
	}

	/// <method>
	/// 死亡处理，触发死亡事件
	/// </method>
	private void Die()
	{
		Debug.Log($"[HealthSystem] {gameObject.name} 死亡");
		OnDeath?.Invoke();
	}

	/// <method>
	/// 回复生命值
	/// </method>
	/// <param name="amount">回复数值</param>
	public void Heal(int amount)
	{
		CurrentHealth += amount;
		if(CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
		Debug.Log($"[HealthSystem] {gameObject.name} 回复生命, 当前生命: {CurrentHealth}");
	}
}