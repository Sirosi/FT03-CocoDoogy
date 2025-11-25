using UnityEngine;

namespace CocoDoogy.EmoteBillboard
{
    // 단순 테스트용

    public class EmoteManager : MonoBehaviour
    {
        [SerializeField] private EmoteBillboard emote;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                emote.ShowEmote();
            }
        }
    }
}
