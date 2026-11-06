public static class SaveSession
{
    public static int SelectedSlot { get; private set; } = -1;
    public static SaveGameData PendingLoadData { get; private set; }

    public static bool HasPendingLoad => PendingLoadData != null;

    public static void SelectSlot(int slotId)
    {
        SelectedSlot = slotId;
    }

    public static void PrepareLoad(SaveGameData data)
    {
        PendingLoadData = data;
        SelectedSlot = data == null ? -1 : data.slotId;
    }

    public static void BeginNewGame()
    {
        SelectedSlot = -1;
        PendingLoadData = null;
    }

    public static void ClearSelection()
    {
        SelectedSlot = -1;
        PendingLoadData = null;
    }
}
