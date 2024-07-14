using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using Input = UnityEngine.Input;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    //estad�sticas de armas
    [Header("Da�o del arma")]
    public int damage;
    [Header("El tiempo que pasa tras cada disparo")]
    public float timeBeetwenShooting;
    [Header("Dispersion de los disparos")]
    public float spread;
    [Header("El alcance de la bala")]
    public float range;
    [Header("El tiempo de recarga del arma")]
    public float reloadTime;
    [Header("El tiempo que pasa entre cada bala cuando hay multiples disparos")]
    public float timeBeetwenShots;
    [Header("El cargador del arma")]
    public int magazineSize;
    private int bulletPerTap = 1;
    [Header("Indica si permite mantener presionado el boton para disparar continuamente")]
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //Booleanos
    bool shooting, readyToShoot, reloading;

    //Referencias
    [Header("Camara del jugador")]
    public Camera fpsCam;
    [Header("Respawn de la bala")]
    public Transform attackPoint;
    [Header("Objeto impactado")]
    public RaycastHit rayHit;
    [Header("Enemigo que se puede bajar la vida")]
    public LayerMask whatIsEnemy;
    //Camara shake 5:33

    //Particulas
    [Header("Fogonazo del arma")]
    public GameObject muzzleEffect;
    [Header("Animacion de cartucho o bala")]
    public GameObject catridge;
    private ParticleSystem muzzleParticleSystem;
    private ParticleSystem catridgeParticleSystem;
    [Header("Humo al disparar")]
    [SerializeField] ParticleSystem smoke;

    private AudioSource randomSource;
    [Header("Sonido de disparos")]
    public AudioClip[] audioShot;

    //Text
    [Header("Texto")]
    public Text ammoText;

    private void Awake()
    {
        muzzleParticleSystem = muzzleEffect.GetComponent<ParticleSystem>();
        catridgeParticleSystem = catridge.GetComponent<ParticleSystem>();
        randomSource = GetComponent<AudioSource>();
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    private void Update()
    {
        MyInput();
        ammoText.text = bulletsLeft + "/" + magazineSize;
    }
    private void MyInput()
    {
        if (allowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }
        //Disparar
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletPerTap;
            Shoot();
        }
    }
    private void Shoot()
    {
        readyToShoot = false;
        muzzleParticleSystem.Play();
        catridgeParticleSystem.Play();
        smoke.Play();
        RandomShot();
        //dispersion de bala
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calcular direccion de la dispersion
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        //RayCast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy))
        {
            Debug.Log(rayHit.collider.name);

            //Usar este if para el ucumar
            /*if (rayHit.collider.CompareTag("Enemy"))
            {
                rayHit.collider.GetComponent<ShootingAI>().TakeDamage(damage);
            }*/
        }

        /*Particulas*/
        //Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));

        bulletsLeft--;
        bulletsShot--;
        Invoke("ResetShot", timeBeetwenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBeetwenShots);
            Debug.Log("Disparo!");
        }
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
        bulletsLeft = magazineSize;
        reloading = false;
    }
    private void RandomShot()
    {
        randomSource.clip = audioShot[Random.Range(0, audioShot.Length)];
        randomSource.Play();
    }
}