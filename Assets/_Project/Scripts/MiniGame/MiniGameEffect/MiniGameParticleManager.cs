using CocoDoogy.Audio;
using CocoDoogy.Core;
using Coffee.UIExtensions;
using Lean.Pool;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static CocoDoogy.UI.StaminaUI.StaminaUI;

namespace CocoDoogy
{
    public class MiniGameParticleManager : Singleton<MiniGameParticleManager>
    {
        [SerializeField] RectTransform canvas;
        [SerializeField] UIParticle coatParticlePrefab;
        [SerializeField] UIParticle trashParticlePrefab;
        [SerializeField] UIParticle wateringParticlePrefab;
        [SerializeField] UIParticle diggingParticlePrefab;
        public void PlayParticle(UIParticle particlePrefab, Vector3 position, Transform parent)
        {
            UIParticle p = LeanPool.Spawn(particlePrefab, position, Quaternion.identity, parent);
            p.Play();
            StartCoroutine(RetrunAfter(p, p.timeScaleMultiplier));
            print("파티클 실행");
        }

        public void ParticleWatering(Transform parent) 
        {
                Instantiate(wateringParticlePrefab, parent);
        }

        public void ParticleDigging(Vector3 position, Transform parent) => PlayParticle(diggingParticlePrefab, position, parent);
        public void ParticleTrash(Vector3 position, Transform parent)=> PlayParticle(trashParticlePrefab, position, parent);

        public void ParticleCoat(Vector3 position, Transform parent)=> PlayParticle(coatParticlePrefab, position, parent);
        

        IEnumerator RetrunAfter(UIParticle p, float time)
        {
            yield return new WaitForSeconds(time);
            LeanPool.Despawn(p);
        }
    }
}
