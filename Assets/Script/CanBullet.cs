using UnityEngine;

public class CanBullet : MonoBehaviour
{
    public Rigidbody rb;

    void Start()
    {

    }

    public void initialize(Vector3 dir)
    {
        rb.AddForce(dir, ForceMode.Impulse);
    }

    void Update()
    {

    }
}
