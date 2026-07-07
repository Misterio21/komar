using UnityEngine;

/// <summary>
/// Ovládání komára: WASD = pohyb v rovině, Mezerník = nahoru, Levý CTRL = dolů.
/// Když komár doletí k člověku, automaticky se na něj "přisaje" (Attached stav),
/// přestane se dát ovládat a postupně ubírá HP člověku. Člověk ho může sundat útokem.
/// Vyžaduje Rigidbody s Use Gravity = false (komár létá volně).
/// Umísti na kořen prefabu "Player_Mosquito". Nastav tag GameObjectu na "Mosquito"
/// a Layer také "Mosquito" (kvůli detekci útoku člověka).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Health))]
public class MosquitoController : MonoBehaviour
{
    public enum State { Flying, Attached, Dead }

    [Header("Létání")]
    [SerializeField] private float flySpeed = 8f;
    [SerializeField] private float verticalSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Přisátí / kousání")]
    [Tooltip("Kolik HP ukousne člověku za jedno 'kousnutí'.")]
    [SerializeField] private float biteDamage = 4f;
    [Tooltip("Jak často (v sekundách) kousne, dokud je přisátý.")]
    [SerializeField] private float biteInterval = 1f;
    [Tooltip("Offset od Human transformu, kam se komár 'přilepí' (např. na rameno).")]
    [SerializeField] private Vector3 attachLocalOffset = new Vector3(0.3f, 1.5f, 0f);

    public State CurrentState { get; private set; } = State.Flying;

    private Rigidbody rb;
    private Health myHealth;
    private Transform attachedHuman;
    private Health attachedHumanHealth;
    private float biteTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
        myHealth = GetComponent<Health>();
        myHealth.OnDeath.AddListener(HandleDeath);
    }

    private void Update()
    {
        if (CurrentState == State.Attached)
        {
            HandleBiting();
        }
    }

    private void FixedUpdate()
    {
        if (CurrentState == State.Flying)
        {
            FlyMovement();
        }
        else if (CurrentState == State.Attached && attachedHuman != null)
        {
            // Drž se na pozici na těle člověka
            Vector3 targetPos = attachedHuman.TransformPoint(attachLocalOffset);
            rb.MovePosition(targetPos);
        }
    }

    private void FlyMovement()
    {
        float h = Input.GetAxisRaw("Horizontal"); // A/D
        float v = Input.GetAxisRaw("Vertical");   // W/S
        float up = 0f;
        if (Input.GetKey(KeyCode.Space)) up += 1f;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) up -= 1f;

        Vector3 horizontalMove = new Vector3(h, 0f, v).normalized * flySpeed;
        Vector3 verticalMove = Vector3.up * up * verticalSpeed;

        rb.linearVelocity = new Vector3(horizontalMove.x, verticalMove.y, horizontalMove.z);
        // Pozn.: v starších verzích Unity je to rb.velocity místo rb.linearVelocity

        Vector3 flatMove = new Vector3(h, 0f, v);
        if (flatMove.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatMove, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void HandleBiting()
    {
        biteTimer += Time.deltaTime;
        if (biteTimer >= biteInterval)
        {
            biteTimer = 0f;
            if (attachedHumanHealth != null && !attachedHumanHealth.IsDead)
            {
                attachedHumanHealth.TakeDamage(biteDamage);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryAttach(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryAttach(collision.collider);
    }

    private void TryAttach(Collider other)
    {
        if (CurrentState != State.Flying) return;
        if (!other.CompareTag("Human")) return;

        HumanController human = other.GetComponentInParent<HumanController>();
        Health humanHealth = other.GetComponentInParent<Health>();
        if (human == null || humanHealth == null) return;

        Attach(other.transform, humanHealth);
    }

    private void Attach(Transform human, Health humanHealth)
    {
        CurrentState = State.Attached;
        attachedHuman = human;
        attachedHumanHealth = humanHealth;
        biteTimer = 0f;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true; // teď se pohybuje ručně přes MovePosition podle člověka
    }

    /// <summary>Zavolá HumanController při úspěšném útoku - odstraní komára z těla.</summary>
    public void ForceDetach()
    {
        if (CurrentState != State.Attached) return;

        CurrentState = State.Flying;
        rb.isKinematic = false;
        attachedHuman = null;
        attachedHumanHealth = null;

        // Malý "odskok" pryč od člověka, ať to vypadá jako odplácnutí
        rb.AddForce(transform.forward * -3f + Vector3.up * 2f, ForceMode.Impulse);
    }

    private void HandleDeath()
    {
        CurrentState = State.Dead;
        rb.isKinematic = true;
        // TODO: přehrát animaci/zvuk smrti podle potřeby
        Destroy(gameObject, 1.5f);
    }
}
