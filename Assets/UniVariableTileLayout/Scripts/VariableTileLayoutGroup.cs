using System.Linq;

namespace UnityEngine.UI.Extensions
{
    public class VariableTileLayoutGroup : GridLayoutGroup
    {
        public override void SetLayoutHorizontal()
        {
            SetCellsAlongAxis(0);
        }

        public override void SetLayoutVertical()
        {
            SetCellsAlongAxis(1);
        }

        private void SetCellsAlongAxis(int axis)
        {
            if (axis == 0)
            {
                foreach (var rect in rectChildren)
                {
                    m_Tracker.Add(this, rect,
                        DrivenTransformProperties.Anchors |
                        DrivenTransformProperties.AnchoredPosition |
                        DrivenTransformProperties.SizeDelta);

                    rect.anchorMin = Vector2.up;
                    rect.anchorMax = Vector2.up;
                    rect.sizeDelta = cellSize;
                }

                return;
            }

            var width = rectTransform.rect.size.x;
            var height = rectTransform.rect.size.y;

            int cellCountX;
            int cellCountY;

            switch (m_Constraint)
            {
                case Constraint.FixedColumnCount:
                    cellCountX = m_ConstraintCount;
                    cellCountY = Mathf.CeilToInt(rectChildren.Count / (float)cellCountX - 0.001f);
                    break;
                case Constraint.FixedRowCount:
                    cellCountY = m_ConstraintCount;
                    cellCountX = Mathf.CeilToInt(rectChildren.Count / (float)cellCountY - 0.001f);
                    break;
                default:
                    cellCountX = cellSize.x + spacing.x <= 0 ? int.MaxValue
                        : Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));

                    cellCountY = cellSize.y + spacing.y <= 0 ? int.MaxValue
                        : Mathf.Max(1, Mathf.FloorToInt((height - padding.vertical + spacing.y + 0.001f) / (cellSize.y + spacing.y)));
                    break;
            }

            var cornerX = (int)startCorner % 2;
            var cornerY = (int)startCorner / 2;

            int cellsPerMainAxis, actualCellCountX, actualCellCountY;
            float[] lastPositions;
            if (startAxis == Axis.Horizontal)
            {
                cellsPerMainAxis = cellCountX;
                actualCellCountX = Mathf.Clamp(cellCountX, 1, rectChildren.Count);
                actualCellCountY = Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(rectChildren.Count / (float)cellsPerMainAxis));
                lastPositions = new float[cellCountX];
            }
            else
            {
                cellsPerMainAxis = cellCountY;
                actualCellCountY = Mathf.Clamp(cellCountY, 1, rectChildren.Count);
                actualCellCountX = Mathf.Clamp(cellCountX, 1, Mathf.CeilToInt(rectChildren.Count / (float)cellsPerMainAxis));
                lastPositions = new float[cellCountY];
            }

            var requiredSpace = new Vector2(
                    actualCellCountX * cellSize.x + (actualCellCountX - 1) * spacing.x,
                    actualCellCountY * cellSize.y + (actualCellCountY - 1) * spacing.y
                    );
            var startOffset = new Vector2(
                    GetStartOffset(0, requiredSpace.x),
                    GetStartOffset(1, requiredSpace.y)
                    );


            foreach (var rect in rectChildren)
            {
                var cell = rect.GetComponent<VariableTileCell>();
                if (cell == null) return;
                float graphicSize;

                var minIndex = lastPositions.Select((value, index) => new {val = value, idx = index})
                    .Aggregate((min, n) => min.val <= n.val ? min : n).idx;

                if (startAxis == Axis.Horizontal)
                {
                    if (cornerX == 1)
                        minIndex = actualCellCountX - 1 - minIndex;
                    graphicSize = cell.Height * (cellSize[0] / cell.Width);
                    SetChildAlongAxis(rect, 0, startOffset.x + (cellSize[0] + spacing[0]) * minIndex, cellSize[0]);
                    SetChildAlongAxis(rect, 1, startOffset.y + lastPositions[minIndex], graphicSize);
                }
                else
                {
                    if (cornerY == 1)
                        minIndex = actualCellCountY - 1 - minIndex;
                    graphicSize = cell.Width * (cellSize[1] / cell.Height);
                    SetChildAlongAxis(rect, 0, startOffset.x + lastPositions[minIndex], graphicSize);
                    SetChildAlongAxis(rect, 1, startOffset.y + (cellSize[1] + spacing[1]) * minIndex, cellSize[1]);
                }

                if (startAxis == Axis.Horizontal)
                {
                    lastPositions[minIndex] += graphicSize + spacing.y;
                }
                else
                {
                    lastPositions[minIndex] += graphicSize + spacing.x;
                }
            }
        }
    }
}
