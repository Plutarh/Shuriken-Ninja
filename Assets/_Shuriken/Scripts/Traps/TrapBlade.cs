using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBlade : MonoBehaviour
{
    Vector3 _prevPos;
    Vector3 _pos;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _prevPos = _pos;
        _pos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
       
        if (collision == null) return;

        var enemy = collision.gameObject.transform.root.GetComponent<AIEnemy>();

        if(enemy != null)
        {
            enemy.TakeDamage(10, (_pos - _prevPos).normalized * 8, EDamageType.Hit);
        }
    }
}
