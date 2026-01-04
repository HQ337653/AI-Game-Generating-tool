using UnityEngine;

/// <summary>
/// GameManagerTimer 管理游戏整体状态和生存时间，挂载在独立管理对象上
/// 是 singleton 管理器，负责胜利和失败判定
/// </summary>
public class GameManagerTimer : MonoBehaviour
{
	/// <var>
	/// 单例实例
	/// </var>
	public static GameManagerTimer Instance;

	/// <var>
	/// 玩家对象，需要在 Inspector 中赋值（用于检测死亡）
	/// </var>
	public GameObject Player;

	/// <var>
	/// 生存计时
	/// </var>
	public float SurvivalTime = 0f;

	/// <var>
	/// 游戏胜利所需时间，需要在 Inspector 中赋值（秒）
	/// </var>
	public float VictoryTime = 120f;

	/// <var>
	/// 游戏结束标记
	/// </var>
	private bool isGameOver = false;

	/// <var>
	/// UIManager 单例实例，不需要赋值
	/// </var>
	private UIManager uiManager;

	private void Awake()
	{
		if(Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		uiManager = UIManager.Instance;
	}

	private void Update()
	{
		if(isGameOver) return;
		SurvivalTime += Time.deltaTime;
		uiManager.UpdateTimeUI(SurvivalTime);
		CheckVictory();
		CheckPlayerDeath();
	}

	/// <method>
	/// 检查玩家是否死亡
	/// </method>
	private void CheckPlayerDeath()
	{
		if(Player == null) return;
		HealthSystem hs = Player.GetComponent<HealthSystem>();
		if(hs != null && hs.CurrentHealth <= 0)
		{
			EndGame(false);
		}
	}

	/// <method>
	/// 检查胜利条件
	/// </method>
	private void CheckVictory()
	{
		if(SurvivalTime >= VictoryTime)
		{
			EndGame(true);
		}
	}

	/// <method>
	/// 游戏结束处理
	/// </method>
	/// <param name="isVictory">是否胜利</param>
	private void EndGame(bool isVictory)
	{
		isGameOver = true;
		Debug.Log($"[GameManagerTimer] 游戏结束，胜利: {isVictory}");
	}
}