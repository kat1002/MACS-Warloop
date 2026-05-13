using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed   = 5f;

    [Header("Banking")]
    [SerializeField] private float bankAngle   = 28f;
    [SerializeField] private float bankInTime  = 0.12f;
    [SerializeField] private float bankOutTime = 0.22f;

    private Rigidbody2D         rb;
    private InputSystem_Actions input;
    private SpriteRenderer      sr;
    private Vector2             moveInput;
    private float               halfW, halfH;
    private int                 bankState;
    private Tween               bankTween;

    private void Awake()
    {
        rb    = GetComponent<Rigidbody2D>();
        input = new InputSystem_Actions();
        sr    = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            SkinManager.Instance?.ApplySprite(sr, 0);
            halfW = sr.bounds.extents.x;
            halfH = sr.bounds.extents.y;
        }
    }

    private void OnEnable()  => input.Player.Enable();
    private void OnDisable() => input.Player.Disable();
    private void OnDestroy() => bankTween?.Kill();

    private void Update()
    {
        moveInput = input.Player.Move.ReadValue<Vector2>();
        UpdateBank();
    }

    private void UpdateBank()
    {
        int next = moveInput.x < -0.1f ? -1 : moveInput.x > 0.1f ? 1 : 0;
        if (next == bankState) return;
        bankState = next;

        bankTween?.Kill();
        bankTween = transform
            .DORotate(new Vector3(0f, bankState * -bankAngle, 0f),
                      bankState == 0 ? bankOutTime : bankInTime)
            .SetEase(bankState == 0 ? Ease.OutCubic : Ease.OutQuad);
    }

    private void FixedUpdate()
    {
        Vector2 next = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        next.x = Mathf.Clamp(next.x, ScreenBounds.MinX + halfW, ScreenBounds.MaxX - halfW);
        next.y = Mathf.Clamp(next.y, ScreenBounds.MinY + halfH, ScreenBounds.MaxY - halfH);
        rb.MovePosition(next);
    }
}
