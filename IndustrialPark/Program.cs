using IndustrialPark.Archive;
using IndustrialPark.SaveFile;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace IndustrialPark;

public static class Program
{
    public static MainForm MainForm;
    public static ArchiveEditorManager ArchiveManager;
    public static List<ArchiveEditor> ArchiveEditors => ArchiveManager.ArchiveEditors;
    public static AssetEditorManager EditorManager;
    public static NameRepository NameRepository;
    public static SharpRenderer Renderer;
    public static TextureManager TextureManager;
    public static ViewConfig ViewConfig;
    public static ViewControls ViewControls;
    public static AboutBox AboutBox;
    public static UserTemplateManager UserTemplateManager;
    public static UndoBuffer UndoBuffer;
    public static BuildISO BuildISO;
    public static HansMainForm HansMainForm;
    public static EventSearch EventSearch;
    public static DynaSearch DynaSearch;
    public static AssetIDGenerator AssetIDGenerator;
    public static PickupSearch PickupSearch;

    public static bool IsDarkMode;

    [STAThread]
    static void Main()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-us");

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        if (!SharpDevice.IsDirectX11Supported())
        {
            MessageBox.Show("DirectX11 feature level 11.0 is required to run Industrial Park. Maximum supported feature level is " + SharpDevice.GetSupportedFeatureLevel().ToString() + ". Please update your DirectX.");
            return;
        }

        Application.SetDefaultFont(new System.Drawing.Font(new FontFamily("Microsoft Sans Serif"), 8.25f));

        IPSettings ipSettings = LoadSettingsFile();

        UndoBuffer = new UndoBuffer();

        // Force Classic or Dark mode, because System mode mixes colors from both modes if the system mode is changed with the application running.
        IsDarkMode = GetIsDarkMode(ipSettings.ColorMode);
        Application.SetColorMode(IsDarkMode ? SystemColorMode.Dark : SystemColorMode.Classic);

        ArchiveManager = new ArchiveEditorManager();
        TextureManager = new TextureManager();
        EditorManager = new AssetEditorManager();
        NameRepository = new NameRepository();


        MainForm = new MainForm(ipSettings);

        ViewConfig = new ViewConfig();
        ViewControls = new ViewControls();
        AboutBox = new AboutBox();
        UserTemplateManager = new UserTemplateManager();
        Application.Run(MainForm);
    }

    private static IPSettings LoadSettingsFile()
    {
        if (File.Exists(MainForm.pathToSettings))
        {
            try
            {
                return JsonConvert.DeserializeObject<IPSettings>(File.ReadAllText(MainForm.pathToSettings));
            }
            catch
            {
            }
        }
        return new IPSettings();
    }

    static bool GetIsDarkMode(SystemColorMode ipLocalColorMode)
    {
        switch (ipLocalColorMode)
        {
            case SystemColorMode.Dark:
                return true;
            case SystemColorMode.Classic:
                return false;
        }

        object value = Registry.GetValue(
            @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
            "AppsUseLightTheme",
            1);

        return Convert.ToInt32(value) == 0;
    }
}
