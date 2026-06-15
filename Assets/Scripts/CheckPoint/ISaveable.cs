public interface ISaveable
{
    string GetUniqueID();      // уникальный ID объекта
    object CaptureState();     // снять состояние
    void RestoreState(object state); // восстановить состояние
}