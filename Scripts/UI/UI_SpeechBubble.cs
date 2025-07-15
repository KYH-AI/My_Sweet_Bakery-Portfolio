using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SpeechBubble : UIBase
{
    [SerializeField] private Image _requestImage;
    [SerializeField] private TextMeshProUGUI _requestCountText;

    private int _count = 0;
    public EmojiType CurrentEmoji { get; private set; } = EmojiType.None;

    public int Count
    {
        get => _count;
        set
        {
            _count = value;
            SetRequestText(_count.ToString());
        }
    }
    
    public void SetRequestImage(Sprite sprite, EmojiType emojiType)
    {
        CurrentEmoji = emojiType;
        _requestImage.sprite = sprite;
        ShowRequestImage();
    }

    public void SetRequestText(string str)
    {
        _requestCountText.text = str;
        ShowRequestText();
    }

    public void ShowRequestText()
    {
        _requestCountText.gameObject.SetActive(true);
    }

    public void ShowRequestImage()
    {
        _requestImage.gameObject.SetActive(true);
    }
    
    public void HideRequestText()
    {
        _requestCountText.gameObject.SetActive(false);
    }

    public void HideRequestImage()
    {
        _requestImage.gameObject.SetActive(false);
    }
}
