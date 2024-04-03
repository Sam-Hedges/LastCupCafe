using UnityEngine;
using UnityEngine.UI;

public class RadialLayoutGroup : LayoutGroup
{
    [SerializeField] private float radius = 100f;
    [SerializeField] private bool reverseOrder = false;
    [SerializeField] private float minAngle = 0f;
    [SerializeField] private float maxAngle = 360f;
    public enum ElementRotation { None, TowardCenter, AwayFromCenter, ScreenAligned }
    [SerializeField] private ElementRotation elementRotation = ElementRotation.None;

    protected override void OnEnable()
    {
        base.OnEnable();
        CalculateLayoutInputVertical();
    }

    public override void CalculateLayoutInputVertical()
    {
        UpdateLayout();
    }

    public override void SetLayoutHorizontal() { }
    public override void SetLayoutVertical() { }

    private void UpdateLayout()
    {
        float angleSpan = CalculateAngleSpan(minAngle, maxAngle);
        float angleStep = CalculateAngleStep(angleSpan, rectChildren.Count, maxAngle > minAngle || (minAngle > maxAngle && Mathf.Abs(angleSpan) < 360));

        for (int i = 0; i < rectChildren.Count; i++)
        {
            int index = reverseOrder ? rectChildren.Count - 1 - i : i;
            float angle = (minAngle + angleStep * index) % 360;
            if (angle < 0) angle += 360; // Normalize angle to be within 0 to 360

            SetChildPosition(rectChildren[i], angle);
        }
    }

    private float CalculateAngleSpan(float minAngle, float maxAngle)
    {
        float span = maxAngle - minAngle;
        span = (span % 360 + 360) % 360; // Normalize to 0 - 360
        if (span == 0 && maxAngle != minAngle)
        {
            return 360; // Full circle
        }
        return span;
    }

    private float CalculateAngleStep(float span, int count, bool clockwise)
    {
        if (count <= 1) return 0;
        if (clockwise) return span / (count - 1);
        return span / (count - 1) * -1; // Reverse direction if counterclockwise
    }

    private void SetChildPosition(RectTransform child, float angle)
    {
        Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
        child.localPosition = direction * radius;

        switch (elementRotation)
        {
            case ElementRotation.TowardCenter:
                child.localRotation = Quaternion.Euler(0, 0, angle + 180);
                break;
            case ElementRotation.AwayFromCenter:
                child.localRotation = Quaternion.Euler(0, 0, angle);
                break;
            case ElementRotation.ScreenAligned:
                child.localRotation = Quaternion.identity;
                break;
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        CalculateLayoutInputVertical();
    }
#endif
}
