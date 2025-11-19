using DG.Tweening;
using UnityEngine;

namespace CocoDoogy.Tile.Piece
{
    /// <summary>
    /// 물 위에 뜨는 상자용
    /// </summary>
    [RequireComponent(typeof(Piece))]
    public class FloatedCaskPiece: MonoBehaviour
    {
        [Tooltip("움직일 상자의 Pivot")] [SerializeField] private Transform cask;


        void OnEnable()
        {
            cask.DOMoveY(-0.1f, 5f).SetLoops(-1, LoopType.Yoyo).SetId(this);
        }
        void OnDisable()
        {
            DOTween.Kill(this, true);
        }
    }
}