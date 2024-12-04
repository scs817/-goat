using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField]
    private TowerSpawner towerSpawner;

    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;
    // Start is called before the first frame update
    private void Awake()
    {
        mainCamera = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // �浹�� ��ü�� �±׸� ����� ���
                Debug.Log("Hit object tag: " + hit.transform.tag);

                if (hit.transform.CompareTag("Wall"))
                {
                    towerSpawner.SpawnTower(hit.transform);
                }
            }
        }
    }
}
