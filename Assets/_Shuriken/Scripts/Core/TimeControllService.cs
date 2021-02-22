using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControllService : MonoBehaviour
{

    Coroutine timeCoro;

    public float enemyHeadSlowMotionTime;

    private void Awake()
    {
        EventService.OnHitEnemyHead += OnHitEnemyHead;
    }

    void Start()
    {
        
    }

   
    void Update()
    {
        
    }

    void OnHitEnemyHead()
    {
        SlowMotion(enemyHeadSlowMotionTime, 0.5f);
    }

    public void SlowMotion(float duration,float timeScale)
    {
        if(timeCoro == null)
            timeCoro = StartCoroutine(IESlowMotion(duration, timeScale));
    }

    IEnumerator IESlowMotion(float duration, float timeScale)
    {
        Time.timeScale = timeScale;

        yield return new WaitForSecondsRealtime(duration);

        while (Time.timeScale < 1)
        {
            Time.timeScale += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = 1;
        timeCoro = null;
    }

    private void OnDestroy()
    {
        EventService.OnHitEnemyHead -= OnHitEnemyHead;
    }
}
