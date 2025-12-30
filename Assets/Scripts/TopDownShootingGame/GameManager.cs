using UnityEngine;

/// <summary>
/// 游戏管理器，场景的启动器和服务中心
/// </summary>
public class GameManager : MonoBehaviour
{
	/// <var>
	/// GameManager的单例实例
	/// </var>
	public static GameManager Instance { get; private set; }

	/// <var>
	/// 对玩家游戏对象的引用
	/// </var>
	public GameObject Player { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		FindPlayer();
	}

	/// <method>
	/// 在场景中查找玩家对象
	/// </method>
	private void FindPlayer()
	{
		Player = GameObject.FindGameObjectWithTag("Player");
		if (Player == null)
		{
			Debug.LogWarning("未找到带有Player标签的游戏对象。");
		}
	}
}
