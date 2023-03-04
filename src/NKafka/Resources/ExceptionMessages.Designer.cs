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
    internal class ExceptionMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NKafka.Resources.ExceptionMessages", typeof(ExceptionMessages).Assembly);
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
        ///   Looks up a localized string similar to Failed to initialize the cluster within the specified time {0}ms.
        /// </summary>
        internal static string ClusterInitFailed {
            get {
                return ResourceManager.GetString("ClusterInitFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kafka message header key cannot be null.
        /// </summary>
        internal static string Kafka_message_header_key_cannot_be_null {
            get {
                return ResourceManager.GetString("Kafka_message_header_key_cannot_be_null", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connector to controller not found.
        /// </summary>
        internal static string NoConnectionToController {
            get {
                return ResourceManager.GetString("NoConnectionToController", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Controller not set.
        /// </summary>
        internal static string NoController {
            get {
                return ResourceManager.GetString("NoController", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The class selected as the custom partitioning algorithm cannot be created.
        /// </summary>
        internal static string PartitionerCreateError {
            get {
                return ResourceManager.GetString("PartitionerCreateError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No partitioning method found.
        /// </summary>
        internal static string PartitionerNotFound {
            get {
                return ResourceManager.GetString("PartitionerNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to construct kafka producer.
        /// </summary>
        internal static string Producer_CreateError {
            get {
                return ResourceManager.GetString("Producer_CreateError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Serializer not specified and there is no default serializer defined for type {0}..
        /// </summary>
        internal static string Producer_SerializerError {
            get {
                return ResourceManager.GetString("Producer_SerializerError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The operation cannot be performed because the producer was closed.
        /// </summary>
        internal static string ProducerClosed {
            get {
                return ResourceManager.GetString("ProducerClosed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unsupported mechanism for Sasl was specified.
        /// </summary>
        internal static string SaslMechanismInvalid {
            get {
                return ResourceManager.GetString("SaslMechanismInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The current version of Sasl is not correct.
        /// </summary>
        internal static string SaslVersionInvalid {
            get {
                return ResourceManager.GetString("SaslVersionInvalid", resourceCulture);
            }
        }
    }
}
