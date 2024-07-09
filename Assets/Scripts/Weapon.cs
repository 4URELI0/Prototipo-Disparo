using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class Weapon : MonoBehaviour
{
    public Camera playerCamera;
    //Disparando
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 3f;
    //Explosion
    public int bulletPerBust = 3;
    public int burstBulletLeft;
    //Intensidad de dispersion
    public float spreadIntensity;

    //Bala
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;//segundos

    public GameObject muzzleEffect;
    //Animacion
    private Animator animator;
    
    /*Modo de disparo*/
    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }
    public ShootingMode currentShootingMode;
    private void Awake()
    {
        readyToShoot = true;
        burstBulletLeft = bulletPerBust;
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (currentShootingMode == ShootingMode.Auto)
        {
            //Mantener presionado el boton izquierdo del mouse
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
        {
            //Hacer click solo una vez 
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        if (readyToShoot && isShooting)
        {
            burstBulletLeft = bulletPerBust;
            FireWeapon();
        }
    }
    void FireWeapon() 
    {
        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");
        readyToShoot = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
        //Instanciar la bala
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        //Apuntar la bala hacia la dirección de disparo
        bullet.transform.forward = shootingDirection;
        //Disparar la bala
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        //Destruir a la bala despues de un tiempo(ya se encontrara la manera de implementar object pooling)
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
        //Controlar si estamos disparando
        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        if (currentShootingMode == ShootingMode.Burst && burstBulletLeft > 1)
        {
            burstBulletLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }
    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }
    public Vector3 CalculateDirectionAndSpread()
    {
        //Disparando desde el centro de la pantalla para comprobar hacia dónde apuntamos
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            //Golpear contra algo
            targetPoint = hit.point;
        }
        else
        {
            //Sin ningun objetivo de disparo
            targetPoint = ray.GetPoint(100);
        }
        Vector3 direction = targetPoint - bulletSpawn.position;
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        //Volviendo a la dirección de disparo y extendiéndose.
        return direction + new Vector3(x, y, 0);
    }
    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}