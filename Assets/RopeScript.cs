using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RopeScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer itemSprite;
    [SerializeField] private SpriteRenderer outline;
    [SerializeField] private Rigidbody rb;
    public bool isHovered = false;
    public bool isGrabed;

    private void Start()
    {
        RopeAppear(); //Do usuniêcia
    }

    public void RopeAppear()
    {
        gameObject.transform.DOMoveY(0.5f, 1f)
            .SetEase(Ease.OutBounce)
            .OnComplete(() => 
            {
                gameObject.transform.DOMoveY(0.2f, 0.5f)
                    .SetEase(Ease.InOutSine);
            });
    }

    public void SetHover(bool hover)
    {
        if (hover == isHovered) return;
        isHovered = hover;
        outline.color = Color.white;
        outline.DOFade(isHovered ? 1f : 0f, 0.25f);
    }

    public void SetOutlineColor(Color color)
    {
        outline.color = color;
    }

    public void StartGrab()
    {
        SetHover(true);
        gameObject.layer = LayerMask.NameToLayer("ItemGrabed");
        rb.isKinematic = true;
        isGrabed = true;
    }

    public void Drop(Vector3 velocity, bool toCrafting = false)
    {
        SetHover(false);
        rb.linearVelocity = velocity;
        transform.parent = null;
        isGrabed = false;
        gameObject.layer = LayerMask.NameToLayer("Item");
    }
}
