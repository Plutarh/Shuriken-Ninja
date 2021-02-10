using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InputService : MonoBehaviour, IInputService
{

    public float raycastLenght = 100;
    public float farMultiplier = 10;

    public event Action<Vector3,GameObject> OnColliderClick;
    public event Action<Vector3> OnNonColliderClick;

    public LayerMask layerMask;

    private void Awake()
    {
        if (raycastLenght == 0) raycastLenght = 200;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastByClick();
    }

    void RaycastByClick()
    {
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit raycastHit;
          
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, raycastLenght, layerMask))
            {
                Debug.Log($"<color=green> {raycastHit.transform.root.name}- { raycastHit.point} raycast </color>");
                //Debug.DrawLine(Camera.main.transform.position, raycastHit.point, Color.cyan, 10f);
                OnColliderClick?.Invoke(raycastHit.point,raycastHit.collider.gameObject);

            }
            else
            {
                Vector3 direction = ray.origin + ray.direction;
                OnNonColliderClick?.Invoke(ray.direction.normalized);
                Debug.Log($"<color=red>{ray.direction.normalized * 10} dir </color>");
                
                //Debug.DrawLine(Camera.main.transform.position, (ray.direction.normalized * 10) + ray.origin, Color.magenta, 10f);

               
            }

        }
    }
}
