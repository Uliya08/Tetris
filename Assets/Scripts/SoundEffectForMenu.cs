using UnityEngine;

/*! \brief Класс SoundEffectForMenu отвечает за воспроизведение звуковых эффектов в меню.
 * 
 * Позволяет воспроизводить звуковые эффекты с использованием компонента AudioSource.
 */
public class SoundEffectForMenu : MonoBehaviour
{
   public AudioSource soundPlay; //!< Компонент AudioSource для воспроизведения звука.

    /*! \brief Метод воспроизводит заданный звуковой эффект. 
     */
    public void PlayThisSoundEffect()
    {
      soundPlay.Play();
    }
}
