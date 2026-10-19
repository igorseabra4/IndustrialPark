using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IndustrialPark;

public partial class MainForm
{   
    // Native Windows API methods to force mouse capture continuation
    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    private const int WM_NCLBUTTONDOWN = 0x00A1;
    private const int HTCAPTION = 2;

    private int docked => tabControlRight.TabPages.Count;
    private Point dragStartPoint;
    private bool isDraggingTab = false;
    private Rectangle initialDragRectangle = default;
    private Rectangle imaginaryDockingZone = default;

    public void InitAdvancedDocking()
    {
        tabControlRight.Multiline = true;
        tabControlRight.AllowDrop = true;
        tabControlRight.Padding = new Point(15, 4);
        tabControlRight.MouseDown += tabControl1_MouseDown;
        tabControlRight.MouseMove += tabControl1_MouseMove;

        this.LocationChanged += (_, _) => UpdateInitialDockingArea();
        this.SizeChanged += (_, _) => UpdateInitialDockingArea();
        this.Layout += (_, _) => UpdateInitialDockingArea();

        splitContainer2.Layout += (_, _) => UpdateInitialDockingArea();
        splitContainer2.SplitterMoved += (_, _) => UpdateInitialDockingArea();

        this.Shown += (_, _) => UpdateInitialDockingArea();
    }

    private void UpdateInitialDockingArea()
    {
        if (IsDisposed || !IsHandleCreated || splitContainer2.IsDisposed || !splitContainer2.IsHandleCreated)
            return;
        Rectangle bounds = splitContainer2.RectangleToScreen(splitContainer2.ClientRectangle);
        int width = Math.Min(360, bounds.Width);
        imaginaryDockingZone = new Rectangle(bounds.Right - width, bounds.Top, width, bounds.Height);
    }

    private void tabControl1_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            dragStartPoint = e.Location;
            isDraggingTab = true;
            initialDragRectangle = tabControlRight.RectangleToScreen(tabControlRight.ClientRectangle);
        }
    }

    private void tabControl1_MouseMove(object sender, MouseEventArgs e)
    {
        if (!isDraggingTab || e.Button != MouseButtons.Left)
            return;

        if (!initialDragRectangle.Contains(Cursor.Position))
        {
            isDraggingTab = false;

            for (int i = 0; i < tabControlRight.TabCount; i++)
            {
                if (tabControlRight.GetTabRect(i).Contains(dragStartPoint))
                {
                    UndockEditor(i);
                    break;
                }
            }
        }
    }

    public void DockEditor(InternalEditor editor)
    {
        if (!editor.CanBeDocked)
            return;

        if (docked == 0)
        {
            splitContainer2.SplitterDistance = splitContainer2.Width - editor.Width;
            splitContainer2.Panel2Collapsed = false;
        }

        var page = new TabPage(editor.Text);

        editor.TopLevel = false;
        editor.FormBorderStyle = FormBorderStyle.None;
        editor.Dock = DockStyle.Fill;
        editor.IsDocked = true;

        page.Controls.Add(editor);
        tabControlRight.TabPages.Add(page);
        tabControlRight.SelectedTab = page;
    }

    public void UndockEditor(int index)
    {
        TabPage page = tabControlRight.TabPages[index];
        InternalEditor editor = (InternalEditor)page.Controls[0];

        page.Controls.Remove(editor);
        tabControlRight.TabPages.RemoveAt(index);

        editor.TopLevel = true;
        editor.FormBorderStyle = FormBorderStyle.Sizable;
        editor.Dock = DockStyle.None;
        editor.IsDocked = false;
        editor.Location = new Point(Cursor.Position.X - (editor.Width / 2), Cursor.Position.Y - 15);

        ReleaseCapture();
        SendMessage(editor.Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);

        if (docked == 0 && collapse)
        {
            imaginaryDockingZone = tabControlRight.RectangleToScreen(tabControlRight.ClientRectangle);
            splitContainer2.Panel2Collapsed = true;
        }
    }

    public void UndockEditor(InternalEditor editor, bool closing)
    {
        foreach (TabPage page in tabControlRight.TabPages)
        {
            if (page.Controls.Contains(editor))
            {
                page.Controls.Remove(editor);
                tabControlRight.TabPages.Remove(page);

                if (!closing)
                {
                    editor.TopLevel = true;
                    editor.FormBorderStyle = FormBorderStyle.Sizable;
                    editor.Dock = DockStyle.None;
                    editor.IsDocked = false;
                    editor.Location = new Point(Cursor.Position.X - 100, Cursor.Position.Y - 15);
                }

                if (docked == 0 && collapse)
                {
                    imaginaryDockingZone = tabControlRight.RectangleToScreen(tabControlRight.ClientRectangle);
                    splitContainer2.Panel2Collapsed = true;
                }
                return;
            }
        }
    }

    bool collapse = true;

    public void ClearDockedAssetEditors()
    {
        collapse = false;
        foreach (TabPage page in tabControlRight.TabPages.Cast<TabPage>().ToList())
        {
            InternalEditor editor = (InternalEditor)page.Controls[0];
            editor.Close();
        }
        collapse = true;
    }

    public void editor_FormDropped(object sender, EventArgs e)
    {
        InternalEditor editor = sender as InternalEditor;
        if (editor == null)
            return;

        if (this.IsPointerInDockingZone())
        {
            ReleaseCapture();
            DockEditor(editor);
        }
    }

    public bool IsPointerInDockingZone()
    {
        Point mousePos = Cursor.Position;

        if (docked > 0)
        {
            Rectangle currentBounds = tabControlRight.RectangleToScreen(tabControlRight.ClientRectangle);
            return currentBounds.Contains(mousePos);
        }
        else
        {
            return imaginaryDockingZone.Contains(mousePos);
        }
    }
}
