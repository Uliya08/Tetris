using UnityEngine;
using UnityEngine.UI;
public class SliderController : MonoBehaviour
{
    public Slider slider;               
    public Image handleImage;            
    public Sprite handleSpriteZero;     
    public Sprite handleSpriteNormal;   
    private void Start()
    {
        UpdateHandleSprite(slider.value);
        slider.onValueChanged.AddListener(UpdateHandleSprite);
    }
    private void UpdateHandleSprite(float value)
    {
        if (value == 0)
        {
            handleImage.sprite = handleSpriteZero;
        }
        else
        {
            handleImage.sprite = handleSpriteNormal;
        }
    }
}
