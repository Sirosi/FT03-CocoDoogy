using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Collections;

namespace CocoDoogy.UI
{
    public class InvertMask : Image
    {
        private Material invertMat = null;


        protected override void Start()
        {
            base.Start();
            StartCoroutine(Fix());
        }

        private IEnumerator Fix()
        {
            yield return null;
            maskable = false;
            maskable = true;
        }


        public override Material materialForRendering
        {
            get
            {
                Material baseMat = base.materialForRendering;
                if (!invertMat || invertMat.shader != baseMat)
                {
                    (invertMat = baseMat).SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                }
                return invertMat;
            }
        }
    }
}