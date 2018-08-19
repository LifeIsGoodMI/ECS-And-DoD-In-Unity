using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;

    private new Rigidbody2D rigidbody;
    private float x, input;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        input = Input.GetAxis("Horizontal");
        x = input * speed * Time.deltaTime;

        rigidbody.velocity = new Vector2(x, rigidbody.velocity.y);
    }
}
