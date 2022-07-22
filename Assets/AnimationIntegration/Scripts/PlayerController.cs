using UnityEngine;

[RequireComponent (typeof(Collider), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private GameObject rifle = null;
    [SerializeField] private GameObject sword = null;
    [SerializeField] private GameObject UItext = null;
    [SerializeField] private float enemyFinishDistance = 1.0f;

    private bool isCanControl = true;
    private bool isCanFinish = false;
    
    private Vector3 cameraOffset = Vector3.zero;
    private Enemy enemyObj = null;
    private Camera cam = null;
    private Animator animator = null;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        SetRigidbody();
        SetCamera();
        ChangeWeapon();
    }

    private void Update()
    {
        CheckFinish();
        Controller();
    }

    public void PlayerIsInEnemyTrigger(Enemy enemyObject)
    {
        isCanFinish = true;
        enemyObj = enemyObject;
    }

    public void PlayerIsNotEnemyTrigger()
    {
        isCanFinish = false;
        enemyObj = null;
    }

    private void SetRigidbody()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
    }

    private void SetCamera()
    {
        cam = Camera.main;
        cameraOffset = cam.transform.position - transform.position;
    }

    private void ChangeWeapon(string weapon = "rifle")
    {
        if (rifle == null)
        {
            Debug.LogError("Rifle object is not assigned in Player controller!");
        }
        else if (sword == null)
        {
            Debug.LogError("Sword object is not assigned in Player controller!");
        }
        else
        {
            switch (weapon)
            {
                case "sword":
                    rifle.SetActive(false);
                    sword.SetActive(true);
                    break;
                default:
                    rifle.SetActive(true);
                    sword.SetActive(false);
                    break;
            }
        }
    }

    private void ChangeUITextState(bool state)
    {
        if(UItext != null)
        {
            UItext.SetActive(state);
        }
        else
        {
            Debug.LogError("UI text is not assigned in Player controller");
        }
    }

    private void Controller()
    {
        if (isCanControl)
        {
            #region Movement
            float moveZ = Input.GetAxisRaw("Vertical");
            float moveX = Input.GetAxisRaw("Horizontal");

            Vector3 movement = new Vector3(moveX, 0, moveZ);

            transform.position += movement.normalized * speed * Time.deltaTime;
            cam.transform.position = transform.position + cameraOffset;
            #endregion

            #region Rotation
            Vector3 rotationTarget = Vector3.zero;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                rotationTarget = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            }

            transform.rotation = Quaternion.LookRotation(rotationTarget - transform.position, Vector3.up);
            #endregion

            #region SetAnimatorParameters
            float rotationSin = Mathf.Sin(transform.rotation.eulerAngles.y * Mathf.Deg2Rad);
            float rotationCos = Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad);

            Vector2 blendTreeVector = Vector2.zero;
            blendTreeVector.x = moveX * rotationCos - moveZ * rotationSin;
            blendTreeVector.y = moveX * rotationSin + moveZ * rotationCos;

            animator.SetFloat("moveX", blendTreeVector.normalized.x);
            animator.SetFloat("moveZ", blendTreeVector.normalized.y);
            #endregion
        }
    }
    private void CheckFinish()
    {
        ChangeUITextState(isCanFinish);
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("finish"))
        {
            ChangeWeapon("sword");
        }
        else if (Input.GetKeyDown(KeyCode.Space) && isCanFinish)
        {
            Vector3 enemyPosition = enemyObj.transform.position;
            Vector3 distanceToEnemy = enemyPosition - transform.position;
            Vector3 finishPosition = Vector3.zero;
             
            if (distanceToEnemy.x > enemyFinishDistance || distanceToEnemy.z > enemyFinishDistance)
            {
               finishPosition.x = Mathf.Clamp(finishPosition.x, enemyPosition.x - enemyFinishDistance, enemyPosition.x + enemyFinishDistance);
               finishPosition.z = Mathf.Clamp(finishPosition.z, enemyPosition.z - enemyFinishDistance, enemyPosition.z + enemyFinishDistance);
            }
            else
            {
               finishPosition = transform.position;
            }

            isCanControl = false;
            transform.position = finishPosition;
            transform.rotation = Quaternion.LookRotation(enemyPosition - transform.position, Vector3.up);
                
            animator.SetTrigger("isFinishing");
            isCanFinish = false;
            enemyObj.Die();
            }
        else
        {
           isCanControl = true;
           ChangeWeapon();
        }
    }
}
