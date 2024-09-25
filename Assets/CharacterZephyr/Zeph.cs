using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Zeph : MonoBehaviour
{
    //CtrlK + CtrlF
    //F12

    //Player
    public Transform activeTransform, boxCollider;
    public Transform ocean, planet, cutsceneManager, zephHud;
    public Animator animatorRig, animatorCine, animatorFixe;
    public GameObject swordSheated, airBall;
    public TextMeshProUGUI hpText, enemyHpText;
    public float newFly, t = 0f;
    Vector3 oldRotation;

    //Suit
    public GameObject swordDrawnSuit, swordPedestalSuit, airBallSuit, swordTrailSuit, sparkSuit;
    public Transform suitRig, suitCine;
    //Armor
    public GameObject swordDrawnArmor, swordPedestalArmor, airBallArmor, swordTrailArmor, sparkArmor;
    public Transform armorRig, armorCine;
    //Flying
    public GameObject airBallFlying;
    //All
    public GameObject swordDrawn, swordPedestal, swordTrail, airBallPos, spark;

    //Actions
    float movementSpeed = 15, rotationSpeed = 500;
    public bool isFlying = false, isSwimming = false, isClimbing = false, isParrying = false;
    private bool isFalling = false, isHitting = false, canCombo = false;
    private bool hasSword = false, inCutscene = false;
    private int health = 5;
    char key = 'k';
    private RaycastHit hit, hitClimb;
    public LayerMask mask, maskClimb;

    //Story
    private bool unlockedSword = false;

    //Inputs
    public float inputX, inputY;
    bool isHit = false;
    string keyPressed;
    public int attackDmg = 0;
    bool hasArmor = false;

    //Cutscenes
    public GameObject kingBody;
    Vector3[] positions = new Vector3[10];
    Vector3[] rotations = new Vector3[10];

    //Camera
    float shake = 0f;
    public bool isShaking = false;
    public Transform cam;
    public bool isRotating = false;

    //Particules
    public GameObject flyingTrail;

    //Sounds
    public GameObject draw, sheathe, run, jump, slash, punch, magic, swim, fly, fall, slide, impact;


    private void Awake()
    {
        //FinKing
        positions[1] = new Vector3(217, 667, -506);
        //FinLake
        positions[2] = new Vector3(201, 533, -613);
        //FinRope
        positions[3] = new Vector3(-101, 918, -24);
        //FinClimb
        positions[4] = new Vector3(-40, 1083, 111);
        //FinKing
        rotations[1] = new Vector3(-20, -90, 45);
        //FinLake
        rotations[2] = new Vector3(39, 248, 55);
        //FinRope
        rotations[3] = new Vector3(0, -90, 0);
        //FinClimb
        rotations[4] = new Vector3(0, 0, 0);

    }

    private void FixedUpdate()
    {
        //Assign suit/armor
        animatorRig = hasArmor ? armorRig.gameObject.GetComponent<Animator>() : suitRig.gameObject.GetComponent<Animator>();
        animatorCine = hasArmor ? armorCine.gameObject.GetComponent<Animator>() : suitCine.gameObject.GetComponent<Animator>();
        activeTransform = hasArmor ? armorCine : suitCine;
        activeTransform.gameObject.SetActive(true);
        airBallPos = hasArmor ? (isFlying ? airBallFlying : airBallArmor) : airBallSuit;
        swordDrawn = hasArmor ? swordDrawnArmor : swordDrawnSuit;
        swordDrawn.SetActive(hasSword);
        swordPedestal = hasArmor ? swordPedestalArmor : swordPedestalSuit;
        swordTrail = hasArmor ? swordTrailArmor : swordTrailSuit;
        spark = hasArmor ? sparkArmor : sparkSuit;
        //Gameplay
        movementSpeed = isFlying ? 75 : (isClimbing ? 10 : (animatorRig.GetBool("Jump") ? 25 : 15));
        LerpFly();
        isRotating = false;
        //HUD
        zephHud.gameObject.SetActive(hasArmor ? true : false);
        hpText.text = "Health: " + health;
        enemyHpText.text = "Health: " + kingBody.GetComponent<KingBoss>().health;
        //Raycasts
        Debug.DrawRay(activeTransform.position + activeTransform.TransformVector(0, 5, 0), activeTransform.forward, Color.red, 4);
        isHitting = Physics.Raycast(activeTransform.position + activeTransform.TransformVector(0, 5, 0), activeTransform.forward, out hit, 4f, mask) ? true : false;
        isFalling = (Physics.Raycast(activeTransform.position + activeTransform.TransformVector(0, 1, 0), -activeTransform.up, out hit, 2f, mask) ? false : true) && !isFlying;
        isClimbing = (Physics.Raycast(activeTransform.position, activeTransform.forward + activeTransform.up / 4, out hitClimb, 2f, maskClimb) ? true : false) && Input.GetKey("z");
        //Movement
        if (!inCutscene && newFly > -3)
        {
            if (isClimbing)
            {
                Climb();
            }
            else
            {
                if (key == 'k' || key == 's' || isFlying)
                {
                    Move();
                }
                if (isSwimming)
                {
                    Swim();
                }
                else
                {
                    Fly();
                    if (!isFalling && key == 'k')
                    {
                        Attack();
                    }
                    if (!isFlying)
                    {
                        Jump();
                        if (!isFalling)
                        {
                            Armor();
                            Slide();
                            Parry();
                        }
                    }
                }
            }
        }
        //Damage
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
        if (isShaking)
        {
            ShakeCamera();
        }
        //Sounds
        run.SetActive((animatorRig.GetFloat("InputY") != 0 || animatorRig.GetFloat("InputX") != 0) && animatorRig.GetCurrentAnimatorStateInfo(0).IsName("MovementTree"));
        swim.SetActive(isSwimming);
        fly.SetActive(animatorRig.GetFloat("Fly") == -1 && animatorRig.GetCurrentAnimatorStateInfo(0).IsName("MovementTree"));
        fall.SetActive(animatorRig.GetFloat("Fly") == 1 && animatorRig.GetCurrentAnimatorStateInfo(0).IsName("MovementTree"));
    }

    private void LerpFly()
    {
        if (animatorRig.GetFloat("Fly") != newFly)
        {
            t += 0.5f * Time.deltaTime;
            animatorRig.SetFloat("Fly", Mathf.Lerp(animatorRig.GetFloat("Fly"), newFly, t));
            if (t > 1f)
            {
                t = 0f;
            }
        }
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            transform.position += activeTransform.forward * Time.deltaTime * movementSpeed;
            isRotating = true;
        }
        if (!isFlying && !isSwimming)
        {
            if (Input.GetKey(KeyCode.S))
            {
                transform.position -= activeTransform.forward * Time.deltaTime * movementSpeed;
                isRotating = true;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += activeTransform.right * Time.deltaTime * movementSpeed;
                isRotating = true;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                transform.position -= activeTransform.right * Time.deltaTime * movementSpeed;
                isRotating = true;
            }
            if (!isFalling && !isSwimming)
            {
                inputY = Input.GetAxis("Vertical");
                animatorRig.SetFloat("InputY", inputY);
                inputX = Input.GetAxis("Horizontal");
                animatorRig.SetFloat("InputX", inputX);
            }
        }
    }

    private void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && key == 'k' && !isFlying && !isFalling)
        {
            key = 's';
            StartCoroutine(ResetKey(1f));
            StartCoroutine(PlaySfx(jump, 0, .2f));
            GetComponent<Rigidbody>().AddRelativeForce(0, 400, 0);
        }
        if (isFalling)
        {
            newFly = 1;
            animatorRig.SetFloat("InputY", 0);
            animatorRig.SetFloat("InputX", 0);
        }
        if (!isFalling && animatorRig.GetFloat("Fly") == 1f)
        {
            newFly = 0;
        }
    }

    private void Slide()
    {
        if (Input.GetKey(KeyCode.LeftShift) && key == 'k')
        {
            key = 'l';
            StartCoroutine(ResetKey(1.5f));
            StartCoroutine(PlaySfx(slide, 0, .5f));
            GetComponent<Rigidbody>().AddRelativeForce(transform.InverseTransformVector(activeTransform.TransformVector(0, 0, 1000)));
            StartCoroutine(ActivateAnimation("Slide", 1.5f, 0));
        }
    }

    private void Climb()
    {
        newFly = 2;
        animatorRig.SetFloat("InputY", 0);
        animatorRig.SetFloat("InputX", 0);
        transform.position += activeTransform.up * Time.deltaTime * movementSpeed;
    }

    private void Fly()
    {
        flyingTrail.SetActive(isFlying ? true : false);
        if (hasArmor == true && Input.GetKey(KeyCode.A) && key == 'k')
        {
            key = 'a';
            StartCoroutine(ResetKey(.2f));
            if (isFlying)
            {
                newFly = 0;
                animatorRig.SetFloat("InputY", 0);
                animatorRig.SetFloat("InputX", 0);
                /*if (Mathf.Sqrt(Mathf.Pow(activeTransform.position.x - planet.position.x, 2) +
                                                            Mathf.Pow(activeTransform.position.y - planet.position.y, 2) +
                                                            Mathf.Pow(activeTransform.position.z - planet.position.z, 2)) > 828.7)
                {
                    isFalling = true;
                }*/
            }
            else
            {
                newFly = -1;
            }
            isFlying = !isFlying;
        }

    }

    private void Swim()
    {
        Vector3 directionHaut = (planet.position - activeTransform.position).normalized;
        if (Mathf.Sqrt(Mathf.Pow(transform.position.x - planet.position.x, 2) +
                                                  Mathf.Pow(transform.position.y - planet.position.y, 2) +
                                                  Mathf.Pow(transform.position.z - planet.position.z, 2)) < 803.3)
        {
            transform.position += transform.up * Time.deltaTime * movementSpeed;
        }
        GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().velocity * -2);
        newFly = -2;
        animatorRig.SetFloat("InputY", 0);
        animatorRig.SetFloat("InputX", 0);
    }

    private void Parry()
    {
        if (Input.GetKey(KeyCode.R))
        {
            key = 'r';
            isParrying = true;
            animatorRig.SetBool("Parry", true);
            StartCoroutine(ResetKey(1f));
        }
        else
        {
            isParrying = false;
            animatorRig.SetBool("Parry", false);
        }
    }

    private void Attack()
    {
        if (Input.GetMouseButton(0))
        {
            key = '0';
            if (isFlying)
            {
                StartCoroutine(ResetKey(2.5f));
                StartCoroutine(Shake(.5f, .4f));
                StartCoroutine(Shake(.6f, .4f));
                StartCoroutine(Shake(.7f, .4f));
                StartCoroutine(Shake(.8f, .4f));
                StartCoroutine(Shake(.9f, .4f));
                StartCoroutine(Shake(1f, .4f));
                StartCoroutine(ActivateAnimation("Throw", 2.5f, 0));
                StartCoroutine(PlaySfx(magic, 0, 2.5f));
                StartCoroutine(Magic());
            }
            else
            {
                if (hasSword)
                {
                    if (isHitting)
                    {
                        StartCoroutine(Shake(.7f, .7f));
                    }
                    if (canCombo)
                    {
                        StartCoroutine(ResetKey(1.2f));
                        StartCoroutine(ActivateAnimation("SlashCombo", 1.2f, 3));
                        StartCoroutine(PlaySfx(slash, .3f, .5f));
                    }
                    else
                    {
                        StartCoroutine(ResetKey(1f));
                        StartCoroutine(ActivateAnimation("Slash", 1.2f, 2));
                        StartCoroutine(CheckCombo(1.2f));
                        StartCoroutine(PlaySfx(slash, .5f, .5f));
                        canCombo = true;
                    }
                }
                else
                {
                    if (isHitting)
                    {
                        StartCoroutine(Shake(.4f, .7f));
                    }
                    if (canCombo)
                    {
                        StartCoroutine(ResetKey(1.2f));
                        StartCoroutine(ActivateAnimation("PunchCombo", 1.2f, 2));
                        StartCoroutine(PlaySfx(punch, .1f, .5f));
                    }
                    else
                    {
                        StartCoroutine(ResetKey(.6f));
                        StartCoroutine(CheckCombo(0.8f));
                        StartCoroutine(ActivateAnimation("Punch", 0.8f, 1));
                        StartCoroutine(PlaySfx(punch, .1f, .5f));
                        canCombo = true;
                    }
                }
            }
        }

        if (Input.GetMouseButton(1))
        {
            key = '1';
            if (isFlying)
            {
                StartCoroutine(ResetKey(2.5f));
                StartCoroutine(Shake(.5f, .4f));
                StartCoroutine(Shake(.6f, .4f));
                StartCoroutine(Shake(.7f, .4f));
                StartCoroutine(Shake(.8f, .4f));
                StartCoroutine(Shake(.9f, .4f));
                StartCoroutine(Shake(1f, .4f));
                StartCoroutine(ActivateAnimation("Throw", 2.5f, 0));
                StartCoroutine(PlaySfx(magic, 0, 2.5f));
                StartCoroutine(Magic());
            }
            else
            {
                if (hasSword)
                {
                    StartCoroutine(ResetKey(1.7f));
                    if (isHitting)
                    {
                        StartCoroutine(Shake(1f, .7f));
                    }
                    StartCoroutine(PlaySfx(slash, .8f, .3f));
                    StartCoroutine(ActivateAnimation("Spin", 1.7f, 3));
                }
                else
                {
                    StartCoroutine(ResetKey(2.9f));
                    StartCoroutine(Shake(.5f, .4f));
                    StartCoroutine(Shake(.6f, .4f));
                    StartCoroutine(Shake(.7f, .4f));
                    StartCoroutine(Shake(.8f, .4f));
                    StartCoroutine(Shake(.9f, .4f));
                    StartCoroutine(Shake(1f, .4f));
                    StartCoroutine(ActivateAnimation("Magic", 2.9f, 0));
                    StartCoroutine(PlaySfx(magic, 0, 2.5f));
                    StartCoroutine(Magic());
                }
            }

        }

        if (Input.GetMouseButton(2))
        {
            key = '2';
            DrawSword();
            /*if (unlockedSword)
            {
                DrawSword();
            }*/
        }
    }

    IEnumerator CheckCombo(float time)
    {
        yield return new WaitForSeconds(time);
        canCombo = false;
    }

    private void Armor()
    {
        if (Input.GetKey(KeyCode.F) && key =='k' && !isFlying)
        {
            key = 'f';
            StartCoroutine(ResetKey(.2f));
            activeTransform.gameObject.SetActive(false);
            hasArmor = !hasArmor;
        }
    }

    private void DrawSword()
    {
        if (!hasSword)
        {
            if (!hasArmor)
            {
                StartCoroutine(ResetKey(1.2f));
                StartCoroutine(ActivateAnimation("Draw", 1.2f, 0));
                StartCoroutine(Draw());
                StartCoroutine(PlaySfx(draw, .5f, .5f));
            }
            else
            {
                hasSword = true;
                StartCoroutine(ResetKey(.2f));
            }
        }
        else
        {
            if (!hasArmor)
            {
                StartCoroutine(ResetKey(1.4f));
                StartCoroutine(ActivateAnimation("Sheath", 1.4f, 0));
                StartCoroutine(Sheath());
                StartCoroutine(PlaySfx(sheathe, .5f, .5f));
            }
            else
            {
                hasSword = false;
                StartCoroutine(ResetKey(.2f));
            }
        }
    }

    private IEnumerator Draw()
    {
        yield return new WaitForSeconds(0.5f);
        swordSheated.SetActive(false);
        hasSword = true;
    }

    private IEnumerator Sheath()
    {
        yield return new WaitForSeconds(0.65f);
        swordSheated.SetActive(true);
        hasSword = false;
    }

    private IEnumerator Magic()
    {
        yield return new WaitForSeconds(.5f);
        airBallPos.SetActive(true);
        yield return new WaitForSeconds(2.3f);
        airBallPos.SetActive(false);
        Instantiate(airBall, airBallPos.transform.position, cam.rotation);
    }

    public void SetDamage(int damage)
    {
        if (!isHit && !isParrying && damage > 0)
        {
            StartCoroutine(ActivateAnimation("Stun", 1.4f, 0));
            GetComponent<Rigidbody>().AddForce(kingBody.transform.forward * 600);
            GetComponent<Rigidbody>().AddRelativeForce(0, 400, 0);
            health -= damage;
            StartCoroutine(ResetHit());
        }
    }

    private IEnumerator ResetHit()
    {
        isHit = true;
        yield return new WaitForSeconds(2f);
        isHit = false;
    }

    private IEnumerator ResetKey(float time)
    {
        yield return new WaitForSeconds(time);
        key = 'k';
    }

    public void PlayCutscene(string triggerName, int waitTime)
    {
        if (waitTime != 0)
        {
            swordDrawn.SetActive(false);
            if (unlockedSword)
            {
                swordSheated.SetActive(true);
            }
        }
        switch (triggerName)
        {
            case "TriggerReynal":
                StartCoroutine(PlayAnimator(waitTime, 1));
                animatorRig.SetFloat("InputX", 0);
                animatorRig.SetFloat("InputY", 0);
                break;
            case "TriggerReynal3":
                StartCoroutine(PlayAnimator(waitTime, 2));
                animatorRig.SetFloat("InputX", 0);
                animatorRig.SetFloat("InputY", 0);
                break;
            case "TriggerRope":
                StartCoroutine(ActivateAnimation("Pedestal", 4f, 0));
                StartCoroutine(PlayAnimator(waitTime, 3));
                swordPedestal.SetActive(true);
                break;
            case "TriggerClimb":
                StartCoroutine(ActivateAnimation("ClimbRope", 38f, 0));
                StartCoroutine(PlayAnimator(waitTime, 4));
                break;
        }
    }

    private IEnumerator PlayAnimator(float time, int index)
    {
        inCutscene = true;
        animatorCine.enabled = true;
        animatorFixe.enabled = true;
        animatorCine.Play(index.ToString(), -1, 0f);
        animatorFixe.Play(index.ToString(), -1, 0f);
        yield return new WaitForSeconds(time);
        animatorFixe.enabled = false;
        animatorCine.enabled = false;
        inCutscene = false;
        Teleport(index, index);
        //Specific
        swordPedestal.SetActive(false);
        if (index == 1)
        {
            unlockedSword = true;
            swordSheated.SetActive(true);
        }
    }

    private void Teleport(int indexPos, int indexRot)
    {
        transform.position = positions[indexPos];
        transform.eulerAngles = rotations[indexRot];
        activeTransform.transform.localPosition = hasArmor ? new Vector3(0, 1, 0) : new Vector3(0, 0, 0);
        activeTransform.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    private IEnumerator ActivateAnimation(string name, float time, int attack)
    {
        attackDmg = attack;
        animatorRig.SetBool(name, true);
        //Specific
        if (attack != 0)
        {
            swordTrail.SetActive(true);
            swordTrail.GetComponent<MeleeWeaponTrail>().Initialize();
        }
        yield return new WaitForSeconds(time - .2f);
        //Specific
        swordTrail.SetActive(false);
        yield return new WaitForSeconds(.2f);
        animatorRig.SetBool(name, false);
        attackDmg = 0;
        animatorRig.SetFloat("InputY", 0f);
        animatorRig.SetFloat("InputX", 0f);
    }

    private void ShakeCamera()
    {
        if (shake > 0f)
        {
            cam.localPosition = cam.localPosition + Random.insideUnitSphere * shake;
            shake -= 2 * Time.deltaTime;
        }
        else
        {
            isShaking = false;
        }
    }

    public IEnumerator Shake(float time, float intensity)
    {
        yield return new WaitForSeconds(time - .1f);
        //Specific
        if(attackDmg > 0)
        {
            StartCoroutine(PlaySfx(impact, 0, .5f));
        }
        yield return new WaitForSeconds(.1f);
        isShaking = true;
        shake = intensity;
        //Specific
        if (attackDmg > 0)
        {
            spark.SetActive(true);
        }
        yield return new WaitForSeconds(.5f);
        //Specific
        spark.SetActive(false);
    }

    private IEnumerator PlaySfx(GameObject sfx, float start, float wait)
    {
        yield return new WaitForSeconds(start);
        sfx.SetActive(true);
        yield return new WaitForSeconds(wait);
        sfx.SetActive(false);
    }

    private IEnumerator Die()
    {
        attackDmg = 0;
        newFly = -3;
        yield return new WaitForSeconds(5.3f);
        newFly = -4;
        inCutscene = true;
        gameObject.SetActive(false);
    }
}