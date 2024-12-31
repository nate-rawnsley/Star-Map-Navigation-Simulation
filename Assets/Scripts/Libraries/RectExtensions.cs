using UnityEngine;

//Changes the size of a RectTransform in the UI.
//from https://forum.unity.com/threads/changing-recttransforms-height-from-script.451071/

public static class RectExtensions {
    public static void SetSize(this RectTransform self, Vector2 size) {
        Vector2 oldSize = self.rect.size;
        Vector2 deltaSize = size - oldSize;

        self.offsetMin = self.offsetMin - new Vector2(
            deltaSize.x * self.pivot.x,
            deltaSize.y * self.pivot.y);
        self.offsetMax = self.offsetMax + new Vector2(
            deltaSize.x * (1f - self.pivot.x),
            deltaSize.y * (1f - self.pivot.y));
    }

    public static void SetHeight(this RectTransform self, float size) {
        self.SetSize(new Vector2(self.rect.size.x, size));
    }
}
