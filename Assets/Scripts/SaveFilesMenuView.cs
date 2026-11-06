using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class SaveFilesMenuView
{
    public static void Show(Canvas canvas)
    {
        canvas.gameObject.SetActive(true);
        canvas.enabled = true;
        RefreshSlots(canvas);
    }

    public static void Hide(Canvas canvas)
    {
        canvas.enabled = false;
    }

    public static void RefreshSlots(Canvas canvas)
    {
        foreach (Button slotButton in GetSlotButtons(canvas))
        {
            int slotId = GetSlotId(slotButton);
            SaveGameData data = SaveSystem.Load(slotId);
            TMP_Text text = slotButton.GetComponentInChildren<TMP_Text>(true);

            if (text != null)
            {
                text.text = data == null
                    ? $"Slot {slotId}\nEmpty Slot"
                    : $"Slot {slotId}\n{data.savedAtTime}";

                RectTransform textTransform = text.rectTransform;
                textTransform.anchorMin = new Vector2(0.30f, 0f);
                textTransform.anchorMax = new Vector2(1f, 1f);
                textTransform.offsetMin = Vector2.zero;
                textTransform.offsetMax = Vector2.zero;
            }

            UpdateThumbnail(slotButton, slotId, data != null);
        }
    }

    public static void Configure(
        Canvas canvas,
        Action<int> selectSlot,
        Action load,
        Action save,
        Action delete,
        Action back,
        bool showSaveButton)
    {
        foreach (Button slotButton in GetSlotButtons(canvas))
        {
            int slotId = GetSlotId(slotButton);
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(() => selectSlot(slotId));
        }

        ConfigureButton(canvas, "Load_Save", load, true);
        ConfigureButton(canvas, "Delete_Save", delete, true);
        ConfigureButton(canvas, "Back_Button", back, true);
        ConfigureButton(canvas, "Save_Game", save, showSaveButton);
        RefreshSlots(canvas);
    }

    private static void ConfigureButton(Canvas canvas, string buttonName, Action action, bool visible)
    {
        Transform buttonTransform = FindChild(canvas.transform, buttonName);
        if (buttonTransform == null)
        {
            return;
        }

        buttonTransform.gameObject.SetActive(visible);
        Button button = buttonTransform.GetComponent<Button>();
        if (button == null)
        {
            return;
        }

        button.onClick.RemoveAllListeners();
        if (action != null)
        {
            button.onClick.AddListener(() => action());
        }
    }

    private static Button[] GetSlotButtons(Canvas canvas)
    {
        return canvas.GetComponentsInChildren<Button>(true)
            .Where(button => button.name.StartsWith("Save_File"))
            .OrderBy(button => button.transform.GetSiblingIndex())
            .ToArray();
    }

    private static int GetSlotId(Button button)
    {
        return button.transform.GetSiblingIndex() + 1;
    }

    private static void UpdateThumbnail(Button slotButton, int slotId, bool hasSave)
    {
        RawImage thumbnail = GetOrCreateThumbnail(slotButton);
        thumbnail.gameObject.SetActive(hasSave && File.Exists(SaveSystem.GetThumbnailPath(slotId)));

        if (!thumbnail.gameObject.activeSelf)
        {
            if (thumbnail.texture is Texture2D oldTexture)
            {
                UnityEngine.Object.Destroy(oldTexture);
                thumbnail.texture = null;
            }

            return;
        }

        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        texture.LoadImage(File.ReadAllBytes(SaveSystem.GetThumbnailPath(slotId)));

        if (thumbnail.texture is Texture2D existingTexture)
        {
            UnityEngine.Object.Destroy(existingTexture);
        }

        thumbnail.texture = texture;
    }

    private static RawImage GetOrCreateThumbnail(Button slotButton)
    {
        Transform existing = FindChild(slotButton.transform, "Save_Thumbnail");
        if (existing != null)
        {
            return existing.GetComponent<RawImage>();
        }

        GameObject thumbnailObject = new GameObject(
            "Save_Thumbnail",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(RawImage));
        thumbnailObject.transform.SetParent(slotButton.transform, false);

        RectTransform thumbnailTransform = thumbnailObject.GetComponent<RectTransform>();
        thumbnailTransform.anchorMin = new Vector2(0.03f, 0.12f);
        thumbnailTransform.anchorMax = new Vector2(0.27f, 0.88f);
        thumbnailTransform.offsetMin = Vector2.zero;
        thumbnailTransform.offsetMax = Vector2.zero;

        RawImage thumbnail = thumbnailObject.GetComponent<RawImage>();
        thumbnail.raycastTarget = false;
        return thumbnail;
    }

    private static Transform FindChild(Transform parent, string childName)
    {
        if (parent.name == childName)
        {
            return parent;
        }

        for (int childIndex = 0; childIndex < parent.childCount; childIndex++)
        {
            Transform result = FindChild(parent.GetChild(childIndex), childName);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}
