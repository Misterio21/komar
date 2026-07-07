using UnityEngine;

/// <summary>
/// Ovládání člověka: WASD pohyb, Mezerník = skok, levé tlačítko myši (nebo klávesa F) = útok.
/// Vyžaduje Rigidbody (Freeze Rotation X/Z doporučeno) a Collider na objektu.
/// Umísti na kořen prefabu "Player_Human". Nastav tag GameObjectu na "Human".
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Health))]
public class HumanController : MonoBehaviour
{
    [Header("Pohyb")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Kontrola země")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Útok")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private float attackDamage = 50f; // dost na zabití komára na 1-2 rány
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask mosquitoLayer;
    [SerializeField] private KeyCode attackKey = KeyCode.F;
    [SerializeField] private bool attackWithMouseButton = true;

    private Rigidbody rb;
    private Health health;
    private Vector3 moveInput;
    private bool isGrounded;
    private float lastAttackTime = -999f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        ReadInput();
        HandleAttackInput();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        Move();
    }

    private void ReadInput()
    {
        float h = Input.GetAxisRaw("Horizontal"); // A/D
        float v = Input.GetAxisRaw("Vertical");   // W/S
        moveInput = new Vector3(h, 0f, v).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void CheckGrounded()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            // Náhradní kontrola, pokud nechceš vytvářet groundCheck Transform
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        }
    }

    private void Move()
    {
        Vector3 velocity = moveInput * moveSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        // Pozn.: v starších verzích Unity je to rb.velocity místo rb.linearVelocity

        if (moveInput.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveInput, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void HandleAttackInput()
    {
        bool attackPressed = Input.GetKeyDown(attackKey) || (attackWithMouseButton && Input.GetMouseButtonDown(0));
        if (attackPressed && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    private void Attack()
    {
        Vector3 center = transform.position + transform.forward * attackRange;
        Collider[] hits = Physics.OverlapSphere(center, attackRadius, mosquitoLayer);

        foreach (var hit in hits)
        {
            // Zkusí zasáhnout přímo Health komponentu komára
            Health mosquitoHealth = hit.GetComponentInParent<Health>();
            if (mosquitoHealth != null)
            {
                mosquitoHealth.TakeDamage(attackDamage);
            }

            // Pokud je komár aktuálně přisátý, útok ho vždy odstraní z těla
            MosquitoController mosquito = hit.GetComponentInParent<MosquitoController>();
            if (mosquito != null)
            {
                mosquito.ForceDetach();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position + transform.forward * attackRange;
        Gizmos.DrawWireSphere(center, attackRadius);
    }
}
