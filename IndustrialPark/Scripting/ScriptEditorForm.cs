using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark.Scripting
{
    public sealed class ScriptEditorForm : Form
    {
        private readonly RichTextBox scriptEditor;
        private readonly RichTextBox output;
        private readonly Button runButton;
        private string currentFilePath;

        public ScriptEditorForm()
        {
            Text = "C# Script Editor";
            StartPosition = FormStartPosition.CenterParent;
            Width = 900;
            Height = 700;
            MinimumSize = new Size(600, 400);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(6)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight
            };

            var openButton = new Button { Text = "Open", AutoSize = true };
            openButton.Click += OpenScript;
            var saveButton = new Button { Text = "Save", AutoSize = true };
            saveButton.Click += SaveScript;
            runButton = new Button { Text = "Run", AutoSize = true };
            runButton.Click += RunScript;
            var warning = new Label
            {
                Text = "Only run trusted scripts; C# scripts have full process permissions.",
                AutoSize = true,
                Padding = new Padding(8, 8, 0, 0)
            };
            toolbar.Controls.Add(openButton);
            toolbar.Controls.Add(saveButton);
            toolbar.Controls.Add(runButton);
            toolbar.Controls.Add(warning);

            scriptEditor = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font(FontFamily.GenericMonospace, 10),
                AcceptsTab = true,
                WordWrap = false,
                Text = "Log(\"Hello from Industrial Park!\");"
            };

            output = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font(FontFamily.GenericMonospace, 9),
                ReadOnly = true,
                WordWrap = false
            };

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 400,
                Panel1MinSize = 100,
                Panel2MinSize = 100
            };
            splitContainer.Panel1.Controls.Add(scriptEditor);
            splitContainer.Panel2.Controls.Add(output);

            layout.Controls.Add(toolbar, 0, 0);
            layout.Controls.Add(splitContainer, 0, 1);
            Controls.Add(layout);
        }

        private async void RunScript(object sender, EventArgs e)
        {
            runButton.Enabled = false;
            AppendOutput($"Running {Path.GetFileName(currentFilePath) ?? "script"}...");

            try
            {
                ScriptOptions options = ScriptOptions.Default
                    .AddReferences(typeof(ScriptGlobals).Assembly)
                    .AddImports("System", "System.IO", "System.Linq", "System.Collections.Generic", "IndustrialPark", "IndustrialPark.Scripting");
                ScriptGlobals globals = new ScriptGlobals(AppendOutput);
                var state = await CSharpScript.RunAsync(scriptEditor.Text, options, globals);
                if (state.ReturnValue != null)
                    AppendOutput(state.ReturnValue.ToString());
                AppendOutput("Script completed.");
            }
            catch (CompilationErrorException ex)
            {
                AppendOutput(string.Join(Environment.NewLine, ex.Diagnostics.Select(diagnostic => diagnostic.ToString())));
            }
            catch (Exception ex)
            {
                AppendOutput(ex.ToString());
            }
            finally
            {
                runButton.Enabled = true;
            }
        }

        private void OpenScript(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Filter = "C# scripts (*.csx;*.cs)|*.csx;*.cs|All files (*.*)|*.*",
                CheckFileExists = true
            };
            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                scriptEditor.Text = File.ReadAllText(dialog.FileName);
                currentFilePath = dialog.FileName;
                Text = $"C# Script Editor - {Path.GetFileName(currentFilePath)}";
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Could not open script", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveScript(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                using var dialog = new SaveFileDialog
                {
                    Filter = "C# scripts (*.csx)|*.csx|C# files (*.cs)|*.cs|All files (*.*)|*.*",
                    DefaultExt = "csx",
                    AddExtension = true
                };
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;
                currentFilePath = dialog.FileName;
            }

            try
            {
                File.WriteAllText(currentFilePath, scriptEditor.Text);
                Text = $"C# Script Editor - {Path.GetFileName(currentFilePath)}";
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Could not save script", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AppendOutput(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(AppendOutput), text);
                return;
            }

            output.AppendText(text + Environment.NewLine);
        }
    }
}
