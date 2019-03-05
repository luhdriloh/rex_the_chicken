using UnityEngine;

public class Pickup : MonoBehaviour
{
    public GameObject _itemGameobject;

    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _collider;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();

        if (_itemGameobject != null)
        {
            SetPickup(_itemGameobject);
        }
    }

    public void SetPickup(GameObject drop)
    {
        _itemGameobject = drop;
        drop.transform.parent = transform;

        IItem item = _itemGameobject.GetComponent<IItem>();
        item.SetAsInactiveItem();

        Vector2 spriteSize = _spriteRenderer.sprite.bounds.size;
        _collider.size = spriteSize;
        _collider.offset = new Vector2((spriteSize.x / 2), 0);

        // set scale and random rotation
        transform.localScale = _itemGameobject.transform.localScale;
        transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 359));
    }
}
