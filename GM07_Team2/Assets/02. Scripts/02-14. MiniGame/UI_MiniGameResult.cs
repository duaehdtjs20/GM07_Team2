using GM07.Order;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MiniGameResult : MonoBehaviour
{
    [SerializeField]
    private Image _resultTextImage;
    [SerializeField]
    private Sprite _failSprite;
    [SerializeField]
    private Sprite _normalSprite;
    [SerializeField]
    private Sprite _goodSprite;
    [SerializeField]
    private Sprite _greatSprite;
    [SerializeField]
    private List<GameObject> _starImages;

    public void ApplyResult(EQuality quality)
    {
        if(_resultTextImage == null ||
            _failSprite == null ||
            _normalSprite == null ||
            _goodSprite == null ||
            _greatSprite == null ||
            _starImages.Count != 3)
        {
            return;
        }

        switch(quality)
        {
            case EQuality.Fail:
                _resultTextImage.sprite = _failSprite;
                _starImages[0].SetActive(false);
                _starImages[1].SetActive(false);
                _starImages[2].SetActive(false);
                break;
            case EQuality.Normal:
                _resultTextImage.sprite = _normalSprite;
                _starImages[0].SetActive(true);
                _starImages[1].SetActive(false);
                _starImages[2].SetActive(false);
                break;
            case EQuality.Good:
                _resultTextImage.sprite = _goodSprite;
                _starImages[0].SetActive(true);
                _starImages[1].SetActive(true);
                _starImages[2].SetActive(false);
                break;
            case EQuality.Great:
                _resultTextImage.sprite = _greatSprite;
                _starImages[0].SetActive(true);
                _starImages[1].SetActive(true);
                _starImages[2].SetActive(true);
                break;
        }
    }
}
