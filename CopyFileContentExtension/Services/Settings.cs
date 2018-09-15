using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyFileContentExtension.Services
{
    public sealed class Settings : INotifyPropertyChanged
    {
        private const string SETTINGS_REGISTRY_PATH = "Software\\CopyFileContentExtension";
        const string PATTERNS_VALUE_NAME = "FilePatterns";
        const string PATTERNS_DEFAULT_VALUE = "*.sql";
        public static Settings Instance { get; private set; }

        static Settings()
        {
            Instance = new Settings();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Settings() { LoadSettings(); }

        private string m_FIlePatterns;


        public string FIlePatterns
        {
            get
            {
                return m_FIlePatterns;
            }
            set
            {
                if (m_FIlePatterns != value)
                {
                    m_FIlePatterns = value;
                    SaveSettings();
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FIlePatterns)));
                }
            }
        }

        private void SaveSettings()
        {
            using (var key =
            RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default)
            .CreateSubKey(SETTINGS_REGISTRY_PATH))
            {
                key.SetValue(PATTERNS_VALUE_NAME, m_FIlePatterns);
            }
        }

        private void LoadSettings()
        {
            using (var key = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default)
            .OpenSubKey(SETTINGS_REGISTRY_PATH))
            {
                if (key != null)
                {
                    var value = key.GetValue(PATTERNS_VALUE_NAME, null);
                    if (value is string)
                    {
                        m_FIlePatterns = value as string;
                    }
                }
            }

            m_FIlePatterns = m_FIlePatterns ?? PATTERNS_DEFAULT_VALUE;
        }
    }
}
