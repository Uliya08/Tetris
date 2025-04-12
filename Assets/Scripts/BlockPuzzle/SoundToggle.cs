using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour
{
    public Image buttonImage; // Перетащите сюда slice2 (кнопку)
    public Image soundOffIcon; // Перетащите сюда slice3 (иконку "выключено")
    private bool isSoundOn = true;

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;

        // Перемещаем кнопку по вертикали (Y) в локальных координатах
        float targetY = isSoundOn ? 36f : -36f;
        buttonImage.transform.localPosition = new Vector3(
            0,
            targetY,
            0
        );

        soundOffIcon.gameObject.SetActive(!isSoundOn);

        // Для отключения звука (если нужно):
        // AudioListener.volume = isSoundOn ? 1f : 0f;
    }
}