using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private InputMaster controls;

    private Camera cam;
    private RaycastHit rayHit;

    [SerializeField] private float bulletRange;
    [SerializeField] private float fireRate, reloadTime;
    [SerializeField] private bool isAutomaic;
    [SerializeField] private int magazineSize;
    private int ammoLeft;

    private bool isShooting, readyToShoot, reloading;

    [SerializeField] private GameObject bulletHolePrefab;
    [SerializeField] private float bulletHoleLifespan;
    //[SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private string EnemyTag;

    [SerializeField] private float horizontalSpread, verticalSpread, burstDelay;
    [SerializeField] private int bulletPerBurst;
    private int bulletsShot;

    private void Awake()
    {
        ammoLeft = magazineSize;
        readyToShoot = true;

        controls = new InputMaster();

        cam = Camera.main;

        controls.Movement.Shoot.started += ctx => StartShot();
        controls.Movement.Shoot.canceled += ctx => EndShot();

        controls.Movement.Reload.performed += ctx => Reload();
    }

    private void Update()
    {
        if(isShooting && readyToShoot && !reloading && ammoLeft > 0)
        {
            bulletsShot = bulletPerBurst;
            PerformShot();
        }
    }

    private void StartShot()
    {
        isShooting = true;
    }

    private void EndShot()
    {
        isShooting = false;
    }

    private void PerformShot()
    {
        readyToShoot = false;

        float x = Random.Range(-horizontalSpread, horizontalSpread);
        float y = Random.Range(-verticalSpread, verticalSpread);

        Vector3 direction = cam.transform.forward + new Vector3(x, y, 0);

        if(Physics.Raycast(cam.transform.position, direction, out rayHit, bulletRange))
        {
            Debug.Log(rayHit.collider.gameObject.name);
            if(rayHit.collider.gameObject.tag == EnemyTag)
            {
                // do enemy damage
            }
            else
            {
                GameObject bulletHole = Instantiate(bulletHolePrefab, rayHit.point + rayHit.normal * 0.0001f, Quaternion.identity) as GameObject;
                bulletHole.transform.LookAt(rayHit.point + rayHit.normal);
                Destroy(bulletHole, bulletHoleLifespan);
            }
        }

        //muzzleFlash.Play();

        ammoLeft--;

        bulletsShot--;

        if(bulletsShot > 0 && ammoLeft > 0)
        {
            Invoke("ResumeBurst", burstDelay);
        }
        else
        {
            Invoke("ResetShot", fireRate);

            if(!isAutomaic)
            {
                EndShot();
            }
        }
    }
    private void ResumeBurst()
    {
        readyToShoot = true;
        PerformShot();
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        ammoLeft = magazineSize;
        reloading = false;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
