using System.Reflection;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace IndustrialPark
{
    public class IPversion
    {
        public string version;
        public string versionName;

        private IPversion()
        {
        }

        public static IPversion CurrentVersion
        {
            get => new IPversion
            {
                version = AboutBox.AssemblyVersion,
                versionName = AboutBox.AssemblyVersion
            };
        }
    }
}
