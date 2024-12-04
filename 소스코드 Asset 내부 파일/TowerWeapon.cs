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

        // 90�� ���� �߰�
        float offset = -90f; // �⺻ ������ ������ ���
        transform.rotation = Quaternion.Euler(0, 0, degree + offset);
    }


    private IEnumerator SearchTarget()
    {
        while (true)
        {
            float closestDistSqr = Mathf.Infinity;
            for (int i = 0; i < enemySpawner.EnemyList.Count; i++)
            {
                // ���� Ÿ���� ���� ���� ���� ���� ���
                float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
                if (distance <= attackRange && distance <= closestDistSqr)
                {
                    closestDistSqr = distance;
                    attackTarget = enemySpawner.EnemyList[i].transform;
                }
            }

            // Ÿ���� �߰ߵǸ� ���� ���·� ��ȯ
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
                break; // ���� ������ �ٽ� Ÿ���� ã���� ���� ��
            }

            float distance = Vector3.Distance(attackTarget.position, transform.position);
            if (distance > attackRange)
            {
                // ������ ����� Ÿ���� null�� �����ϰ� SearchTarget ���·� ���ư�
                attackTarget = null;
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // ���� Ÿ�ָ̹��� ������Ÿ�� �߻�
            yield return new WaitForSeconds(attackRate);
            SpawnProjectile(); // ������Ÿ�� �߻�
        }
    }

    private void SpawnProjectile()
    {
        // ������ ����� ������ ������Ÿ���� �߻�
        if (attackTarget != null)
        {
            GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
            clone.GetComponent<Projectile>().Setup(attackTarget, attackDamage);
        }
    }


}
