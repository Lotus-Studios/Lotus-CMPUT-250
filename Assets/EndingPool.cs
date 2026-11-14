using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndingPool : MonoBehaviour
{
    [SerializeField] private float inPoolWaitAlarm = 3f;
    private bool playerInPool = false;
    private float currentPoolAlarm = 0f;
    public UnityEvent poolAlarm;
    // Start is called before the first frame update
    void Start()
    {
        currentPoolAlarm = inPoolWaitAlarm;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInPool)
        {
            currentPoolAlarm -= Time.deltaTime;
            if(currentPoolAlarm <= 0)
            {
                poolAlarm?.Invoke();
                Destroy(this.gameObject);
            }
        }
        else
        {
            currentPoolAlarm = inPoolWaitAlarm;
        }

        Debug.Log($"Current pool alarm {currentPoolAlarm}");
    }

    void OnTriggerEnter(Collider other)
    {
        playerInPool = true;
    }

    void OnTriggerExit(Collider other)
    {
        playerInPool = false;
    }
}
