using UnityEngine;

/// <summary>
/// Handles it all for a single bird.
/// Movement, rotation, destruction.
/// </summary>
public class Bird : MonoBehaviour
{
    public Transform player;
    private Vector3 target;

    public float speed, rotSpeed = 2.0f;

    private new Rigidbody2D rigidbody;



    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }


    private void Update()
    {
        Move();
    }


    private void Move ()
    {
        target = player.position;
        var dir = target - transform.position;

        if (dir != Vector3.zero)
        {
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            var targetRot = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotSpeed);
        }

        rigidbody.velocity = dir.normalized * speed * Time.deltaTime;
    }
}
