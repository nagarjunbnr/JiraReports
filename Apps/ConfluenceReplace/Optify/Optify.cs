using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupRangeDictionary = System.Collections.Generic.Dictionary<string, Optify.OptionGroupRange>;
using GroupFlagsDictionary = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<char>>;
using FlagDescriptionDictionary = System.Collections.Generic.Dictionary<char, string>;
using FlagGroupsDictionary = System.Collections.Generic.Dictionary<char, System.Collections.Generic.List<string>>;
using FlagRequiresDictionary = System.Collections.Generic.Dictionary<char, char[]>;
using FlagRequiredDictionary = System.Collections.Generic.Dictionary<char, bool>;
using FlagPropertyDictionary = System.Collections.Generic.Dictionary<char, Optify.FlagPropertyInfo>;
using FlagPropertyNames = System.Collections.Generic.Dictionary<string, char>;
using AssemblyNamePropertyDictionary = System.Collections.Generic.Dictionary<string, System.Reflection.PropertyInfo>;
using UsedFlagDictionary = System.Collections.Generic.Dictionary<char, bool>;

namespace Optify
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Options
    {
        protected static readonly Type BoolType = typeof(bool);
        protected static readonly Type LongType = typeof(long);
        protected static readonly Type IntType = typeof(int);
        protected static readonly Type ShortType = typeof(short);
        protected static readonly Type ByteType = typeof(byte);
        protected static readonly Type StringType = typeof(string);
        protected static readonly Type GuidType = typeof(Guid);
        protected static readonly Type DecimalType = typeof(decimal);
        protected static readonly Type FloatType = typeof(float);
        protected static readonly Type DoubleType = typeof(double);

        private static readonly Dictionary<Type, Func<string, object>> parsers
            = new Dictionary<Type, Func<string, object>>()
        {
            { BoolType, (s) => bool.Parse(s) },
            { LongType, (s) => long.Parse(s) },
            { IntType, (s) => int.Parse(s) },
            { ShortType, (s) => short.Parse(s) },
            { ByteType, (s) => byte.Parse(s) },
            { StringType, (s) => s },
            { GuidType, (s) => Guid.Parse(s) },
            { DecimalType, (s) => decimal.Parse(s) },
            { FloatType, (s) => float.Parse(s) },
            { DoubleType, (s) => double.Parse(s) },
        };

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOptionType"></typeparam>
        /// <param name="commandLine"></param>
        /// <returns></returns>
        public static TOptionType Parse<TOptionType>(string commandLine)
            where TOptionType : new()
        {
            Options<TOptionType> options = new Options<TOptionType>();
            return options.ParseCommandLine(commandLine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected static object ParseType(Type type, string value)
        {
            return parsers[type](value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        protected static TType ParseType<TType>(string value)
        {
            return (TType)ParseType(typeof(TType), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testType"></param>
        /// <returns></returns>
        protected static bool IsAllowableType(Type testType)
        {
            return testType == BoolType || testType == LongType
                || testType == IntType || testType == ShortType
                || testType == ByteType || testType == StringType
                || testType == GuidType || testType == DecimalType
                || testType == FloatType || testType == DoubleType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testType"></param>
        /// <returns></returns>
        protected static Type GetBaseType(Type testType)
        {
            if (testType.GetTypeInfo().IsGenericType && testType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return testType.GenericTypeArguments.First();
            }

            return testType;
        }

        protected static PropertyInfo[] GetClassProperties(Type optionType)
        {
            List<PropertyInfo> basePropertyList = optionType.GetRuntimeProperties().ToList();
            List<PropertyInfo> orderedPropertyList = new List<PropertyInfo>();

            orderedPropertyList.AddRange(basePropertyList.Where(p => GetBaseType(p.PropertyType) == BoolType));
            orderedPropertyList.AddRange(basePropertyList.Where(p => GetBaseType(p.PropertyType) != BoolType));

            return orderedPropertyList.ToArray();
        }

        internal Options() { }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TOptionType"></typeparam>
    public partial class Options<TOptionType> : Options
        where TOptionType : new()
    {
        private static readonly Type OptionType = typeof(TOptionType);
        private static readonly Attribute[] ClassAttributes = OptionType.GetTypeInfo().GetCustomAttributes().ToArray();
        private static readonly PropertyInfo[] ClassProperties = GetClassProperties(OptionType);
        private static readonly StringComparer defaultComparer = StringComparer.OrdinalIgnoreCase;

        private Queue<char> activeFlags = new Queue<char>();
        private char? defaultFlag = null;
        private PropertyInfo defaultOptionProperty = null;
        private PropertyInfo assemblyNameProperty = null;

        private FlagDescriptionDictionary optionDescriptions = new FlagDescriptionDictionary();
        private FlagGroupsDictionary groupsPerFlag = new FlagGroupsDictionary();
        private FlagRequiresDictionary requiresOneOptions = new FlagRequiresDictionary();
        private FlagRequiresDictionary requiresAllOptions = new FlagRequiresDictionary();
        private FlagRequiredDictionary requiredFlags = new FlagRequiredDictionary();
        private FlagPropertyDictionary propertyFlags = new FlagPropertyDictionary();
        private FlagPropertyNames flagPropertyNames = new FlagPropertyNames();

        private GroupRangeDictionary optionGroups = new GroupRangeDictionary();
        private GroupFlagsDictionary flagsPerGroup = new GroupFlagsDictionary();
        private AssemblyNamePropertyDictionary assemblyNameProperties = new AssemblyNamePropertyDictionary(defaultComparer);

        private bool defaultFlagExplicitlyUsed = false;
        private bool defaultFlagImplicitlyUsed = false;
        private UsedFlagDictionary usedFlags = new UsedFlagDictionary();
        private UsedFlagDictionary implicitlyUsedFlags = new UsedFlagDictionary();


        private bool firstToken = true;
        private TOptionType option = new TOptionType();

        internal Options() : base() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandLine"></param>
        /// <returns></returns>
        public TOptionType ParseCommandLine(string commandLine)
        {
            ParseOptionClassDefinition();

            foreach (Token token in CommandLineTokenizer.Parse(commandLine))
            {
                ParseToken(token);
                this.firstToken = false;
            }

            Validate();

            return option;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void ParseOptionClassDefinition()
        {
            foreach (PropertyInfo property in ClassProperties)
            {
                ParseOptionPropertyDefinition(property);
            }

            ValidateGroupDefinition();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ValidateGroupDefinition()
        {
            foreach (var kvp in this.flagsPerGroup)
            {
                string group = kvp.Key;
                List<char> flagsInGroup = kvp.Value;

                bool foundGroup = false;
                foreach (Attribute attribute in ClassAttributes)
                {
                    OptionGroupAttribute classGroupAttr = attribute as OptionGroupAttribute;
                    if (classGroupAttr == null) continue;

                    foundGroup = true;

                    if (classGroupAttr.MaximumConcurrent < classGroupAttr.MinimumConcurrent)
                        throw new Exception($"Maximum for group '{group}' is greater than minimum!");

                    if (classGroupAttr.MinimumConcurrent < 0)
                        throw new Exception($"Minimum for group '{group}' must be zero or greater!");

                    if (classGroupAttr.MaximumConcurrent == 0)
                        throw new Exception($"Maximum for group '{group}' must be greater than zero (or unspecified)!");

                    if (classGroupAttr.MinimumConcurrent > flagsInGroup.Count)
                        throw new Exception($"There are not enough flags to satisfy minimum for group '{group}'!");

                    if (this.optionGroups.ContainsKey(group))
                        throw new Exception($"Group '{group}' is specified on option class more than once!");

                    this.optionGroups[group] = new OptionGroupRange()
                    {
                        MaximumConcurrent = classGroupAttr.MaximumConcurrent,
                        MinimumConcurrent = classGroupAttr.MinimumConcurrent
                    };

                    break;
                }

                if (!foundGroup)
                    throw new Exception($"Group '{group}' does not have range defined on the class");
            }

            // TODO: determine if there is an impossible combination of group ranges for flags
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        private void ParseOptionPropertyDefinition(PropertyInfo property)
        {
            PropertyOptions propertyOptions = ParsePropertyOptions(property);

            if (propertyOptions.FlagArguments.Count > 0)
            {
                foreach(char flagArgument in propertyOptions.FlagArguments)
                {
                    FlagPropertyInfo info = this.propertyFlags[flagArgument];
                    info.ArgumentInfo = property;
                }
                return;
            }

            if (propertyOptions.IsAssemblyNameProperty)
            {
                this.assemblyNameProperty = property;
                return;
            }

            if (!String.IsNullOrWhiteSpace(propertyOptions.AssemblyName))
            {
                if (!this.assemblyNameProperties.ContainsKey(propertyOptions.AssemblyName))
                {
                    this.assemblyNameProperties[propertyOptions.AssemblyName] = property;
                }

                if (propertyOptions.Flag == null)
                    return;
            }

            if (propertyOptions.IsDefault)
            {
                this.defaultFlag = propertyOptions.Flag.Value;
                this.defaultOptionProperty = property;
            }

            if (propertyOptions.RequiresOne != null)
                this.requiresOneOptions[propertyOptions.Flag.Value] = propertyOptions.RequiresOne;

            if (propertyOptions.RequiresAll != null)
                this.requiresAllOptions[propertyOptions.Flag.Value] = propertyOptions.RequiresAll;

            this.requiredFlags[propertyOptions.Flag.Value] = propertyOptions.IsRequired;
            this.groupsPerFlag[propertyOptions.Flag.Value] = propertyOptions.Groups;
            this.optionDescriptions[propertyOptions.Flag.Value] = propertyOptions.Description;
            this.flagPropertyNames[property.Name] = propertyOptions.Flag.Value;

            if (propertyOptions.PropertyType == BoolType)
            {
                this.propertyFlags[propertyOptions.Flag.Value] = new FlagPropertyInfo() { FlagInfo = property };
            }
            else
            {
                this.propertyFlags[propertyOptions.Flag.Value] = new FlagPropertyInfo() { FlagInfo = property, ArgumentInfo = property };
            }


            foreach (string group in propertyOptions.Groups)
            {
                if (this.flagsPerGroup.TryGetValue(group, out List<char> flags))
                {
                    flags.Add(propertyOptions.Flag.Value);
                }
                else
                {
                    this.flagsPerGroup[group] = new List<char>() { propertyOptions.Flag.Value };
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private PropertyOptions ParsePropertyOptions(PropertyInfo property)
        {
            PropertyOptions propertyOptions = new PropertyOptions();
            Attribute[] propertyAttribs = property.GetCustomAttributes().ToArray();

            propertyOptions.PropertyType = GetBaseType(property.PropertyType);

            foreach (Attribute propertyAttrib in propertyAttribs)
            {
                switch (propertyAttrib)
                {
                    case DefaultOptionAttribute defaultAttrib:
                        propertyOptions.IsDefault = true;
                        break;
                    case OptionDescriptionAttribute descAttrib:
                        propertyOptions.Description = descAttrib.Description;
                        break;
                    case OptionGroupAttribute groupAttrib:
                        propertyOptions.Groups.Add(groupAttrib.GroupName);
                        break;
                    case OptionRequiredAttribute requiredAttrib:
                        propertyOptions.IsRequired = true;
                        break;
                    case RequiresOneOptionAttribute requiresOneAttrib:
                        propertyOptions.RequiresOne = requiresOneAttrib.RequiredOptions;
                        break;
                    case RequiresAllOptionsAttribute requiresAllAttrib:
                        propertyOptions.RequiresAll = requiresAllAttrib.RequiredOptions;
                        break;
                    case OptionFlagAttribute flagAttr:
                        propertyOptions.Flag = flagAttr.Flag;
                        break;
                    case OptionFlagArgumentAttribute flagArgAttr:
                        propertyOptions.FlagArguments.AddRange(flagArgAttr.Flags);
                        break;
                    case AssemblyNameAttribute asmAttr:
                        propertyOptions.IsAssemblyNameProperty = true;
                        break;
                    case AssemblyNameFlagAttribute asmAttr:
                        propertyOptions.AssemblyName = asmAttr.AssemblyName;
                        propertyOptions.Flag = asmAttr.Flag;
                        break;
                }
            }

            ValidatePropertyOptions(property, propertyOptions);

            return propertyOptions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="propertyOptions"></param>
        private void ValidatePropertyOptions(PropertyInfo property, PropertyOptions propertyOptions)
        {
            if (IsAllowableType(propertyOptions.PropertyType) == false)
            {
                throw new Exception(
                    "Property must be of type bool, long, int, short, byte, string, Gid, decimal, float or double.");
            }

            if(propertyOptions.FlagArguments.Count > 0)
            {
                foreach(char flagArgument in propertyOptions.FlagArguments)
                {
                    if(propertyFlags.TryGetValue(flagArgument, out FlagPropertyInfo value))
                    {
                        if (value.FlagInfo != null && GetBaseType(value.FlagInfo.PropertyType) == BoolType)
                            continue;
                    }

                    throw new Exception($"A boolean property must be marked with flag '{flagArgument}' in order to specify an argument for it.");
                }

                return;
            }

            if (propertyOptions.IsAssemblyNameProperty)
            {
                if (GetBaseType(propertyOptions.PropertyType) != StringType)
                    throw new Exception("Only string properties can be populated with the assembly name");

                if (this.assemblyNameProperty != null)
                {
                    throw new Exception("Only one property at a time can be populated with the assembly name!");
                }

                return;
            }

            if (!String.IsNullOrWhiteSpace(propertyOptions.AssemblyName))
            {
                if (GetBaseType(propertyOptions.PropertyType) != BoolType)
                    throw new Exception("Only boolean properties can be switched based on assembly name");

                if (this.assemblyNameProperties.ContainsKey(propertyOptions.AssemblyName))
                {
                    throw new Exception("Only one flag at a time can have the same assembly name associated!");
                }

                if (propertyOptions.Flag == null)
                    return;
            }

            if (propertyOptions.Flag == null)
                throw new Exception("A property must have an associated flag!");

            if (!char.IsLetter(propertyOptions.Flag.Value))
                throw new Exception("A flag must be alphanumeric!");

            if (propertyOptions.IsDefault && this.defaultOptionProperty != null)
                throw new Exception("Only one default property may exist!");

            if (this.propertyFlags.ContainsKey(propertyOptions.Flag.Value))
                throw new Exception("The same flag cannot be used on more than one property!");

            if (propertyOptions.IsDefault)
            {
                if (propertyOptions.PropertyType == BoolType)
                {
                    throw new Exception("Default flag cannot be a true/false switch.");
                }
            }
        }

        /// <summary>
        /// Validates the provided option object, given the definition
        /// </summary>
        private void Validate()
        {
            ValidateRequiredOptions();
            ValidateRequiresOneOptions();
            ValidateRequiresAllOptions();
            ValidateGroups();

            if (this.activeFlags.Count > 0)
                throw new Exception($"Flags are missing required arguments: '{String.Join(",", activeFlags)}'");
        }

        /// <summary>
        /// Validates that all required options were satisfied
        /// </summary>
        private void ValidateRequiredOptions()
        {
            foreach (var required in this.requiredFlags)
            {
                if (required.Value == false) continue;

                if (required.Key == this.defaultFlag && this.defaultFlagImplicitlyUsed)
                    continue;

                if (!this.usedFlags.ContainsKey(required.Key))
                    throw new Exception($"Required flag '{required.Key}' was not found.");
            }
        }

        /// <summary>
        /// Validates that all options that depend on at least one other option are satisfied
        /// </summary>
        private void ValidateRequiresOneOptions()
        {
            foreach (var kvp in this.requiresOneOptions)
            {
                bool foundOne = false;
                foreach (char requiredOption in kvp.Value)
                {
                    if (foundOne) break;

                    if (this.usedFlags.ContainsKey(requiredOption))
                    {
                        foundOne = true;
                    }
                }

                if (!foundOne)
                    throw new Exception($"Flag '{kvp.Key}' requires one or more of the following flags: '{String.Join(",", kvp.Value)}'");
            }
        }

        /// <summary>
        /// Validates that all options that reqire multiple other options are satisfied
        /// </summary>
        private void ValidateRequiresAllOptions()
        {
            foreach (var kvp in this.requiresAllOptions)
            {
                if (!this.usedFlags.ContainsKey(kvp.Key)) continue;

                foreach (char requiredOption in kvp.Value)
                {
                    if (!this.usedFlags.ContainsKey(requiredOption))
                    {
                        throw new Exception($"Flag '{kvp.Key}' requires all of the following flags: '{String.Join(",", kvp.Value)}'");
                    }
                }
            }
        }

        /// <summary>
        /// Validates that flags in groups are in the prescribed range
        /// </summary>
        private void ValidateGroups()
        {
            Dictionary<string, int> flagsInGroups = new Dictionary<string, int>();

            foreach (char usedFlag in this.usedFlags.Keys)
            {
                if (this.groupsPerFlag.TryGetValue(usedFlag, out List<string> groups))
                {
                    foreach (string group in groups)
                    {
                        if (flagsInGroups.TryGetValue(group, out int count))
                            flagsInGroups[group] = count + 1;
                        else
                            flagsInGroups[group] = 1;
                    }
                }
            }

            foreach(char implicitFlag in this.implicitlyUsedFlags.Keys)
            {
                if (this.usedFlags.ContainsKey(implicitFlag)) continue;

                if (this.groupsPerFlag.TryGetValue(implicitFlag, out List<string> groups))
                {
                    foreach (string group in groups)
                    {
                        if (flagsInGroups.TryGetValue(group, out int count))
                            flagsInGroups[group] = count + 1;
                        else
                            flagsInGroups[group] = 1;
                    }
                }
            }

            foreach (var kvp in this.optionGroups)
            {
                int count;

                if(!flagsInGroups.TryGetValue(kvp.Key, out count))
                {
                    count = 0;
                }

                if (count < kvp.Value.MinimumConcurrent)
                {
                    List<char> flags = this.flagsPerGroup[kvp.Key];
                    throw new Exception($"At least {kvp.Value.MinimumConcurrent} of the following flags must be used at once: '{String.Join(",", flags)}'");
                }

                if (kvp.Value.MaximumConcurrent != null && count > kvp.Value.MaximumConcurrent)
                {
                    List<char> flags = this.flagsPerGroup[kvp.Key];
                    throw new Exception($"No more than {kvp.Value.MaximumConcurrent} of the following flags can be used at once: '{String.Join(",", flags)}'");
                }
            }
        }

        /// <summary>
        /// Parses a token from the command line (either a flag or an argument for a flag)
        /// </summary>
        /// <param name="token"></param>
        private void ParseToken(Token token)
        {
            switch (token.TokenType)
            {
                case TokenType.Argument:
                    ParseArgument(token.Value);
                    break;
                case TokenType.Flag:
                    ParseFlag(token.Value[0]);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument"></param>
        private void ParseArgument(string argument)
        {
            if(firstToken)
            {
                ParseFirstArgument(argument);
                return;
            }

            if(activeFlags.Count != 0)
            {
                char flag = activeFlags.Dequeue();
                FlagPropertyInfo info = this.propertyFlags[flag];
                PropertyInfo argumentProperty = info.ArgumentInfo;

                SetValue(argumentProperty, argument);
            }
            else if(this.defaultFlagExplicitlyUsed == false && this.defaultOptionProperty != null)
            {
                this.defaultFlagImplicitlyUsed = true;
                SetValue(this.defaultOptionProperty, argument);
            }
            else
            {
                throw new ArgumentException($"No option found for argument '{argument}'");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument"></param>
        private void ParseFirstArgument(string argument)
        {
            if(this.assemblyNameProperty != null)
            {
                SetValue(this.assemblyNameProperty, argument);
            }

            if(this.assemblyNameProperties.TryGetValue(argument, out PropertyInfo property))
            {
                SetValue(property, true);

                if(this.flagPropertyNames.TryGetValue(property.Name, out char flag))
                {
                    implicitlyUsedFlags[flag] = true;
                    this.activeFlags.Enqueue(flag);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="argument"></param>
        private void SetValue(PropertyInfo property, string argument)
        {
            Type propertyType = GetBaseType(property.PropertyType);
            property.SetValue(this.option, ParseType(propertyType, argument));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="argument"></param>
        private void SetValue(PropertyInfo property, bool argument)
        {
            Type propertyType = GetBaseType(property.PropertyType);
            property.SetValue(this.option, argument);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        private void ParseFlag(char flag)
        {
            if (this.usedFlags.ContainsKey(flag))
                throw new InvalidOperationException($"Cannot use the same flag twice. Offending flag: {flag}");

            this.usedFlags[flag] = true;
            
            if(flag == this.defaultFlag)
            {
                this.defaultFlagExplicitlyUsed = true;
            }

            FlagPropertyInfo info = this.propertyFlags[flag];
            PropertyInfo flagProperty = info.FlagInfo;

            Type propertyType = GetBaseType(flagProperty.PropertyType);

            if (propertyType == BoolType)
            {
                SetValue(flagProperty, true);

                if (info.ArgumentInfo != null)
                    activeFlags.Enqueue(flag);
            }
            else
            {
                activeFlags.Enqueue(flag);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PropertyOptions
    {
        public Type PropertyType { get; set; }
        public char? Flag { get; set; } = null;
        public List<char> FlagArguments { get; set; } = new List<char>();
        public bool IsDefault { get; set; } = false;
        public bool IsRequired { get; set; } = false;
        public char[] RequiresOne { get; set; } = null;
        public char[] RequiresAll { get; set; } = null;
        public List<string> Groups { get; set; } = new List<string>();
        public string Description { get; set; } = String.Empty;
        public string AssemblyName { get; set; } = String.Empty;
        public bool IsAssemblyNameProperty { get; set; } = false;
    }

    public class FlagPropertyInfo
    {
        public PropertyInfo FlagInfo { get; set; }

        public PropertyInfo ArgumentInfo { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OptionGroupRange
    {
        /// <summary>
        /// The minimum number of concurrent switches in this group that is allowed. Defaults
        /// to zero.
        /// </summary>
        public int MinimumConcurrent { get; set; } = 0;

        /// <summary>
        /// The maximum number of concurrent switches in this group that is allowed. Defaults
        /// to null.
        /// </summary>
        public int? MaximumConcurrent { get; set; } = null;
    }
}
