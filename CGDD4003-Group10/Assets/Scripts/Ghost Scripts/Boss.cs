using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    //Inky attack = 0
    //Blinky attack = 1
    //Pinky attack = 2

    public int blinkysKilled { get; private set;}
    public int inkysKilled { get; private set; }
    public int pinkysKilled { get; private set; }
    public int clydesKilled { get; private set; }

    struct AttackChoice
    {
        public int attackID;
        public float weight;
        public AttackChoice(int id, float weight)
        {
            attackID = id;
            this.weight = weight;
        }
    }

    public struct BossHitInformation
    {
        public int pointWorth;
    }

    public enum BossState { Initial, Chase, AttackCooldown, Dash, Slam, Attack, Enrage }
    public BossState currentState { get; private set; }

    PlayerController player;

    Animator animator;
    Rigidbody rb;

    [Header("Boss Heads")]
    [SerializeField] BossHead inkyHead;
    [SerializeField] BossHead blinkyHead;
    [SerializeField] BossHead pinkyHead;
    [SerializeField] BossHead clydeHead;

    [Header("Movement Settings")]
    [SerializeField] [Range(0,100)] float rotationSmoothing = 0.5f;
    [SerializeField] float movementSpeed;

    [Header("Attack Setup")]
    [SerializeField] Transform inkyProjSpawn;
    [SerializeField] Transform[] pinkyProjSpawn;
    [SerializeField] Transform clydeProjSpawn;
    [SerializeField] GameObject inkyProjectile;
    [SerializeField] GameObject pinkyProjectile;
    [SerializeField] GameObject clydeProjectile;
    [SerializeField] GameObject blinkyLaser;
    [SerializeField] LineRenderer[] blinkyLasers;
    [SerializeField] Transform[] blinkyLaserEnds;
    [SerializeField] Transform blinkyRaycastOrigin;

    [Header("Attack Settings")]
    [SerializeField] float normalAttackCooldown = 6;
    [SerializeField] float enrageAttackCooldown = 4;
    [SerializeField] float slamAttackCooldown = 2.5f;
    [SerializeField] float attackLeading = 2f;
    [SerializeField] float pinkySpread = 0.07f;
    [SerializeField] float blinkyAttackDuration = 5f;
    [SerializeField] LayerMask blinkyLayerMask;
    [SerializeField] float blinkyMaxRange = 20f;
    [SerializeField] [Range(0,100)] float blinkyRotationSmoothing = 10f;

    [Header("Slam Settings")]
    [SerializeField] float slamThreshold;
    [SerializeField] float slamStrength = 100;
    [SerializeField] float slamStunLength = 0.5f;

    [Header("Score Display Settings")]
    [SerializeField] GameObject scoreIncrementPrefab;
    [SerializeField] BoxCollider scoreIncrementSpawnArea;

    [Header("Damage Settings")]
    [SerializeField] GameObject bigInkyBloodPrefab;
    [SerializeField] GameObject smallInkyBloodPrefab;
    [SerializeField] GameObject bigBlinkyBloodPrefab;
    [SerializeField] GameObject smallBlinkyBloodPrefab;
    [SerializeField] GameObject bigPinkyBloodPrefab;
    [SerializeField] GameObject smallPinkyBloodPrefab;
    [SerializeField] GameObject bigClydeBloodPrefab;
    [SerializeField] GameObject smallClydeBloodPrefab;
    [SerializeField] int bloodAmountUponHeadDeath = 20;
    [SerializeField] int bloodAmountUponDeath = 40;
    [SerializeField] BoxCollider inkyBloodSpawnArea;
    [SerializeField] BoxCollider pinkyBloodSpawnArea;
    [SerializeField] BoxCollider blinkyBloodSpawnArea;
    [SerializeField] BoxCollider clydeBloodSpawnArea;
    [SerializeField] GameObject shieldPrefab;
    [SerializeField] float baseRailgunDamage;
    [SerializeField] float patienceMultiplier = 1.05f; // multiplier to the damage based on the amount of ghosts of a particular kind you kill before shooting the corresponding head

    AttackChoice[] attackChoices;

    float attackCooldownTimer;

    public int damage { get 
        {
            int d = (inkyHead.dead ? 1 : 0) + ((blinkyHead.dead ? 1 : 0) << 1) + ((pinkyHead.dead ? 1 : 0) << 2);
            return d; 
        } 
    }

    int currentAttack = 0;

    private void Start()
    {
        currentState = BossState.Initial;

        blinkysKilled = 0;
        inkysKilled = 0;
        pinkysKilled = 0;
        clydesKilled = 0;

        AttackChoice inky = new AttackChoice(0, 1f);
        AttackChoice blinky = new AttackChoice(1, 1f);
        AttackChoice pinky = new AttackChoice(2, 1f);
        attackChoices = new AttackChoice[3];
        attackChoices[0] = inky;
        attackChoices[1] = blinky;
        attackChoices[2] = pinky;

        player = FindObjectOfType<PlayerController>();

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        currentRotationSmoothing = rotationSmoothing;
    }

    private void Update()
    {
        Act();
        LookAtPlayer();
    }

    float currentRotationSmoothing;
    void LookAtPlayer()
    {
        Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
        dirToPlayer.y = 0;
        Quaternion rot = Quaternion.FromToRotation(transform.forward, dirToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot * transform.rotation, currentRotationSmoothing * Time.deltaTime);
    }

    public void IncrementKillCount(int id)
    {
        switch (id)
        {
            case 0:
                inkysKilled++;
                inkyHead.DeactivateShield();
                break;
            case 1:
                blinkysKilled++;
                blinkyHead.DeactivateShield();
                break;
            case 2:
                pinkysKilled++;
                pinkyHead.DeactivateShield();
                break;
            case 3:
                clydesKilled++;
                clydeHead.DeactivateShield();
                break;
        }
    }

    void Act()
    {
        switch(currentState)
        {
            case BossState.Initial:
                Initial();
                break;
            case BossState.AttackCooldown:
                AttackCooldown();
                break;
            case BossState.Chase:
                Chase();
                break;
            case BossState.Dash:
                break;
            case BossState.Attack:
                Attack();
                break;
            case BossState.Slam:
                break;
            case BossState.Enrage:
                break;
        }
    }

    void Initial()
    {
        currentState = BossState.Chase;
    }

    //For now the boss will just follow the same chasing pattern as blinky
    void Chase()
    {
        float distToPlayer = (player.transform.position - transform.position).magnitude;
        if (distToPlayer < slamThreshold)
        {
            currentState = BossState.Slam;
            SlamInitiate();
        }

        //currentState = BossState.Attack;
        /*Vector2Int playerGridPosition = map.CheckEdgePositions(transform.position);

        targetGridPosition = playerGridPosition;

        Move(false);

        lastTargetGridPosition = targetGridPosition;
        PlayChaseSound();

        if(attackTimer >= attackCooldown)
        {
            Attack();
        }*/
    }

    private void AttackCooldown()
    {
        attackCooldownTimer -= Time.deltaTime;

        if(attackCooldownTimer <= 0)
        {
            currentState = BossState.Chase;
        }
    }

    //For now it just does a simple random generator to select the next attack
    //In the future I will make it so that it can potentially be weighted and varied based on which heads are active or not
    private void Attack()
    {
        if (damage == 7)
        {
            animator.SetTrigger("ClydeAttack");

            attackCooldownTimer = enrageAttackCooldown;
        }
        else
        {
            currentAttack = 0;

            float totalWeight = 0;
            for (int i = 0; i < attackChoices.Length; i++)
            {
                totalWeight += attackChoices[i].weight;
            }
            float rand = Random.Range(0.1f, totalWeight);
            for (int i = 0; i < attackChoices.Length; i++)
            {
                if (rand <= attackChoices[i].weight)
                {
                    currentAttack = attackChoices[i].attackID;
                    break;
                }
                rand -= attackChoices[i].weight;
            }

            for (int i = 0; i < attackChoices.Length; i++) //Reset weights if head is not dead
            {
                if(attackChoices[i].weight > 0)
                    attackChoices[i].weight = 1;
            }

            if (currentAttack == 0)
            {
                animator.SetTrigger("InkyAttack");
            }
            else if (currentAttack == 1)
            {
                animator.SetBool("BlinkyAttacking", true);
                animator.SetTrigger("BlinkyAttack");

                currentRotationSmoothing = blinkyRotationSmoothing;
            }
            else if (currentAttack == 2)
            {
                animator.SetTrigger("PinkyAttack");
            }

            attackCooldownTimer = normalAttackCooldown;
        }

        currentState = BossState.AttackCooldown;
    }

    void ChangeAttackChoiceWeight(int attackID, float newWeight)
    {
        for (int i = 0; i < attackChoices.Length; i++)
        {
            if (attackChoices[i].attackID == attackID)
                attackChoices[i].weight = newWeight;
        }
    }
    
    //Animation Event
    private void PinkyAttack()
    {
        attackChoices[2].weight = 0.3f; //Add weight to current attack to make it less likely to happen twice in a row

        Vector3 dirToPlayer = (player.transform.position + player.velocity * attackLeading - transform.position).normalized;
        Vector3 dirToPlayer2 = (player.transform.position + player.velocity * attackLeading - player.transform.right * pinkySpread - transform.position).normalized;
        Vector3 dirToPlayer3 = (player.transform.position + player.velocity * attackLeading + player.transform.right * pinkySpread - transform.position).normalized;
        Quaternion rot = Quaternion.FromToRotation(transform.forward, dirToPlayer);
        Quaternion rot2 = Quaternion.FromToRotation(transform.forward, dirToPlayer2);
        Quaternion rot3 = Quaternion.FromToRotation(transform.forward, dirToPlayer3);

        GameObject proj1 = Instantiate(pinkyProjectile, pinkyProjSpawn[0].position, transform.rotation);
        GameObject proj2 = Instantiate(pinkyProjectile, pinkyProjSpawn[1].position, transform.rotation);
        GameObject proj3 = Instantiate(pinkyProjectile, pinkyProjSpawn[2].position, transform.rotation);
        proj1.transform.rotation = rot * proj1.transform.rotation;
        proj2.transform.rotation = rot2 * proj2.transform.rotation;
        proj3.transform.rotation = rot3 * proj3.transform.rotation;
    }

    //Animation Event
    private void BlinkyAttack()
    {
        attackChoices[1].weight = 0.3f; //Add weight to current attack to make it less likely to happen twice in a row
        StartCoroutine(BlinkyLaser());
    }
    IEnumerator BlinkyLaser()
    {
        blinkyLaser.gameObject.SetActive(true);

        float timer = 0;
        while (timer < blinkyAttackDuration)
        {
            float distance = blinkyMaxRange;
            RaycastHit hitInfo = new RaycastHit();
            if(Physics.Raycast(blinkyRaycastOrigin.position, blinkyRaycastOrigin.transform.forward, out hitInfo, blinkyMaxRange, blinkyLayerMask))
            {
                distance = hitInfo.distance;
            }

            for (int i = 0; i < blinkyLasers.Length; i++)
            {
                blinkyLasers[i].SetPosition(1, Vector3.forward * distance * .7f);
            }
            for (int i = 0; i < blinkyLaserEnds.Length; i++)
            {
                blinkyLaserEnds[i].localPosition = Vector3.forward * distance * .7f;
            }

            yield return null;
            timer += Time.deltaTime;
        }

        animator.SetBool("BlinkyAttacking", false);

        blinkyLaser.gameObject.SetActive(false);

        currentRotationSmoothing = rotationSmoothing;
    }

    //Animation Event
    private void InkyAttack()
    {
        attackChoices[0].weight = 0.3f; //Add weight to current attack to make it less likely to happen twice in a row

        Instantiate(inkyProjectile, inkyProjSpawn.position, transform.rotation);
    }

    //Animation Event
    private void ClydeAttack()
    {
        Instantiate(clydeProjectile, clydeProjSpawn.position, transform.rotation);
    }

    private void SlamInitiate()
    {
        animator.SetTrigger("Slam");
    }
    //Animation Event
    private void SlamAttack()
    {
        Vector3 vectorToPlayer = player.transform.position - transform.position;
        float distToPlayer = vectorToPlayer.magnitude;
        Vector3 dirToPlayer = vectorToPlayer.normalized;
        if (distToPlayer <= slamThreshold)
        {
            player.SlamHit(dirToPlayer * slamStrength * ((slamThreshold - distToPlayer) / 1.5f + 1), slamStunLength);
        }

        attackCooldownTimer = slamAttackCooldown;
        currentState = BossState.AttackCooldown;
    }

    public BossHitInformation GotHit(Vector3 hitPosition, int headID)
    {
        BossHitInformation hit = new BossHitInformation();

        BossHead head = inkyHead;
        switch(headID)
        {
            case 0:
                head = inkyHead;
                hit.pointWorth = inkyHead.TakeDamage(baseRailgunDamage, patienceMultiplier, inkysKilled);
                break;
            case 1:
                head = blinkyHead;
                hit.pointWorth = blinkyHead.TakeDamage(baseRailgunDamage, patienceMultiplier, blinkysKilled);
                break;
            case 2:
                head = pinkyHead;
                hit.pointWorth = pinkyHead.TakeDamage(baseRailgunDamage, patienceMultiplier, pinkysKilled);
                break;
            case 3:
                head = clydeHead;
                hit.pointWorth = clydeHead.TakeDamage(baseRailgunDamage, patienceMultiplier, clydesKilled);
                break;
        }

        if(hit.pointWorth <= 0)
        {
            GameObject obj = Instantiate(shieldPrefab, hitPosition + transform.forward * 0.2f, transform.rotation, transform);
            obj.transform.localRotation *= Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.forward);
            Destroy(obj, 2.5f);

            //hitSoundSource.PlayOneShot(shieldHitSound);
        }
        else
        {
            SpawnBlood(3, headID);
            //StartCoroutine(Knockback(transform.forward * Time.deltaTime * -200, 0.1f));
            rb.AddForce(transform.forward * Time.deltaTime * -3000);
            //hitSoundSource.PlayOneShot(hitSound);
        }

        if (hit.pointWorth > 0 && scoreIncrementPrefab != null)
        {
            Vector3 spawnPoint = scoreIncrementSpawnArea.gameObject.transform.position + new Vector3(
                Random.Range(-scoreIncrementSpawnArea.bounds.extents.x, scoreIncrementSpawnArea.bounds.extents.x),
                Random.Range(-scoreIncrementSpawnArea.bounds.extents.y, scoreIncrementSpawnArea.bounds.extents.y),
                Random.Range(-scoreIncrementSpawnArea.bounds.extents.z, scoreIncrementSpawnArea.bounds.extents.z)
                );

            GameObject go = Instantiate(scoreIncrementPrefab, spawnPoint, scoreIncrementPrefab.transform.rotation);
            ScoreIncrementDisplay display = go.GetComponent<ScoreIncrementDisplay>();
            if (display) display.SetText((hit.pointWorth).ToString());
        }

        return hit;
    }

    //Head got killed
    public void HeadKilled(int headID)
    {
        int bloodAmount = bloodAmountUponHeadDeath;
        if(headID == 3)
        {
            bloodAmount = bloodAmountUponDeath;
        }

        SpawnBlood(bloodAmount, headID);

        if (damage == 7 && headID < 3)
            StartCoroutine(EnrageSequence());
        else if (damage == 7 && headID == 3)
            StartCoroutine(DeathSequence());
        else
            animator.SetFloat("Damage", damage);

        if(headID < 3)
            ChangeAttackChoiceWeight(headID, 0);
    }

    //Animation Event for Enrage animation
    public void SetMaxDamage()
    {
        animator.SetFloat("Damage", damage);
    }

    /*IEnumerator Knockback(Vector3 force, float duration)
    {
        float timer = 0;
        while(timer < duration)
        {
            transform.Translate(force * Time.deltaTime * (Time.deltaTime / duration), Space.World);
            yield return null;
            timer += Time.deltaTime;
        }
    }*/

    void SpawnBlood(int bloodAmount, int headID)
    {
        GameObject bigBlood = bigInkyBloodPrefab;
        GameObject smallBlood = smallInkyBloodPrefab;

        Collider bloodSpawnArea = inkyBloodSpawnArea;
        switch (headID)
        {
            case 0:
                bigBlood = bigInkyBloodPrefab;
                smallBlood = smallInkyBloodPrefab;
                bloodSpawnArea = inkyBloodSpawnArea;
                break;
            case 1:
                bigBlood = bigBlinkyBloodPrefab;
                smallBlood = smallBlinkyBloodPrefab;
                bloodSpawnArea = blinkyBloodSpawnArea;
                break;
            case 2:
                bigBlood = bigPinkyBloodPrefab;
                smallBlood = smallPinkyBloodPrefab;
                bloodSpawnArea = pinkyBloodSpawnArea;
                break;
            case 3:
                bigBlood = bigClydeBloodPrefab;
                smallBlood = smallClydeBloodPrefab;
                bloodSpawnArea = clydeBloodSpawnArea;
                break;
        }

        for (int i = 0; i < bloodAmount; i++)
        {
            Vector3 spawnPoint = bloodSpawnArea.gameObject.transform.position + new Vector3(
                Random.Range(-bloodSpawnArea.bounds.extents.x, bloodSpawnArea.bounds.extents.x),
                Random.Range(-bloodSpawnArea.bounds.extents.y, bloodSpawnArea.bounds.extents.y),
                Random.Range(-bloodSpawnArea.bounds.extents.z, bloodSpawnArea.bounds.extents.z)
            );

            if (Random.Range(0, 5) < 1)
            {
                Instantiate(smallBlood, spawnPoint, smallBlood.transform.rotation);
            }
            else
            {
                Instantiate(bigBlood, spawnPoint, bigBlood.transform.rotation);
            }
        }
    }

    IEnumerator EnrageSequence()
    {
        yield return new WaitForSeconds(0.1f);

        animator.SetTrigger("Enrage");
    }

    IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(0.75f);

        animator.SetTrigger("Death");
    }
}
