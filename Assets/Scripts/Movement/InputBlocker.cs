using System.Collections.Generic;
using UnityEngine;

public static class InputBlocker
{
    // Считаем источники блокировки, а не просто bool
    private static readonly HashSet<string> _sources = new HashSet<string>();

    public static bool IsBlocked => _sources.Count > 0;

    // Для дебага — можно увидеть, что блокирует
    public static IReadOnlyCollection<string> ActiveSources => _sources;

    public static void Block(string source)
    {
        _sources.Add(source);
        Debug.Log($"[InputBlocker] Blocked by: {source}. Total: {_sources.Count}");
    }

    public static void Unblock(string source)
    {
        _sources.Remove(source);
        Debug.Log($"[InputBlocker] Unblocked: {source}. Remaining: {_sources.Count}");
    }

    // Принудительный сброс (например, при загрузке сцены)
    public static void Clear()
    {
        _sources.Clear();
    }
    public static bool IsBlockedBy(string source) =>
    _sources.Count == 1 && _sources.Contains(source);
}