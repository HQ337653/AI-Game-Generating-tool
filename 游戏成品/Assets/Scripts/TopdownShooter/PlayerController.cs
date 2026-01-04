using UnityEngine;

/// <summary>
/// PlayerController 负责玩家的移动和射击输入处理，挂载在玩家角色对象上
/// 使用旧输入系统监听 WASD 移动和鼠标左键射击，依赖 Bullet prefab 生成子弹
/// </summary>
public class PlayerController : MonoBehaviour
{
	/// <var>
	/// 玩家移动速度，需要在 Inspector 中赋值
	/// </var>
	public float MoveSpeed = 5f;

	/// <var>
	/// 玩家子弹 prefab，需要在 Inspector 中赋值
	/// </var>
	public GameObject BulletPrefab;

	/// <var>
	/// 子弹生成位置，需要在 Inspector 中赋值（玩家子弹发射点 Transform）
	/// </var>
	public Transform BulletSpawnPoint;

	/// <var>
	/// UIManager 单例实例，不需要赋值
	/// </var>
	private UIManager uiManager;

	private void Start()
	{
		uiManager = UIManager.Instance;
	}

	/// <method>
	/// 每帧处理玩家输入和移动
	/// </method>
	private void Update()
	{
		HandleMovement();
		HandleShooting();
	}

	/// <method>
	/// 处理 WASD 移动
	/// </method>
	private void HandleMovement()
	{
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");
		Vector3 dir = new Vector3(h, 0, v).normalized;
		transform.Translate(dir * MoveSpeed * Time.deltaTime, Space.World);
	}

	/// <method>
	/// 处理鼠标左键射击
	/// </method>
	private void HandleShooting()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Shoot();
			Debug.Log("按下左键");
		}
	}

	/// <method>
	/// 生成子弹并朝鼠标点击方向发射
	/// </method>
	private void Shoot()
	{
		if(BulletPrefab != null && BulletSpawnPoint != null)
		{
			GameObject bullet = Instantiate(BulletPrefab, BulletSpawnPoint.position, Quaternion.identity);
			bullet.SetActive(true);
			Vector3 mousePos = Input.mousePosition;
			Ray ray = Camera.main.ScreenPointToRay(mousePos);
			Plane plane = new Plane(Vector3.up, BulletSpawnPoint.position);
			if(plane.Raycast(ray, out float distance))
			{
				Vector3 targetPoint = ray.GetPoint(distance);
				bullet.transform.forward = (targetPoint - BulletSpawnPoint.position).normalized;
				Debug.Log("[PlayerController] 发射子弹");
			}
		}
	}
}