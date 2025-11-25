using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI
{
    public class Star : MonoBehaviour
    {
        [SerializeField] private Image image;
        
        [SerializeField] private Sprite brightStarSprite;
        [SerializeField] private Sprite darkStarSprite;

        [SerializeField] private float duration;
    }
}