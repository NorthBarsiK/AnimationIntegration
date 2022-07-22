using System.Collections;
using UnityEngine;

[RequireComponent (typeof(EnemyTrigger), typeof(Collider))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float secondsToRespawn = 5.0f;

    private Rigidbody[] ragdollParts;
    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        ragdollParts = GetComponentsInChildren<Rigidbody>();
        OnSpawn();
    }

    public void Die()
    {
        if (ragdollParts.Length != 0)
        {
            foreach (Rigidbody rigidbody in ragdollParts)
            {
                rigidbody.isKinematic = false;
            }
        }
        animator.enabled = false;

        GetComponent<EnemyTrigger>().ChangeColliderState(false);
        StartCoroutine(NewEnemy());
    }

    private void SpawnNewEnemy()
    {
        OnSpawn();
        transform.position = GetNewPosition();
    }

    private Vector3 GetNewPosition()
    {
        float posX = Random.Range(-10, 10);
        float posZ = Random.Range(-10, 10);

        Vector3 newPosition = new Vector3(posX, 0, posZ);

        return newPosition;
    }

    private void OnSpawn()
    {
        if (animator != null)
        {
            if (!animator.enabled)
            {
                animator.enabled = true;
            }
        }
        else
        {
            Debug.LogError("Enemy children doesn't have Animator");
        }

        if (!GetComponent<Collider>().enabled)
        {
            GetComponent<EnemyTrigger>().ChangeColliderState(true);
        }

        if (ragdollParts.Length != 0)
        {
            foreach (Rigidbody rigidbody in ragdollParts)
            {
                rigidbody.isKinematic = true;
            }
        }
    }

    private IEnumerator NewEnemy()
    {
        yield return new WaitForSeconds(secondsToRespawn);
        SpawnNewEnemy();
    }
}
