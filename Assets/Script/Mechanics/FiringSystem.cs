using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class FiringSystem : MonoBehaviour
{
    //Gun Config
    [Header ("Gun Configuration")]
    public int minDamage; 
    public int maxDamage; 
    public float timeShooting,recoil, normalSpread,movingSpread, range, timeReload;
    public int magazineSize;
    public bool fullAutoWeapon;
    public int bulletsInMagazine;
    float spread;

    //combat state
    public bool shooting;
    public bool ready;
    public bool reloading;

    [Header("Graphics")]
    public Color hitColor = Color.red;
    private Color originalColor;
    private Image[] crosshairImages;

    private Image uiImage;
    private SpriteRenderer spriteRenderer;

    private AmmoManager ammoManager;
    public TextMeshProUGUI reloadText;

    //reference Setting
    [Header("References Configuration")]
    public Camera cam;
    public Transform aimingPoint;
    public RaycastHit rayHit;
    public LayerMask enemies;
    public Rigidbody rb;
    public GameObject Crosshair;
    //Graphics
    //camshake (from camera shake class)
    public CameraShake camShake;
    //gun shooting fx
    public GameObject muzzleFire, impactMark;
    public Animator animator;

    private Vector3 accumulatedRecoil = Vector3.zero;
    public PlayerMovement movement;
    [Header("Sound Settings")]
    public AudioClip shootingSound;
    public AudioClip reloadingSound;
    public AudioClip dryFiringSound;

    // Start is called before the first frame update
    void Start()
    {
      cam = GetComponentInParent<Camera>();
      aimingPoint = GameObject.Find("firePoint").GetComponent<Transform>();
      rb = GetComponentInParent<Rigidbody>();
      enemies = LayerMask.GetMask("Enemy" , "Wall", "Ground");
      movement = GetComponentInParent<PlayerMovement>();
      ammoManager = GameObject.Find("Gameplay").GetComponent<AmmoManager>();
      ammoManager.UpdateAmmo(bulletsInMagazine);
      reloadText = GameObject.Find("ReloadReminder").GetComponent<TextMeshProUGUI>();

        if (Crosshair != null)
        {
            crosshairImages = Crosshair.GetComponentsInChildren<Image>();

            if (crosshairImages.Length > 0)
            {
                originalColor = crosshairImages[0].color;
            }
        }

    }
    private IEnumerator HitMarkerEffect()
    {
        if (crosshairImages == null || crosshairImages.Length == 0) yield break;

        SetAllCrosshairColors(hitColor);

        yield return new WaitForSeconds(0.1f);

        SetAllCrosshairColors(originalColor);
    }
    private void SetAllCrosshairColors(Color targetColor)
    {
        foreach (Image img in crosshairImages)
        {
            if (img != null) img.color = targetColor;
        }
    }

    private void Awake() //Initialize the guns
    {
        bulletsInMagazine = magazineSize;
        ready = true;

    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isReloading", reloading);
        animator.SetBool("IsShooting", shooting);
        takeInput();

        CheckCrosshairTarget();
        // Check if the ammo count is zero and display a reload reminder
        if (bulletsInMagazine == 0)
        {
            reloadText.text = "Press R to Reload!";
        }
        else
        {
            reloadText.text = ""; // Clear the reload reminder text if there is ammo
        }
    }

    private void CheckCrosshairTarget()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, enemies))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                SetAllCrosshairColors(hitColor);
                return; 
            }
        }

        SetAllCrosshairColors(originalColor);
    }

    private void FixedUpdate()
    {
        debug();
    }

    private void takeInput()
    {
            //check is fullAuto enabled
            if (fullAutoWeapon && bulletsInMagazine >= 0)
                shooting = Input.GetButton("Fire1");
            else if(bulletsInMagazine >= 0)
                shooting = Input.GetButtonDown("Fire1");

            //check reload ability
            if (Input.GetButtonDown("Reload") && bulletsInMagazine < magazineSize && !reloading)
            {
                Reload();
            
            }

            //check shooting ability
            if ( ready && shooting && !reloading && bulletsInMagazine > 0)
            {
                Fire();
            }
            else if (ready && shooting && !reloading && bulletsInMagazine <= 0)
            {
                DryFire();
            }

    }
    private void DryFire()
    {
        ready = false; //already start shooting
        SoundManager.Instance.PlaySound(dryFiringSound);
        Invoke("resetFire", 1f);
    }
    // player action
    private void Fire()
    {
        /*ready = false; //already start shooting 
        animator.SetTrigger("Firing");
        SoundManager.Instance.PlaySound(shootingSound);

        //apply recoil and gun spreads
        Vector3 shootingDirection = cam.transform.forward;
        shootingDirection += applySpread(cam.transform.forward);
        shootingDirection += applyRecoil(shootingDirection);*/
        ready = false;
        animator.SetTrigger("Firing");
        SoundManager.Instance.PlaySound(shootingSound);

        Vector3 shootingDirection = cam.transform.forward;

        shootingDirection += applySpread(shootingDirection);

        //Raycast
        if (Physics.Raycast(cam.transform.position, shootingDirection,out rayHit, range, enemies))
        {
            Debug.Log("Hit: " + rayHit.collider.name + " | Tag: " + rayHit.collider.tag);
                IDamageable target = rayHit.collider.GetComponentInParent<IDamageable>();
                if (target != null)
                {   
                    int baseDamage = Random.Range(minDamage, maxDamage + 1);
                    float distance = rayHit.distance; //distance matter
                    float damageMultiplier = Mathf.Clamp01(1 - (distance / range));
                    int finalDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);

                    finalDamage = Mathf.Max(1, finalDamage); //avoid it become 0 damage
                    target.TakeDamage(finalDamage);
                    Debug.Log("Damaged value"+finalDamage);
                    GameObject impactObj = Instantiate(impactMark, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                    impactObj.transform.localScale = Vector3.one * 0.1f;
                    Destroy(impactObj, 5f);
                }
        }
         else
        {
            Debug.Log("Raycast hit nothing.");
        }
        ApplyCameraRecoil();

        //shakeCameraHere
        CameraShake camShake = cam.GetComponent<CameraShake>();
        if (camShake != null)
        {
            //shake duration and magnitude
            camShake.ShakeCamera(0.1f, recoil);
        }

        //muzzle fire effect
        if (muzzleFire != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFire, aimingPoint.position, Quaternion.identity);
            Destroy(muzzleFlash, 0.2f);
        }
        bulletsInMagazine--;
        ammoManager.UpdateAmmo(bulletsInMagazine);
        Invoke("resetFire", timeShooting); //make the time gaps between each bullet shot from gun

    }

    private void ApplyCameraRecoil()
    {
        PlayerLook playerLook = cam.GetComponent<PlayerLook>();
        if (playerLook != null)
        {
            playerLook.xRotation -= (recoil * 10f);
        }
    }
    private void resetFire()
    {
        ready = true;
        animator.ResetTrigger("Firing");
        if (!shooting)
        {
            accumulatedRecoil = Vector3.zero;
        }
    }
    private void Reload()
    {
        Debug.Log("Reloading...");
        animator.SetTrigger("Reloading");
        SoundManager.Instance.PlaySound(reloadingSound);
        accumulatedRecoil = Vector3.zero;
        reloading = true;
        Invoke("resetReload", timeReload);
    }
    private void resetReload()
    {
        bulletsInMagazine = magazineSize;
        ammoManager.UpdateAmmo(bulletsInMagazine);
        reloading = false;
    }

    //gun mechanism
    /*private Vector3 applyRecoil(Vector3 shootingDirection)
    {

        //accumulatedRecoil += recoil * cam.transform.up;
        if (movement.currentState == PlayerMovement.MovementState.crouching)
        {
            accumulatedRecoil += recoil * cam.transform.up * 0.5f;
        }
        else
        {
            accumulatedRecoil += recoil * cam.transform.up;
        }
        shootingDirection += accumulatedRecoil;
        PlayerLook playerLook = cam.GetComponent<PlayerLook>();
        playerLook.xRotation -= (recoil*10);

        return shootingDirection;
    }*/
    
    /*private Vector3 applySpread(Vector3 shootingDirection)
    {
        // if player is moving then spread become more higher else is normal when standing
        switch (movement.currentState)
        {

            case PlayerMovement.MovementState.crouching:
                spread = normalSpread * 0.5f;
                break;
            case PlayerMovement.MovementState.sprinting:
                spread = movingSpread;
                break;
            default:
                spread = normalSpread;
                break;

        }
        shootingDirection += Random.Range(-spread, spread) * cam.transform.right;
        return shootingDirection;
    }
    */
    private Vector3 applySpread(Vector3 baseDirection)
    {
        switch (movement.currentState)
        {
            case PlayerMovement.MovementState.crouching:
                spread = normalSpread * 0.5f;
                break;
            case PlayerMovement.MovementState.sprinting:
                spread = movingSpread;
                break;
            default:
                spread = normalSpread;
                break;
        }

        return Random.Range(-spread, spread) * cam.transform.right +
               Random.Range(-spread, spread) * cam.transform.up;
    }


    public void debug()
    {
        Debug.DrawRay(cam.transform.position, cam.transform.forward * range, Color.red, 0.1f);
    }
}
