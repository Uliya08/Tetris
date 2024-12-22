using UnityEngine;
using UnityEngine.UI;

/*! \brief Класс SliderController управляет слайдером и изменяет спрайт хендла в зависимости от его значения.
 * 
 * При изменении значения слайдера, спрайт хендла обновляется на соответствующий: 
 * если значение 0, устанавливается спрайт для значения 0, 
 * иначе устанавливается нормальный спрайт.
*/
public class SliderController : MonoBehaviour
{
    public Slider slider;                //!< Ссылка на слайдер
    public Image handleImage;            //!< Ссылка на компонент Image хендла
    public Sprite handleSpriteZero;      //!< Спрайт для значения 0
    public Sprite handleSpriteNormal;    //!< Спрайт для нормальных значений

    /*! \brief Метод инициализации */
    private void Start()
    {
        UpdateHandleSprite(slider.value);
        slider.onValueChanged.AddListener(UpdateHandleSprite);
    }

    /*! \brief Обновляет спрайт хендла в зависимости от значения слайдера
     * 
     * \param value Значение слайдера
     */
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
