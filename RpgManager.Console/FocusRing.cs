using Terminal.Gui.ViewBase;

namespace RpgManager.Console;

/// <summary>
/// Keeps keyboard navigation deterministic across screens whose visual view trees
/// contain nested tab groups. Only the active screen's interactive targets are in
/// this ring, in the order chosen by its screen builder.
/// </summary>
internal sealed class FocusRing
{
    private View[] _targets = [];
    private int _currentIndex = -1;

    public void SetTargets(params View[] targets)
    {
        var focusedTarget = FindFocusedTarget();
        var oldIndex = _currentIndex;
        _targets = targets
            .Where(target => target is { CanFocus: true, Visible: true, Enabled: true })
            .Distinct()
            .ToArray();

        var preservedIndex = focusedTarget is null ? -1 : Array.IndexOf(_targets, focusedTarget);
        if (preservedIndex >= 0)
        {
            _currentIndex = preservedIndex;
            return;
        }

        _currentIndex = FindFocusedIndex();
        if (_currentIndex < 0 && _targets.Length > 0)
            _currentIndex = Math.Clamp(oldIndex, 0, _targets.Length - 1);
    }

    public bool Focus(View target)
    {
        var index = Array.IndexOf(_targets, target);
        if (index < 0 || (!target.HasFocus && !target.SetFocus()))
            return false;

        _currentIndex = index;
        return true;
    }

    public bool Move(NavigationDirection direction)
    {
        if (_targets.Length == 0)
            return false;

        var current = FindFocusedIndex();
        if (current < 0)
            current = _currentIndex;

        var step = direction == NavigationDirection.Forward ? 1 : -1;
        var start = current < 0 ? (step > 0 ? -1 : 0) : current;
        for (var offset = 1; offset <= _targets.Length; offset++)
        {
            var index = (start + step * offset) % _targets.Length;
            if (index < 0)
                index += _targets.Length;

            var target = _targets[index];
            if (!target.Visible || !target.Enabled || !target.CanFocus)
                continue;

            if (!target.HasFocus && !target.SetFocus())
                continue;

            _currentIndex = index;
            return true;
        }

        return false;
    }

    private View? FindFocusedTarget()
    {
        var index = FindFocusedIndex();
        return index >= 0 ? _targets[index] : null;
    }

    private int FindFocusedIndex()
    {
        // Registered targets are actionable leaves (lists, editors, fields), not
        // their containing panels, so the one with HasFocus is the active control.
        for (var i = 0; i < _targets.Length; i++)
        {
            if (_targets[i].HasFocus)
                return i;
        }

        return -1;
    }
}
