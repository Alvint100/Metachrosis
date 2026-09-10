using System.Collections;
using UnityEngine;

// TO USE:
// - Attach this script to the player object (with CharacterController).
// - Assign the lightHitboxPrefab and heavyHitboxPrefab with prefabs that have
//   a capsule collider (set as trigger) and an AttackHitbox component, oriented
//   along the local Z axis.
public class AttackBehaviour : MonoBehaviour
{
    [Header("Hitbox Prefabs + player")]
    public GameObject lightHitboxPrefab;
    public GameObject heavyHitboxPrefab;
    public PlayerMovement movement;


    [Header("Light Attack")]
    public float lightDamage = 10f;
    public float lightDuration = 0.3f;
    public float lightArcDegrees = 100f;
    public float lightHitboxDistanceFromPlayer = 1.5f;
    public float lightComboWindow  = 0.4f;


    [Header("Heavy Attack")]
    public float heavyDamage = 30f;
    public float heavyDuration = 0.6f;
    public float heavyArcDegrees = 90f;
    public float heavyHitboxDistanceFromPlayer = 1.5f;


    [Header("Movement Slow on Attack, between 0 (full stop on attack) and 1 (No change)")]
    public float lightSwingSlowFactor = 0.6f;
    public float heavySwingSlowFactor = 0.3f;


    private int leftClick = 0;
    private int rightClick = 1;

    private bool _isAttacking = false;
    private int  _comboStep = 0;
    private bool _inputBuffered = false;
    private bool _heavyBuffered = false;
    private Coroutine _attackCoroutine;

    void Update()
    {
        if (_isAttacking && movement.IsDashing)
            CancelAttack();

        if (Input.GetMouseButtonDown(leftClick))
            TryAttack(false);

        if (Input.GetMouseButtonDown(rightClick))
            TryAttack(true);
    }

    
    void TryAttack(bool isHeavy)
    {
        if (!_isAttacking)
        {
            if (isHeavy)
                _attackCoroutine = StartCoroutine(HeavyAttackRoutine());

            else
                _attackCoroutine = StartCoroutine(LightAttackRoutine(_comboStep));
        }
        else
        {
            BufferInput(isHeavy);
        }
    }

    void BufferInput(bool isHeavy)
    {
        _inputBuffered = !isHeavy;
        _heavyBuffered = isHeavy;
    }


    // LIGHT ATTACK(3 hit combo) !!! // LIGHT ATTACK(3 hit combo) !!! // LIGHT ATTACK(3 hit combo) !!! // LIGHT ATTACK(3 hit combo) !!!
    IEnumerator LightAttackRoutine(int comboStep)
    {
        _isAttacking = true;
        _inputBuffered = false;

        float startAngle;
        float endAngle;
        float halfArc = lightArcDegrees / 2f;
        float sign = -1f;

        // basically alternates between right and left
        //eg. Step 1 swings - to +, Step 2 swings + to -
        if (comboStep % 2 != 0)
            sign = 1f;

        startAngle = halfArc * sign;
        endAngle = -startAngle;
        //end of alternation stuff


        movement.SetAttackSpeedMultiplier(lightSwingSlowFactor);

        GameObject hitboxObj = SpawnHitbox(lightHitboxPrefab, lightDamage);
        yield return SweepHitboxAcrossArc(hitboxObj, lightDuration, startAngle, endAngle, lightHitboxDistanceFromPlayer);

        Destroy(hitboxObj);
        movement.SetAttackSpeedMultiplier(1f);

        _comboStep = (comboStep + 1) % 3;

        // combo window stuff
        float comboElapsed = 0f;
        _isAttacking = false;

        while (comboElapsed < lightComboWindow)
        {
            if (movement.IsDashing)
            {
                _comboStep = 0;
                _inputBuffered = false;
                yield break;
            }

            if (_inputBuffered && !_heavyBuffered)
            {
                _attackCoroutine = StartCoroutine(LightAttackRoutine(_comboStep));
                yield break;
            }

            if (_heavyBuffered)
            {
                _attackCoroutine = StartCoroutine(HeavyAttackRoutine());
                yield break;
            }

            comboElapsed += Time.deltaTime;
            yield return null;
        }

        // this is for if players didn't combo, its just some cleanup to reset states and the like
        _comboStep = 0;
        _inputBuffered = false;
        _heavyBuffered = false;
    }

    GameObject SpawnHitbox(GameObject hitboxPrefab, float damage)
    {
        GameObject hitboxObj = Instantiate(hitboxPrefab, transform.position, transform.rotation, transform);
        AttackHitbox hitbox = hitboxObj.GetComponent<AttackHitbox>();
        hitbox.Init(damage);
        return hitboxObj;
    }

    IEnumerator SweepHitboxAcrossArc(GameObject hitboxObj, float duration, float startAngle, float endAngle, float distanceFromPlayer)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (movement.IsDashing)
                yield break;

            float t = elapsed / duration;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, t);

            Quaternion rot = Quaternion.Euler(0f, currentAngle, 0f);
            Vector3 offset = rot * Vector3.forward * distanceFromPlayer;
            hitboxObj.transform.localPosition = offset;
            hitboxObj.transform.localRotation = Quaternion.LookRotation(offset.normalized, Vector3.up);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

// HEAVY ATTACK // HEAVY ATTACK // HEAVY ATTACK // HEAVY ATTACK // HEAVY ATTACK // HEAVY ATTACK // HEAVY ATTACK // HEAVY ATTACK
//heavy attack is top to bottom with no combo behaviours (for now)!!!
//TODO:
// - add effects on hit
// - add hitstun for heavy attack to differentiate
// - add combo behaviour in beta release
    IEnumerator HeavyAttackRoutine()
    {
        _isAttacking = true;
        _inputBuffered = false;
        _heavyBuffered = false;
        _comboStep = 0;

        movement.SetAttackSpeedMultiplier(heavySwingSlowFactor);

        GameObject hitboxObj = SpawnHitbox(heavyHitboxPrefab, heavyDamage);

        float startTilt = -60f;
        float endTilt = 60f;
        float swingTimeElapsed = 0f;

        while (swingTimeElapsed < heavyDuration)
        {
            if (movement.IsDashing)
                break;

            float t = swingTimeElapsed / heavyDuration;
            float tilt = Mathf.Lerp(startTilt, endTilt, t);
            Vector3 offset = Quaternion.Euler(tilt, 0f, 0f) * Vector3.forward * heavyHitboxDistanceFromPlayer;

            hitboxObj.transform.localPosition = offset;
            hitboxObj.transform.localRotation = Quaternion.LookRotation(offset.normalized, Vector3.up);

            swingTimeElapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(hitboxObj);
        movement.SetAttackSpeedMultiplier(1f);

        _isAttacking   = false;
        _inputBuffered = false;
        _heavyBuffered = false;
    }

    void CancelAttack()
    {
        if (_attackCoroutine != null)
            StopCoroutine(_attackCoroutine);

        foreach (Transform child in transform)
        {
            if (child.GetComponent<AttackHitbox>() != null)
                Destroy(child.gameObject);
        }

        movement.SetAttackSpeedMultiplier(1f);
        _isAttacking = false;
        _inputBuffered = false;
        _heavyBuffered = false;
        _comboStep = 0;
    }

    public bool IsAttacking
    {
        get { return _isAttacking; }
    }
}