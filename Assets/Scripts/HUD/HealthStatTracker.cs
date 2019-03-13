using UnityEngine;

public class HealthStatTracker : MonoBehaviour
{
    public GameObject _healthBar;

    public Color _fullHealthColor;
    public Color _lowHealthColor;

    public float _fullXBarPosition;
    public float _lowXBarPosition;

    private SpriteRenderer _barSpriteRenderer;
    private float _minMaxBarDifference;

    private void Start()
    {
        _barSpriteRenderer = _healthBar.GetComponent<SpriteRenderer>();
        _minMaxBarDifference = _fullXBarPosition - _lowXBarPosition;

        Player player = GameObject.Find("Rex").GetComponent<Player>();
        player.AddHealthChangeSubscriber(HandlePlayerHealthChange);
    }

    private void HandlePlayerHealthChange(int newHealthValue, int maxHealthValue)
    {
        float healthLeftPercent = newHealthValue / (float)maxHealthValue;
        _healthBar.transform.localPosition = new Vector3(_lowXBarPosition + healthLeftPercent * _minMaxBarDifference, _healthBar.transform.localPosition.y, _healthBar.transform.localPosition.z);
        _barSpriteRenderer.color = Color.Lerp(_lowHealthColor, _fullHealthColor, healthLeftPercent);
    }
}
