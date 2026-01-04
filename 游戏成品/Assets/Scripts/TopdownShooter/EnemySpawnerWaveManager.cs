using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// EnemySpawnerWaveManager 负责按波次生成敌人并管理波次进程，挂载在独立管理对象上
/// 监听 EnemyAI 死亡事件生成金币并通知 UIManager 更新
/// 是 singleton 管理器，目标物品为敌人 prefab
/// </summary>
public class EnemySpawnerWaveManager : MonoBehaviour
{
	/// <var>
	/// 单例实例
	/// </var>
	public static EnemySpawnerWaveManager Instance;

	/// <var>
	/// 敌人 prefab 列表，需要在 Inspector 中赋值
	/// </var>
	public List<GameObject> EnemyPrefabs;

	/// <var>
	/// 刷新点列表，需要在 Inspector 中赋值（Transform）
	/// </var>
	public List<Transform> SpawnPoints;

	/// <var>
	/// 当前波次编号
	/// </var>
	public int CurrentWave = 0;

	/// <var>
	/// 当前存活敌人数量
	/// </var>
	public int AliveEnemies = 0;

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
		StartWave();
	}

	/// <method>
	/// 开始新波次
	/// </method>
	public void StartWave()
	{
		CurrentWave++;
		int enemyCount = CurrentWave * 3;
		AliveEnemies = enemyCount;
		Debug.Log($"[EnemySpawnerWaveManager] 开始波次 {CurrentWave}，生成 {enemyCount} 个敌人");
		for(int i=0;i<enemyCount;i++)
		{
			SpawnEnemy();
		}
	}

	/// <method>
	/// 生成单个敌人
	/// </method>
	private void SpawnEnemy()
	{
		if(EnemyPrefabs.Count == 0 || SpawnPoints.Count == 0) return;
		GameObject enemyPrefab = EnemyPrefabs[Random.Range(0, EnemyPrefabs.Count)];
		Transform spawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Count)];
		GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        enemy.SetActive(true);
        HealthSystem hs = enemy.GetComponent<HealthSystem>();
		if(hs != null) hs.OnDeath += OnEnemyDeath;
		Debug.Log($"[EnemySpawnerWaveManager] 生成敌人 {enemy.name}");
	}

	/// <method>
	/// 敌人死亡回调
	/// </method>
	private void OnEnemyDeath()
	{
		AliveEnemies--;
		if(AliveEnemies <= 0)
		{
			Debug.Log($"[EnemySpawnerWaveManager] 波次 {CurrentWave} 完成");
			StartWave();
		}
	}
}