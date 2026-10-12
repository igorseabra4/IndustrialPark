using System;
using System.Windows.Forms;

namespace IndustrialPark;

public class UndoRedoMessageFilter : IMessageFilter
{
    private readonly Form _form;
    private readonly Action _undo;
    private readonly Action _redo;

    public UndoRedoMessageFilter(Form form, Action undo = null, Action redo = null)
    {
        _form = form;
        _undo = undo ?? Program.UndoBuffer.Undo;
        _redo = redo ?? Program.UndoBuffer.Redo;

        Application.AddMessageFilter(this);
        _form.FormClosed += (_, _) => Dispose();
    }

    public bool PreFilterMessage(ref Message m)
    {
        const int WM_KEYDOWN = 0x0100;

        if (m.Msg != WM_KEYDOWN || _form.IsDisposed || !_form.Visible)
            return false;

        Control focused = Control.FromHandle(m.HWnd);

        if (focused == null || !IsInsideForm(focused))
            return false;

        if ((Control.ModifierKeys & Keys.Control) == 0)
            return false;

        Keys key = (Keys)(int)m.WParam;
        bool shift = (Control.ModifierKeys & Keys.Shift) != 0;

        if (key == Keys.Z)
        {
            if (shift)
                _redo();
            else
                _undo();

            return true;
        }

        if (key == Keys.Y)
        {
            _redo();
            return true;
        }

        return false;
    }

    private bool IsInsideForm(Control control)
    {
        while (control != null)
        {
            if (control == _form)
                return true;

            control = control.Parent;
        }

        return false;
    }

    public void Dispose()
    {
        Application.RemoveMessageFilter(this);
    }
}