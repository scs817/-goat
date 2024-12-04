using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject enemyHPSliderPrefab;
    [SerializeField]
    private Transform canvasTransform;
    [SerializeField]
    private float spawnTime;
    [SerializeField]
    private Transform[] wayPoints;
    [SerializeField]
    private PlayerHP playerHP;
    [SerializeField]
    private PlayerGold playerGold;
    private List<Enemy> enemyList;
    [SerializeField] private Tilemap tilemap; // 타일맵
    [SerializeField] private Transform spawnPoint; // 적이 출현할 위치
    [SerializeField] private Transform exitPoint;

    public List<Enemy> EnemyList => enemyList;

    public void StartSpawner()
    {
        enemyList = new List<Enemy>();
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy()
    {
        Vector3Int start = tilemap.WorldToCell(spawnPoint.position);  // spawnPoint는 적이 출현할 위치 (예: 출발지점)
        Vector3Int end = tilemap.WorldToCell(exitPoint.position);
        while (true)
        {
            GameObject clone = Instantiate(enemyPrefab);
            Enemy enemy = clone.GetComponent<Enemy>();

            enemy.Setup(this, tilemap, start, end);
            enemyList.Add(enemy);

            SpawnEnemyHPSlider(clone);

            yield return new WaitForSeconds(spawnTime);
        }
    }
    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        if (type == EnemyDestroyType.Arrive)
        {
            playerHP.TakeDamage(1);
            Destroy(enemy.gameObject);
        }
        else if (type == EnemyDestroyType.Kill)
        {
            playerGold.CurrentGold += gold;
            Destroy(enemy.gameObject);
        }


        enemyList.Remove(enemy);
    }

    private void SpawnEnemyHPSlider(GameObject enemy)
    {
        GameObject sliderClone = Instantiate(enemyHPSliderPrefab);

        sliderClone.transform.SetParent(canvasTransform);
        sliderClone.transform.localScale = Vector3.one;

        sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);

        sliderClone.GetComponent<EnemyHPViewer>().Setup(enemy.GetComponent<EnemyHP>());
    }
}
