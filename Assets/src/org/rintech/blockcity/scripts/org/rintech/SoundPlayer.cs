using UnityEngine;
namespace org.rintech.blockcity
{
    public class SoundPlayer : MonoBehaviour
    {
        void Start()
        {
            AudioSource source = GetComponent<AudioSource>();
            source.Play();
            Destroy(gameObject, source.clip.length);
        }
        public static void play (Vector3 position, float volume, AudioClip clip)
        {
            GameObject obj = new GameObject("Sound");
            obj.transform.position = position;
            AudioSource audiosource = obj.AddComponent<AudioSource>();
            audiosource.volume = volume;
            audiosource.clip = clip;
            obj.AddComponent<SoundPlayer>();
        }
    }
}
