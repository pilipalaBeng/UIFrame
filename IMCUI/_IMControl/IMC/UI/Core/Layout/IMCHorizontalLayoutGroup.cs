using UnityEngine;

namespace IMCUI.UI
{
    [AddComponentMenu("Layout/Horizontal Layout Group", 150)]
    public class IMCHorizontalLayoutGroup : HorizontalOrVerticalLayoutGroup
    {
        protected IMCHorizontalLayoutGroup()
        {}

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalcAlongAxis(0, false);
        }

        public override void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1, false);
        }

        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis(0, false);
        }

        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis(1, false);
        }
    }
}
