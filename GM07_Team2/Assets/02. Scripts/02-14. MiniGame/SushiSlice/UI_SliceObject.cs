using UnityEngine;
using UnityEngine.UI;

public class UI_SliceObject : MonoBehaviour
{
    [Header("Size")]
    [SerializeField]
    private Vector2 _riceSize;
    [SerializeField]
    private Vector2 _wasabiSize;
    [SerializeField]
    private Vector2 _fishSize;
    [SerializeField]
    private Vector2 _junkSize;

    private Image _image;
    private UI_SushiSliceGame _game;
    private RectTransform _rectTransform;
    private Vector2 _velcity;
    private float _gravity;
    private float _despawnY;
    private bool _isResolved;
    private float _rotSpeed;
    public ESliceObjectType SliceObjectType { get; private set; }
    public RectTransform RectTransform => _rectTransform;
    public bool IsRequired => SliceObjectType == ESliceObjectType.Rice ||
                              SliceObjectType == ESliceObjectType.Wasabi ||
                              SliceObjectType == ESliceObjectType.Fish;
    public bool CanSlice => !_isResolved;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
    }
    private void Update()
    {
        if (_isResolved)
        {
            return;
        }
        _velcity.y -= _gravity * Time.unscaledDeltaTime;
        _rectTransform.anchoredPosition += _velcity * Time.unscaledDeltaTime;
        _rectTransform.Rotate(0, 0, _rotSpeed* Time.unscaledDeltaTime);
        if(_rectTransform.anchoredPosition.y < _despawnY)
        {
            Miss();
        }
    }
    public void Init(UI_SushiSliceGame game, ESliceObjectType type, Sprite image, Vector2 velocity,float gravity, float despawnY, float rotSpeed)
    {
        _game = game;
        SliceObjectType = type;
        _image.sprite = image;
        _image.preserveAspect = true;
        _image.raycastTarget = false;
        _velcity = velocity;
        _gravity = gravity;
        _despawnY = despawnY;
        _rotSpeed = rotSpeed;
        _isResolved = false;
        SetSize();
    }
    public void Slice()
    {
        if (_isResolved)
        {
            return;
        }
        _isResolved = true;
        _game.OnObjectSliced(this);
        AudioManager.Instance?.PlaySFX(EAudioType.SliceGmae_Slice);
        Destroy(gameObject);
    }
    private void Miss()
    {
        if (_isResolved)
        {
            return;
        }
        _isResolved = true;
        _game.OnobjectMissed(this);
        Destroy(gameObject);
    }
    private void SetSize()
    {
        Vector2 targetSize = Vector2.zero;
        switch(SliceObjectType)
        {
            case ESliceObjectType.Rice:
                targetSize = _riceSize;
                break;
            case ESliceObjectType.Wasabi:
                targetSize = _wasabiSize;
                break;
            case ESliceObjectType.Fish:
            case ESliceObjectType.WrongFish:
                targetSize = _fishSize;
                break;
            case ESliceObjectType.Junk:
                targetSize = _junkSize;
                break;
        }
        _rectTransform.sizeDelta = targetSize;
    }
}
