using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurretState
{
    IDLE,
    COOLDOWN,
    ACTIVE,
    DESTROYED
}

public class TurretTest : MonoBehaviour
{
    [SerializeField]private GameObject turretTop;

    [SerializeField]private GameObject turretBase;
    [SerializeField]private GameObject leftGun;
    [SerializeField]private GameObject rightGun;

    [SerializeField] private GameObject _player;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _bulletPrefab;
    public float rotationSpeed = 2;
    public float bulletSpeed = 10;

    public float activeRadius = 5f;

    public static TurretState turretState;

    private bool shootCoroutineRunning = false;
    // Start is called before the first frame update
    void Start()
    {
        turretTop.transform.forward = new Vector3(0, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        HandleTurretPosition();
    }
    
    void HandleTurretPosition()
    {
        // cache frequently accessed fields
        var playerTransform = _player.transform;
        var turretTransform = turretTop.transform;
        // set the turret active
        if (Vector3.Distance(playerTransform.position, turretTransform.position) < activeRadius)
        {
            MoveTurretToTransform(playerTransform);
            if (!shootCoroutineRunning)
            {
                StartCoroutine(Shoot());
            }

        }
        else
        {
            // face turret towards the floor
            MoveTurretToTransform(turretBase.transform);
            _animator.SetBool("Shoot", false);
        }
    }

    /// <summary>
    /// Slowly lerps the turret's location towards the desired rotation
    /// </summary>
    void MoveTurretToTransform(Transform targetTransform)
    {
        var turretTransform = turretTop.transform;
        Quaternion targetRotation = Quaternion.LookRotation(targetTransform.position - turretTransform.position);
        turretTop.transform.rotation =
            Quaternion.Slerp(turretTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    IEnumerator Shoot()
    {
        Transform[] barrelTransforms = {leftGun.transform, rightGun.transform};
        if (!shootCoroutineRunning)
        {
            shootCoroutineRunning = true;
            _animator.SetBool("Shoot", true);
            foreach (var barrel in barrelTransforms)
            {
                // left first then right
                var bullet = Instantiate(_bulletPrefab, barrel.position + (barrel.forward),
                    Quaternion.LookRotation(_player.transform.position - barrel.position));
                bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * bulletSpeed, ForceMode.VelocityChange);
                yield return new WaitForSeconds(0.5f);
            }
            shootCoroutineRunning = false;
        }

    }
}
