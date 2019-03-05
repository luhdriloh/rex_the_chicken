using System.Collections;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public int _muzzleFlashFrames;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Fire()
    {
        StartCoroutine(EmitMuzzleFlash());
    }

    private IEnumerator EmitMuzzleFlash()
    {
        _spriteRenderer.enabled = true;

        for (int i = 0; i < _muzzleFlashFrames; i++)
        {
            yield return 0;
        }

        _spriteRenderer.enabled = false;
    }
}
