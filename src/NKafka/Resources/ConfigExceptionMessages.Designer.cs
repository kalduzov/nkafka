﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NKafka.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ConfigExceptionMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ConfigExceptionMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NKafka.Resources.ConfigExceptionMessages", typeof(ConfigExceptionMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Incorrect broker version set. Minimum supported {0} Maximum supported {0}.
        /// </summary>
        internal static string BrokerVersionInvalid {
            get {
                return ResourceManager.GetString("BrokerVersionInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BootstrapServers not set.
        /// </summary>
        internal static string No_bootstrap_servers {
            get {
                return ResourceManager.GetString("No_bootstrap_servers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to BrokerVersion not set.
        /// </summary>
        internal static string NoBrokerVersion {
            get {
                return ResourceManager.GetString("NoBrokerVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to In the settings, the type of distribution by sections Custom is specified, but the custom class is not specified.
        /// </summary>
        internal static string PartitionerConfig_CustomClassNotFound {
            get {
                return ResourceManager.GetString("PartitionerConfig_CustomClassNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified custom type does not inherit the &apos;IPartitioner&apos; interface.
        /// </summary>
        internal static string PartitionerConfig_InterfaceInvalid {
            get {
                return ResourceManager.GetString("PartitionerConfig_InterfaceInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The settings specify a custom partitioning algorithm class, but the distribution type is specified other than Custom.
        /// </summary>
        internal static string PartitionerConfig_TypePartitionerInvalid {
            get {
                return ResourceManager.GetString("PartitionerConfig_TypePartitionerInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The SecurityProtocols property is configured to work with Sasl, but there is no configuration for Sasl..
        /// </summary>
        internal static string Sasl_no_configured {
            get {
                return ResourceManager.GetString("Sasl_no_configured", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The SecurityProtocols property is configured to work with Ssl, but there is no configuration for Ssl..
        /// </summary>
        internal static string Ssl_no_configured {
            get {
                return ResourceManager.GetString("Ssl_no_configured", resourceCulture);
            }
        }
    }
}
