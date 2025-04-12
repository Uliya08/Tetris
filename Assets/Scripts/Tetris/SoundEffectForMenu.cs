using UnityEngine;
public class SoundEffectForMenu : MonoBehaviour
{
   public AudioSource soundPlay; 
    public void PlayThisSoundEffect()
    {
      soundPlay.Play();
    }
}
