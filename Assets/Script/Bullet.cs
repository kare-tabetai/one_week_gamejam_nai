using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody rb;


    public void Initialize(Vector3 dir)
    {
        rb.AddForce(dir, ForceMode.Impulse);
    }
}
