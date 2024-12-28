using DG.Tweening;
using UnityEngine;

public class HeartUI : MonoBehaviour
{
    public float shake_duration;
    public float destroy_delay;
    public UnityEngine.UI.Image ui_image;
    public Sprite break_heart_sprite;

    public void Clash()
    {
        ui_image.sprite = break_heart_sprite;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOShakePosition(shake_duration));
        sequence.AppendInterval(destroy_delay);
        sequence.AppendCallback(() =>
        {
            Destroy(gameObject);
        });
    }
}
