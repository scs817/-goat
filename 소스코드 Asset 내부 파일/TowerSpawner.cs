using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;
    [SerializeField]
    private int towerBuildGold = 50;
    [SerializeField]
    private EnemySpawner enemySpawner;
    [SerializeField]
    private PlayerGold playerGold;

    public void SpawnTower(Transform tileTransform)
    {
        if (towerBuildGold > playerGold.CurrentGold)
        {
            Debug.Log("Not enough gold!");
            return;
        }

        Tile tile = tileTransform.GetComponent<Tile>();

        if (tile.IsBuildTower == true)
        {
            Debug.Log("Tower already built on this tile.");
            return;
        }

        tile.IsBuildTower = true;
        playerGold.CurrentGold -= towerBuildGold;
        GameObject clone = Instantiate(towerPrefab, tileTransform.position, Quaternion.identity);

        clone.GetComponent<TowerWeapon>().Setup(enemySpawner);
        Debug.Log("Tower spawned at " + tileTransform.position);
    }

    // Start is called before the first frame update

}
