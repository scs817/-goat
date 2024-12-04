using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public enum WeaponState { SearchTarget = 0, AttackToTarget }

public class TowerWeapon : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private float attackRate = 0.5f;
    [SerializeField]
    private float attackRange = 2.0f;
    [SerializeField]
    private int attackDamage = 1;
    private WeaponState weaponState = WeaponState.SearchTarget;
    private Transform attackTarget = null;
    private EnemySpawner enemySpawner;

    // Start is called before the first frame update
    public void Setup(EnemySpawner enemySpawner)
    {
        this.enemySpawner = enemySpawner;

        ChangeState(WeaponState.SearchTarget);
    }

    public void ChangeState(WeaponState newState)
    {
        StopCoroutine(weaponState.ToString());

        weaponState = newState;

        StartCoroutine(weaponState.ToString());

    }

    // Update is called once per frame
    private void Update()
    {
        if (attackTarget != null)
        {
            RotateToTarget();
        }

    }

    private void RotateToTarget()
    {
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;

        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

        // 90도 보정 추가
        float offset = -90f; // 기본 방향이 위쪽일 경우
        transform.rotation = Quaternion.Euler(0, 0, degree + offset);
    }


    private IEnumerator SearchTarget()
    {
        while (true)
        {
            float closestDistSqr = Mathf.Infinity;
            for (int i = 0; i < enemySpawner.EnemyList.Count; i++)
            {
                // 적이 타워의 공격 범위 내에 있을 경우
                float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
                if (distance <= attackRange && distance <= closestDistSqr)
                {
                    closestDistSqr = distance;
                    attackTarget = enemySpawner.EnemyList[i].transform;
                }
            }

            // 타겟이 발견되면 공격 상태로 전환
            if (attackTarget != null)
            {
                ChangeState(WeaponState.AttackToTarget);
            }

            yield return null;
        }
    }

    private IEnumerator AttackToTarget()
    {
        while (true)
        {
            if (attackTarget == null)
            {
                ChangeState(WeaponState.SearchTarget);
                break; // 적이 없으면 다시 타겟을 찾으러 가야 함
            }

            float distance = Vector3.Distance(attackTarget.position, transform.position);
            if (distance > attackRange)
            {
                // 범위를 벗어나면 타겟을 null로 설정하고 SearchTarget 상태로 돌아감
                attackTarget = null;
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // 공격 타이밍마다 프로젝타일 발사
            yield return new WaitForSeconds(attackRate);
            SpawnProjectile(); // 프로젝타일 발사
        }
    }

    private void SpawnProjectile()
    {
        // 공격할 대상이 있으면 프로젝타일을 발사
        if (attackTarget != null)
        {
            GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
            clone.GetComponent<Projectile>().Setup(attackTarget, attackDamage);
        }
    }


}
