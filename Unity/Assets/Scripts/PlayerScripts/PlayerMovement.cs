using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference jump;
    [SerializeField] private InputActionReference dash;
    [SerializeField] private InputActionReference attack;
    [SerializeField] private LayerMask rayCastLayer;
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravity;
    [SerializeField] private float maxDashTime;
    [SerializeField] private float dashSpeed;
    private Animator _animator;
    private Vector2 _movement;
    private SpriteRenderer _spriteRenderer;
    private Action _detachDiedListener;
    private float _currentDashTime;
    private float _offset;
    private float _width;
    private bool _isGrounded;

    private void Start()
    {
        _animator = gameObject.GetComponentInChildren<Animator>();
        _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();

        _width = GetComponentInChildren<BoxCollider2D>().bounds.size.x;
        _offset = _width / 2 * 1.1f;

        jump.action.performed += OnJump;
        dash.action.performed += OnDash;

        if (TryGetComponent(out Health health))
        {
            Action onDeath = () =>
            {
                _animator.SetTrigger("died");
                enabled = false;
            };
            _detachDiedListener = health.Died -= onDeath;

            health.Died += onDeath;
        }
    }


    private void Update()
    {
        _movement.y -= gravity * Time.deltaTime;

        Vector2 position = transform.position;
        var xMovement = _movement.x * Time.deltaTime * speed;
        if (_currentDashTime < maxDashTime)
        {
            var dashDistance = dashSpeed * Time.deltaTime;
            if (xMovement > 0) xMovement += dashDistance;
            else if (xMovement < 0) xMovement -= dashDistance;
            _currentDashTime += dashDistance;
        }

        xMovement = RayCastX(position, xMovement);
        position.x += xMovement;

        var yMovement = _movement.y * Time.deltaTime * jumpHeight;
        yMovement = RayCastY(position, yMovement);
        position.y += yMovement;

        transform.position = position;
    }

    private void OnDestroy()
    {
        jump.action.performed -= OnJump;
        dash.action.performed -= OnDash;
        _detachDiedListener?.Invoke();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!_isGrounded) return;
        _isGrounded = false;
        _movement.y = context.ReadValue<float>();
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        _currentDashTime = 0.0f;
        _animator.SetTrigger("dashStart");
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        gameObject.TryGetComponent(out UniversalAttack uniAttack);
        uniAttack.DoAttack();
    }

    public void GetPlayerMovement(InputAction.CallbackContext context)
    {
        if (!enabled) return;
        _movement.x = context.ReadValue<float>();
        _animator.SetBool("walking", _movement.x != 0);
        _spriteRenderer.flipX = _movement.x < 0;
    }

    private float RayCastX(Vector2 position, float xMovement)
    {
        switch (xMovement)
        {
            case > 0.0f:
                var pos = new Vector3(position.x + _offset, position.y, 0);
                Debug.DrawRay(pos, Vector2.right * xMovement, Color.yellow);
                var hit = Physics2D.Raycast(pos, Vector2.right, xMovement);

                if (hit)
                {
                    var distance = hit.point.x - position.x - _offset;
                    if (distance < xMovement) return distance;
                }

                break;

            case < 0.0f:
                var pos1 = new Vector3(position.x - _offset, position.y, 0);
                var hit1 = Physics2D.Raycast(pos1, Vector2.left, -xMovement);

                if (hit1)
                {
                    var distance = position.x - hit1.point.x - _offset;
                    if (distance < -xMovement) return -distance;
                }

                break;
        }

        return xMovement;
    }

    private float RayCastY(Vector2 position, float yMovement)
    {
        if (!(yMovement < 0)) return yMovement;
        var pos = new Vector3(position.x, position.y - _offset, 0);
        var hit = Physics2D.Raycast(pos, Vector2.down, -yMovement, rayCastLayer);

        if (!hit) return yMovement;
        var distance = -hit.point.y - -position.y - _offset;
        if (distance < -yMovement)
        {
            _isGrounded = true;
            _movement.y = 0;
            return -distance;
        }

        return yMovement;
    }
}
