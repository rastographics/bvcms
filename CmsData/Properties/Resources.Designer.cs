﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CmsData.Properties {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CmsData.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;FieldTypes&gt;
        ///  &lt;FieldType Name=&quot;Empty&quot; /&gt;
        ///  &lt;FieldType Name=&quot;Group&quot;&gt;
        ///    &lt;Comparison Type=&quot;AllTrue&quot; /&gt;
        ///    &lt;Comparison Type=&quot;AnyTrue&quot; /&gt;
        ///    &lt;Comparison Type=&quot;AllFalse&quot; /&gt;
        ///    &lt;Comparison Type=&quot;AnyFalse&quot; /&gt;
        ///  &lt;/FieldType&gt;
        ///  &lt;FieldType Name=&quot;Bit&quot;&gt;
        ///    &lt;Comparison Type=&quot;Equal&quot; Display=&quot;{0} = {1}&quot; /&gt;
        ///    &lt;Comparison Type=&quot;NotEqual&quot; Display=&quot;{0} &amp;lt;&amp;gt; {1}&quot; /&gt;
        ///  &lt;/FieldType&gt;
        ///  &lt;FieldType Name=&quot;EqualBit&quot;&gt;
        ///    &lt;Comparison Type=&quot;Equal&quot; Display=&quot;{0} = {1}&quot; /&gt;        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string CompareMap {
            get {
                return ResourceManager.GetString("CompareMap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;table&gt;
        ///{{#Registrant}}
        ///  &lt;tr&gt;
        ///    &lt;td colspan=&apos;2&apos; style=&apos;{{DataStyle}}{{BottomBorder}}&apos;&gt;
        ///      &lt;p&gt;
        ///        &lt;hr/&gt;
        ///        Registrant: &lt;registrant&gt;{{Name}}&lt;/registrant&gt;&lt;br/&gt;
        ///        for {{OrganizationName}}
        ///      &lt;/p&gt;
        ///    &lt;/td&gt;
        ///  &lt;/tr&gt;
        ///{{/Registrant}}
        ///{{#IfShowTransaction}}
        ///  &lt;tr&gt;
        ///    &lt;td colspan=&apos;2&apos; style=&apos;{{BottomBorder}}&apos;&gt;
        ///      &lt;table&gt;
        ///        &lt;tr&gt;
        ///          &lt;td style=&apos;{{LabelStyle}}&apos;&gt;Registrant Fee&lt;/td&gt;
        ///          &lt;td align=&apos;right&apos; style=&apos;{{DataStyle}}&apos;&gt;{{AmtFee}}&lt;/td&gt;
        ///        &lt;/tr&gt;
        ///  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Details2 {
            get {
                return ResourceManager.GetString("Details2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Name,Type,DataValueField,DataSource,QuartersLabel,Category,Title,Params,Description
        ///Group,Group,,,,Grouping,,,&quot;Groups conditions or other groups together in an &quot;&quot;all true&quot;&quot; or &quot;&quot;any one true&quot;&quot; relationship&quot;
        ///Age,NullInteger,,,,Personal,,,Enter an age. Leave blank to find those without a DOB.
        ///GenderId,Code,,GenderCodes,,Personal,Gender,,Select Gender from the list.
        ///MaritalStatusId,Code,,MaritalStatusCodes,,Personal,Marital Status,,Select Marital Status from the list.
        ///Birthday,StringEqual,,,,Personal,Birt [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string FieldMap {
            get {
                return ResourceManager.GetString("FieldMap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {first},
        ///&lt;p&gt;We are very sorry, but something has gone wrong and your online giving transaction did not complete.&lt;/p&gt;
        ///&lt;p&gt;Please contact the church for help in resolving this issue.&lt;/p&gt;
        ///&lt;p&gt;Thank you.&lt;/p&gt;
        ///.
        /// </summary>
        internal static string ManagedGiving_FailedGivingMessage {
            get {
                return ResourceManager.GetString("ManagedGiving_FailedGivingMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DECLARE @UpperLimit INT = 50000;	
        ///	IF NOT EXISTS(SELECT NULL FROM dbo.Numbers)	
        ///	BEGIN	
        ///		WITH n AS	
        ///		(	
        ///		    SELECT	
        ///		        rn = ROW_NUMBER() OVER	
        ///		        (ORDER BY s1.[object_id])	
        ///		    FROM sys.objects AS s1	
        ///		    CROSS JOIN sys.objects AS s2	
        ///		    CROSS JOIN sys.objects AS s3	
        ///		)	
        ///		INSERT dbo.Numbers ( Number )	
        ///		(SELECT rn - 1	
        ///		 FROM n	
        ///		 WHERE rn &lt;= @UpperLimit + 1)	
        ///	END.
        /// </summary>
        internal static string SetupNumbers {
            get {
                return ResourceManager.GetString("SetupNumbers", resourceCulture);
            }
        }
    }
}
