//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BMS {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.8.0.0")]
    internal sealed partial class Setting : global::System.Configuration.ApplicationSettingsBase {
        
        private static Setting defaultInstance = ((Setting)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Setting())));
        
        public static Setting Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string FileIndex {
            get {
                return ((string)(this["FileIndex"]));
            }
            set {
                this["FileIndex"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string StartPos {
            get {
                return ((string)(this["StartPos"]));
            }
            set {
                this["StartPos"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string NamePos {
            get {
                return ((string)(this["NamePos"]));
            }
            set {
                this["NamePos"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string LogFolderIndex {
            get {
                return ((string)(this["LogFolderIndex"]));
            }
            set {
                this["LogFolderIndex"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string WriteLogPos {
            get {
                return ((string)(this["WriteLogPos"]));
            }
            set {
                this["WriteLogPos"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int StationPLC {
            get {
                return ((int)(this["StationPLC"]));
            }
            set {
                this["StationPLC"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("COM1")]
        public string COMSylvac {
            get {
                return ((string)(this["COMSylvac"]));
            }
            set {
                this["COMSylvac"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("COM2")]
        public string COMDoDaoSylvac01 {
            get {
                return ((string)(this["COMDoDaoSylvac01"]));
            }
            set {
                this["COMDoDaoSylvac01"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("COM3")]
        public string COMDoDaoSylvac02 {
            get {
                return ((string)(this["COMDoDaoSylvac02"]));
            }
            set {
                this["COMDoDaoSylvac02"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("COM4")]
        public string COMDoDaoSylvac03 {
            get {
                return ((string)(this["COMDoDaoSylvac03"]));
            }
            set {
                this["COMDoDaoSylvac03"] = value;
            }
        }
    }
}
