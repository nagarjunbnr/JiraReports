using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optify
{
    /// <summary>
    /// Used to mark a property as the "Default" option. When in use, if no switches are specified, 
    /// the argument (if any) is used to populate the property marked as default. It should go
    /// without saying that this attribute should only be used on one property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultOptionAttribute : Attribute { }

    /// <summary>
    /// Used to specify the description of a flag, to be used when auto-generating help
    /// documentation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionDescriptionAttribute : Attribute
    {
        /// <summary>
        /// The description of the attached option. Used to auto-generate help.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        public OptionDescriptionAttribute(string description)
        {
            this.Description = description;
        }
    }

    /// <summary>
    /// Creates a group of switches for the purpose of restricting the number of switches in the 
    /// group that can be used at the same time. When used on a property, use the constructor to
    /// specify the group name only. Should be also used on the class to specify the range.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class OptionGroupAttribute : Attribute
    {
        /// <summary>
        /// The name of the group that is being created
        /// </summary>
        public string GroupName { get; private set; }

        /// <summary>
        /// The minimum number of concurrent switches in this group that is allowed. Defaults
        /// to zero.
        /// </summary>
        public int MinimumConcurrent { get; private set; } = 0;

        /// <summary>
        /// The maximum number of concurrent switches in this group that is allowed. Defaults
        /// to null.
        /// </summary>
        public int? MaximumConcurrent { get; private set; } = null;

        /// <summary>
        /// Adds a property to the group <paramref name="groupName"/>
        /// </summary>
        /// <param name="groupName"></param>
        public OptionGroupAttribute(string groupName)
        {
            this.GroupName = groupName;
        }

        /// <summary>
        /// Should be used on a class to specify the minimum and maximum concurrent options that are
        /// allowed in the specified group.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="minimumConcurrent"></param>
        /// <param name="maximumConcurrent"></param>
        public OptionGroupAttribute(string groupName, int minimumConcurrent, int maximumConcurrent) : this(groupName)
        {
            this.MinimumConcurrent = minimumConcurrent;
            this.MaximumConcurrent = maximumConcurrent;
        }
    }

    /// <summary>
    /// Indicates that the decorated option is required
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionRequiredAttribute : Attribute { }

    /// <summary>
    /// Indicates that the decorated option requires at least one of the specified flags
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiresOneOptionAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public char[] RequiredOptions { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public RequiresOneOptionAttribute(params char[] options)
        {
            this.RequiredOptions = options;
        }
    }

    /// <summary>
    /// Indicates that the decorated option requires all of the specified flags
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiresAllOptionsAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public char[] RequiredOptions { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public RequiresAllOptionsAttribute(params char[] options)
        {
            this.RequiredOptions = options;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionFlagAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public char Flag { get; private set; }

        public OptionFlagAttribute(char flag)
        {
            this.Flag = flag;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionFlagArgumentAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public char[] Flags { get; private set; }

        public OptionFlagArgumentAttribute(params char[] flags)
        {
            this.Flags = flags;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AssemblyNameAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="assemblyName"></param>
        public AssemblyNameAttribute()
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AssemblyNameFlagAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public char Flag { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssemblyName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="assemblyName"></param>
        public AssemblyNameFlagAttribute(char flag, string assemblyName)
        {
            this.Flag = flag;
            this.AssemblyName = assemblyName;
        }
    }
}
