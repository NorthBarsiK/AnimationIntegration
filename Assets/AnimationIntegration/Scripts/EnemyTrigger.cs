using UnityEngine;

[RequireComponent (typeof (Enemy), typeof(SphereCollider))]
public class EnemyTrigger : MonoBehaviour
{
    [SerializeField] private float triggerRadius = 4.0f;

    private void Start()
    {
        SetCollider();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            playerController.PlayerIsInEnemyTrigger(GetComponent<Enemy>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            playerController.PlayerIsNotEnemyTrigger();
        }
    }

    public void ChangeColliderState(bool state)
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        collider.enabled = state;
    }

    private void SetCollider()
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = triggerRadius;
    }
}
