﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Umamimolecule.AzureDurableFunctionsStatus.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Umamimolecule.AzureDurableFunctionsStatus.Resources.ExceptionMessages", typeof(ExceptionMessages).Assembly);
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
        ///   Looks up a localized string similar to No instance with ID &apos;{0}&apos; was found..
        /// </summary>
        internal static string InstanceNotFound {
            get {
                return ResourceManager.GetString("InstanceNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid value &apos;{0}&apos; for parameter &apos;{1}&apos;..
        /// </summary>
        internal static string InvalidParameterException {
            get {
                return ResourceManager.GetString("InvalidParameterException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid value &apos;{0}&apos; for parameter &apos;{1}&apos;.  Valid values are: {2}.
        /// </summary>
        internal static string InvalidParameterExceptionWithValues {
            get {
                return ResourceManager.GetString("InvalidParameterExceptionWithValues", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The required parameter &apos;{0}&apos; was missing..
        /// </summary>
        internal static string RequiredParameterMissingException {
            get {
                return ResourceManager.GetString("RequiredParameterMissingException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The required query parameter &apos;{0}&apos; was missing..
        /// </summary>
        internal static string RequiredQueryParameterMissingException {
            get {
                return ResourceManager.GetString("RequiredQueryParameterMissingException", resourceCulture);
            }
        }
    }
}
