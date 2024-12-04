using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderPositionAutoSetter : MonoBehaviour
{
    [SerializeField]
    private Vector3 distance = Vector3.down * 20.0f;
    private Transform targetTransform;
    private RectTransform rectTansform;

    public void Setup(Transform target)
    {
        targetTransform = target;
        rectTansform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (targetTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
        rectTansform.position = screenPosition + distance;
    }
    // Start is called before the first frame update

}
