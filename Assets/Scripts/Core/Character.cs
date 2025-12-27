using Sirenix.OdinInspector;
using UnityEngine;

namespace ZombieHouseDefense.Core
{
    [RequireComponent(typeof(AudioSource))]
    public abstract class Character : MonoBehaviour
    {
        [BoxGroup("Character"),InlineProperty]
        public CharacterResource Hp;
        [SerializeField, BoxGroup("Character")]
        private float baseSpeed = 5f;
        public float Speed
        {
            get { return baseSpeed + SpeedMod; }
            set { baseSpeed = value; }
        }
        [ShowInInspector, ReadOnly, BoxGroup("Character")]
        private float SpeedMod = 0;
        [BoxGroup("Character")]
        public int level = 1;
        
        [BoxGroup("components")]
        protected AudioSource AudioSource;

        protected virtual void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
            if (AudioSource == null)
            {
                Debug.LogError("Character: AudioSource component is missing.");
            }
            Hp = new CharacterResource();
        }


        public virtual void AddSpeedMod(float mod)
        {
            SpeedMod += mod;
        }
        

        public virtual void Damage(int amt)
        {
            Hp.Current -= amt;
            
            if (Hp.IsEmpty)
            {
                Die();
            }
        }


        protected abstract void Die();


        protected virtual void Heal(int amt)
        {
            Hp.Current += amt;
        }


        protected virtual void LevelUp()
        {
            level++;
        }

        protected System.Collections.IEnumerator PlaySound(AudioClip clip)
        {
            if (AudioSource == null)
            {
                Debug.LogError("Character.PlaySound: AudioSource is not initialized.");
                yield break;
            }

            bool played = AudioSource.PlaySound(clip);
            if (!played)
                yield break;
                
            yield return new WaitWhile(() => AudioSource.isPlaying);
        }
        
    }
}