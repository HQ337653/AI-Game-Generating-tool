using UnityEngine;

/// <summary>
/// 处理玩家射击逻辑
/// </summary>
public class PlayerShooting : MonoBehaviour
{
	/// <summary>
	/// 当射击时触发的事件
	/// </summary>
	public event System.Action OnShoot;

	/// <var>
	/// 子弹预制体
	/// </var>
	public GameObject bulletPrefab;

	/// <var>
	/// 子弹发射点（如枪口位置）
	/// </var>
	public Transform firePoint;

	/// <var>
	/// 射击冷却时间
	/// </var>
	public float fireRate = 0.5f;

	/// <var>
	/// 子弹速度
	/// </var>
	public float bulletSpeed = 10f;

	private float nextFireTime = 0f;

	private void Start()
	{
		if (firePoint == null)
		{
			firePoint = transform;
		}
	}

	/// <method>
	/// 处理射击输入
	/// </method>
	/// <param name=\"targetPoint\">射击目标点（鼠标位置）</param>
	public void HandleShootInput(Vector3 targetPoint)
	{
		if (Time.time < nextFireTime) return;
		if (bulletPrefab == null) return;
		
		// 计算射击方向
		Vector2 direction = (targetPoint - firePoint.position).normalized;
		
		// 生成子弹
		GameObject bullet = SpawnBullet(direction);
		
		// 设置下次可射击时间
		nextFireTime = Time.time + fireRate;
		
		// 触发射击事件
		OnShoot?.Invoke();
	}

	/// <method>
	/// 生成子弹对象
	/// </method>
	/// <param name=\"direction\">子弹飞行方向</param>
	/// <returns>生成的子弹对象</returns>
	private GameObject SpawnBullet(Vector2 direction)
	{
		if (bulletPrefab == null) return null;
		
		GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
		
		// 获取Bullet组件并初始化
		Bullet bulletComponent = bullet.GetComponent<Bullet>();
		if (bulletComponent != null)
		{
			bulletComponent.Initialize(direction);
		}
		
		return bullet;
	}
}
