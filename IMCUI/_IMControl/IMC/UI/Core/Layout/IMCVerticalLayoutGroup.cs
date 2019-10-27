using UnityEngine;

namespace IMCUI.UI
{
    [AddComponentMenu("Layout/Vertical Layout Group", 151)]
    public class IMCVerticalLayoutGroup : HorizontalOrVerticalLayoutGroup
    {
        protected IMCVerticalLayoutGroup()
        {}

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalcAlongAxis(0, true);
        }

        public override void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1, true);
        }

        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis(0, true);
        }

        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis(1, true);
        }
    }
}
