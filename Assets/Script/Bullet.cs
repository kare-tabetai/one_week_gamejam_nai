using DG.Tweening;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum Type
    {
        PlayerBullet,
        EnemyBullet,
    }
    public Rigidbody rb;
    public Type type = Type.PlayerBullet;
    public int damage = 1;
    public float destroy_delay = 5.0f;
    bool hit = false;

    public void Initialize(Vector3 dir)
    {
        rb.AddForce(dir, ForceMode.Impulse);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (hit) { return; }

        if (collision.gameObject.CompareTag("Stage"))
        {
            OnHit();
            return;
        }

        if (type == Type.EnemyBullet)
        {
            if (!collision.gameObject.CompareTag("Player")) { return; }
            collision.gameObject.GetComponent<VendingMachineController>().Damaged(damage);
            return;
        }

        if (type == Type.PlayerBullet)
        {
            if (!collision.gameObject.CompareTag("Enemy")) { return; }
            collision.gameObject.GetComponent<NormalEnemy>().Damaged(damage);
            return;
        }
    }

    private void OnHit()
    {
        hit = true;
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(destroy_delay);
        sequence.AppendCallback(() =>
        {
            Destroy(gameObject);
        });

    }
}
