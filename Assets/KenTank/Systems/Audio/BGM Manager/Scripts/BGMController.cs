using System.Collections;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace KenTank.Systems.Audio
{
    public class BGMController : MonoBehaviour
    {
        [Header("Music")]
        [SerializeField] bool enableMusic = true;
        [SerializeField] EventReference music;

        public static EventInstance musicInstance;

        void Awake()
        {
            StartCoroutine(Play());
        }

        IEnumerator Play() 
        {
            while (!RuntimeManager.HaveAllBanksLoaded)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);

            if (enableMusic)
            {
                if (music.IsNull) yield break;

                if (musicInstance.isValid())
                {
                    if (GetInstanceGUID(musicInstance) == music.Guid) yield break;
                    
                    musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }
                musicInstance = RuntimeManager.CreateInstance(music);
                musicInstance.set3DAttributes(transform.To3DAttributes());
                musicInstance.start();
            }
            else
            {
                if (musicInstance.isValid())
                {
                    musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }
            }
        }

        GUID GetInstanceGUID(EventInstance instance)
        {
            instance.getDescription(out EventDescription desc);

            desc.getID(out var path);
            return path;
        }
    }
}