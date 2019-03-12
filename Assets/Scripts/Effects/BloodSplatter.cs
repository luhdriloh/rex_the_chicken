using UnityEngine;

public class BloodSplatter : MonoBehaviour
{
    private SpriteRenderer _spriterenderer;

    private void Awake()
    {
        _spriterenderer = GetComponent<SpriteRenderer>();
    }

    public void SetBloodSplatter(Vector3 position, float zRotation, Sprite sprite)
    {
        transform.position = position;
        transform.eulerAngles = new Vector3(0f, 0f, zRotation);
        _spriterenderer.sprite = sprite;
    }
}
