using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System;

public class DecoderMgr : Singleton<DecoderMgr>
{
    /// <summary>
    /// 语法格式
    /// </summary>
    public enum LineSyntax
    {
        UnDefine = 0,
        InstanceCallMethod,
        Constructor,
    }

    /// <summary>
    /// 节点类
    /// </summary>
    public class Node
    {
        /// <summary>
        /// 行为树在导出代码版本的时候已经丢失Agent的类型，通过节点取出的AgentType可能为父类，无法正确取得准确的Agent，所以这里会把所有的方法，属性全部放到一个新的总的Agent类里去
        /// 这意味着还原后的workspace版本xml只能用来学习，无法再一次导出作为游戏内资源驱动行为树
        /// </summary>
        public static string AgentType = "WrapperAgent";
        public Dictionary<string, string> Properties;
        public List<Connector> Connectors;

        public string Name;
    }

    /// <summary>
    /// 连线类
    /// </summary>
    public class Connector
    {
        public string Identifier;
        public List<Node> Nodes;
    }

    public class EnumType
    {
        public class EnumMemberType
        {
            public string Namespace;
            public string NativeValue;
            public string Name;
            public string DisplayName;
            public string Description;
            public int Value;
        }
        public bool isCustomized;
        public bool isImplemented;
        public string enumName;
        public string Namespace;
        public string ExportLocation;
        public string DisplayName;
        public string Description;
        public List<EnumMemberType> Members = new List<EnumMemberType>();
        public string Fullname
        {
            get
            {
                return string.IsNullOrEmpty(Namespace) ? enumName : (Namespace + "::" + enumName);
            }
        }
    }

    public class StructType
    {
        public bool isRef;
        public bool isCustomized;
        public bool isImplemented;
        public string structName;
        public string Namespace;
        public string BaseName;
        public string ExportLocation;
        public string DisplayName;
        public string Description;
        public List<PropertyDef> Properties = new List<PropertyDef>();
        public string Fullname
        {
            get
            {
                return string.IsNullOrEmpty(Namespace) ? structName : (Namespace + "::" + structName);
            }
        }
    }

    public class PropertyDef
    {
        public AgentType AgentType;
        public string Name;
        public string BasicName;
        public string DisplayName;
        public string BasicDescription;
        public string ClassName;
        public string NativeType;
        public System.Type Type;
        public bool IsCustomized;
        public string DefaultValue;
        public bool IsStatic;
        public bool IsPublic;
        public bool IsReadonly;
        public VariableDef Variable;

        private uint id = 0;
        public uint ID
        {
            get
            {
                if (id == 0)
                {
                    id = CRC32.CalcCRC(BasicName.Replace("[]", ""));
                }
                return id;
            }
        }
    }

    public enum MemberType
    {
        Property = 0,
        Method,
        Task
    }

    public class AgentType
    {
        public string Name;
        public AgentType Base;
        public string OldName;
        public string DisplayName;
        public string Description;
        public bool IsStatic;
        public bool IsCustomized;
        public bool IsImplemented;
        public string ExportLocation;
        public List<PropertyDef> Properties = new List<PropertyDef>();
        public List<MethodDef> Methods = new List<MethodDef>();
    }

    public class MethodDef
    {
        public class Param
        {
            public string Name;
            public string NativeType;
            public System.Type Type;
            public bool IsOut;
            public bool IsRef;
            public bool IsConst;
            public string DisplayName;
            public string Description;
        }
        public AgentType AgentType;
        public string BasicName
        {
            get
            {
                if (!string.IsNullOrEmpty(_name))
                {
                    int index = _name.LastIndexOf(':');
                    if (index >= 0)
                    {
                        return _name.Substring(index + 1);
                    }
                }

                return _name;
            }
        }
        public string DisplayName;
        public string BasicDescription;
        public string ClassName;
        public string NativeReturnType;
        public System.Type ReturnType;
        public bool IsStatic;
        public bool IsPublic;
        private string _name = "";
        public string Name
        {
            get
            {
                // full name
                return _name.Contains(":") ? _name : (this.ClassName + "::" + _name);
            }
            set
            {
                // full name
                _name = value.Contains(":") ? value : (this.ClassName + "::" + value);
            }
        }
        public MemberType MemberType;
        public List<Param> Params = new List<Param>();
    }

    public class VariableDef
    {
        public string ValueClass;
        public System.Object Value;
    }


    List<BehaviorNodeDecoder> Decoders = new List<BehaviorNodeDecoder>();

    List<XmlNode> _agentsXMLNodes = new List<XmlNode>();
    List<XmlNode> _typesXMLNodes = new List<XmlNode>();
    //List<XmlNode> _instancesXMLNodes = new List<XmlNode>();

    List<EnumType> _enumTypes = new List<EnumType>();
    List<StructType> _stuctTypes = new List<StructType>();
    //List<AgentType> _agentTypes = new List<AgentType>();
    Dictionary<string, AgentType> _agentTypes = new Dictionary<string, AgentType>();

    public override void Init()
    {
        base.Init();
        Decoders.Add(new BehaviorTreeDecoder());
        Decoders.Add(new ActionDecoder());
        Decoders.Add(new SequenceDecoder());
        Decoders.Add(new AssignmentDecoder());
        Decoders.Add(new DecoratorLoopDecoder());
        Decoders.Add(new SelectorLoopDecoder());
        Decoders.Add(new WithPreconditionDecoder());
        Decoders.Add(new ConditionDecoder());
        Decoders.Add(new IfElseDecoder());
        Decoders.Add(new NoopDecoder());
        Decoders.Add(new ParallelDecoder());
        Decoders.Add(new TrueDecoder());
        Decoders.Add(new DecoratorAlwaysFailureDecoder());
        Decoders.Add(new ComputeDecoder());
        Decoders.Add(new FalseDecoder());
        Decoders.Add(new SelectorDecoder());
        Decoders.Add(new DecoratorLoopUntilDecoder());
        Decoders.Add(new WaitFramesDecoder());
        Decoders.Add(new DecoratorNotDecoder());
        Decoders.Add(new DecoratorAlwaysSuccessDecoder());
        Decoders.Add(new SelectorProbabilityDecoder());
        Decoders.Add(new DecoratorWeightDecoder());
        Decoders.Add(new OrDecoder());
        Decoders.Add(new SelectorStochasticDecoder());
        Decoders.Add(new AndDecoder());

        //加载meta
        preLoadMeta();
    }

    public override void UnInit()
    {
        base.UnInit();
        Decoders.Clear();
        _agentsXMLNodes.Clear();
        _typesXMLNodes.Clear();
        //_instancesXMLNodes.Clear();
        _enumTypes.Clear();
        _stuctTypes.Clear();
    }

    public AgentType TryGetWrapperAgent()
    {
        if (_agentTypes.ContainsKey(Node.AgentType))
        {
            return _agentTypes[Node.AgentType];
        }

        string agentBase = "behaviac::Agent";
        string oldName = "";
        string agentDisp = Node.AgentType;
        AgentType agent = new AgentType();
        agent.IsCustomized = true;
        agent.Name = Node.AgentType;
        agent.OldName = oldName;
        agent.Base = null;
        agent.DisplayName = agentDisp;
        _agentTypes[agent.Name] = agent;

        return agent;
    }

    public static bool isSupportedClass(string typename)
    {
        switch (typename)
        {
            case "bool":
            case "int":
            case "uint":
            case "float":
            case "double":
            case "string":
            case "short":
            case "ushort":
            case "sbyte":
            case "ubyte":
            case "long":
            case "llong":
            case "ulong":
            case "ullong":
            case "char":
            case "behaviac.EBTStatus":
            case "behaviac.Agent":
            case "EBTStatus":
            case "Agent":
            case "void":
                return true;
            default:
                Debug.LogWarning(typename + "非Behaviac默认类型，将转换成String处理");
                return false;
        }
    }

    public static string FixPropertyString(string type, string value)
    {
        switch (type)
        {
            case "float":
                return value.Replace("f", "");
            case "uint":
            case "ushort":
            case "ulong":
            case "ullong":
                return value.Replace("u", "");
            default:
                return value;
        }
    }

    public static string FixPropertyString(string value)
    {
        string tmp = value;
        if (Regex.IsMatch(value, @"[0-9]{1,}\.*[0-9]*f"))
        {
            return value.Replace("f", "");
        }
        if (Regex.IsMatch(value, @"[0-9]{1,}uL"))
        {
            return value.Replace("uL", "");
        }
        if (Regex.IsMatch(value, @"[0-9]{1,}u"))
        {
            return value.Replace("u", "");
        }
        return value;
    }

    public static string ParseArgsString(string filePath, string argsstr)
    {
        if (string.IsNullOrEmpty(argsstr))
        {
            return "";
        }
        //argsstr格式为   ("Idle",0.1,0,false) 这样，规定下
        argsstr = argsstr.Substring(1, argsstr.Length - 2);
        argsstr = argsstr.Replace(" ", "");
        string[] args = argsstr.Split(',');
        if (args != null && args.Length > 0)
        {
            for (int i = 0, imax = args.Length; i < imax; i++)
            {
                if (string.IsNullOrEmpty(args[i]))
                {
                    continue;
                }
                if (args[i].StartsWith("\"") && args[i].EndsWith("\""))
                {
                    //字符串
                }
                else
                {
                    float tmp;
                    if (float.TryParse(args[i], out tmp))
                    {
                        //数字类型
                        //把C#浮点中的f给去了或者无符号的的u给去了
                        args[i] = DecoderMgr.FixPropertyString(args[i]);
                    }
                    else
                    {
                        //C# 的 数据类型后缀，这里要把最后一位强行删除后再判断下
                        if (float.TryParse(args[i].Substring(0, args[i].Length - 1), out tmp))
                        {
                            //数字类型
                            //把C#浮点中的f给去了或者无符号的的u给去了
                            args[i] = DecoderMgr.FixPropertyString(args[i]);
                        }
                        else
                        {
                            string argline = "";
                           
                            if (DecoderMgr.SearchLine(filePath, args[i].Replace(".", @"\.") + @".*\=.*;", out argline))
                            {
                                //这里behaviac不支持把一个方法的返回值作为另外一个方法的参数，所以只用判断 参数是一个常量还是一个Agent的属性即可
                                Match m = DecoderMgr.FindFirstMatch(argline, @"\..*\(.*\)\;");
                                if (m != null && m.Length > 0)
                                {
                                    string functionname = DecoderMgr.FindFirstMatch(argline, @"\..*\(").Value;
                                    functionname = functionname.Substring(1, functionname.Length - 2);
                                    if (functionname == "GetVariable")
                                    {
                                        //这里赋值的右边是取得agent的某个属性
                                        string uid = DecoderMgr.FindLastMatch(argline, functionname + @"\(.*?\)").Value;
                                        uid = uid.Substring(uid.IndexOf("(") + 1, uid.LastIndexOf(")") - uid.IndexOf("(") - 2);//多减少一位是因为最后还有一位字母u
                                        string pname = "";
                                        if (Singleton<DecoderMgr>.GetInstance().tryGetVariable(filePath, uid, out pname))
                                        {
                                            string type = DecoderMgr.FindFirstMatch(argline, @"[\S]* ").Value.Replace(" ", "");
                                            if (!isSupportedClass(type))
                                            {
                                                type = "string";
                                            }
                                            //node.Properties["Opr"] = string.Format("{0} Self.{1}::{2}", type, DecoderMgr.Node.AgentType, pname);
                                            args[i] = string.Format("{0} Self.{1}::{2}", type, DecoderMgr.Node.AgentType, pname);
                                            //args[i] = string.Format("Self.{0}::{1}", DecoderMgr.Node.AgentType, pname);

                                        }
                                        else
                                        {
                                            Debug.LogError("没有通过UID : " + uid + " 找到对应的属性名字");
                                        }
                                    }
                                    else
                                    {
                                        Debug.LogError("理论是不可能走到这里的!");
                                    }
                                }
                                else
                                {
                                    //到这里应该是常量类型赋值 
                                    string type = DecoderMgr.FindFirstMatch(argline, @"[\S]* ").Value.Replace(" ", "");
                                    if (Regex.IsMatch(argline, " " + args[i] + " "))
                                    {
                                        //格式int a = ... 这样能直接取到类型
                                        //方法就能取好 type
                                    }
                                    else
                                    {
                                        //否则就是这样的格式，十分恶心,还得全文搜索一次
                                        //int a;
                                        //a = 1;
                                        string typeline;
                                        if (DecoderMgr.SearchLine(filePath, args[i].Replace("this.", "") + @"\;", out typeline))
                                        {
                                            //type = DecoderMgr.FindFirstMatch(typeline, @"[\S]* ").Value.Replace(" ", "");
                                            type = typeline.Substring(0, typeline.LastIndexOf(args[i].Replace("this.", "")));
                                            type = type.Replace("private", "");
                                            type = type.Replace(" ", "");
                                            type = type.Replace("\t", "");
                                        }
                                        else
                                        {
                                            Debug.LogError(filePath + "未能找到" + args[i] + "的类型，请检查" + " regex |" + args[i] + @"\;" + "|");
                                        }
                                    }
                                    if (!DecoderMgr.isSupportedClass(type))
                                    {
                                        type = "string";
                                    }
                                    string value = DecoderMgr.FindLastMatch(argline, @" [\S]*\;").Value.Replace(";", "").Replace(" ", "");
                                    value = DecoderMgr.FixPropertyString(type, value);
                                    args[i] = /*"const " + type + " " + */value;
                                }
                            }
                            else
                            {
                                Debug.LogError("文件:" + filePath + " 未能找到属性 " + args[i] + "正则:" + args[i].Replace(".", @"\.") + @".*\=.*;");
                            }
                        }
                    }
                }
            }
            argsstr = "";
            for (int i = 0, imax = args.Length; i < imax; i++)
            {
                argsstr += args[i];
                if (i != imax - 1)
                {
                    argsstr += ",";
                }
            }
        }

        return argsstr;
    }

    public static string ParseOperator(string op)
    {
        switch (op)
        {
            case "+":
                return "Add";
            case "-":
                return "Sub";
            case "*":
                return "Mul";
            case "/":
                return "Div";
            default:
                return op;
        }
    }

    /// <summary>
    /// 给定一个变量名，在指定文件内找到这个变量的赋值，并按照behaviac可识别的格式返回
    /// int a = 0               返回 "const int 0"
    /// int a = agent.p_id;     返回 "int self.WrapperAgent::p_id"
    /// int a = agemt.Func(agent.p_id);   返回 "self.WrapperAgent::Func(self.WrapperAgent::p_id)"
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="VariableName"></param>
    /// <returns></returns>
    public static string FindVariableAndParse(string filePath, string VariableName, bool needconst = false)
    {
        string line;
        if (SearchLine(filePath, VariableName.Replace("this.", "") + @" \=", out line))
        {
            string type = line.Substring(0, line.IndexOf(" "));
            type = type.Replace(" ", "");
            type = type.Replace("\t", "");


            string value = line.Substring(line.LastIndexOf(" ") + 1, line.Length - line.LastIndexOf(" ") - 2);
            value = value.Replace(" ", "");

            Match m = DecoderMgr.FindFirstMatch(line, @"\..*\(.*\)\;");
            if (m != null && m.Length > 0)
            {
                string functionname = DecoderMgr.FindFirstMatch(line, @"\.[^()]*\(").Value;
                functionname = functionname.Substring(1, functionname.Length - 2);
                if (functionname == "GetVariable")
                {
                    string uid = DecoderMgr.FindLastMatch(line, functionname + @"\(.*?\)").Value;
                    uid = uid.Substring(uid.IndexOf("(") + 1, uid.LastIndexOf(")") - uid.IndexOf("(") - 2);//多减少一位是因为最后还有一位字母u
                    string pname = "";
                    if (Singleton<DecoderMgr>.GetInstance().tryGetVariable(filePath, uid, out pname))
                    {
                        if (!DecoderMgr.isSupportedClass(type))
                        {
                            type = "string";
                        }
                        else if (isEBTStatus(type))
                        {
                            if (!type.StartsWith("behaviac::"))
                            {
                                type = "behaviac::" + type;
                            }
                        }
                        return string.Format("{0} Self.{1}::{2}", type, DecoderMgr.Node.AgentType, pname);
                    }
                    else
                    {
                        Debug.LogError("没有通过UID : " + uid + " 找到对应的属性名字");
                    }
                }
                else
                {
                    Singleton<DecoderMgr>.GetInstance().AddMethod("ObjAgent", functionname);
                    string argsstr = DecoderMgr.FindLastMatch(line, @"\([^()]*\)").Value;
                    return string.Format("Self.{0}::{1}({2})", DecoderMgr.Node.AgentType, functionname, DecoderMgr.ParseArgsString(filePath, argsstr));
                }
            }
            else
            {
                if (!isSupportedClass(type))
                {
                    type = "string";
                    value = "\"" + value + "\"";
                }



                if (isNum(value))
                {
                    return !needconst ? FixPropertyString(value) : string.Format("const {0} {1}", type, FixPropertyString(value));
                }
                else if (isString(value))
                {
                    //return 
                    return !needconst ? value : string.Format("const {0} {1}", type, value);
                }
                else if (isBool(value))
                {
                    return !needconst ? value : string.Format("const {0} {1}", type, value);
                }
                else if (isEBTStatus(value))
                {
                    if (!type.StartsWith("behaviac::"))
                    {
                        type = "behaviac::" + type;
                    }
                    if (value.StartsWith("EBTStatus."))
                    {
                        value = value.Substring(value.IndexOf(".") + 1);
                    }
                    return !needconst ? value : string.Format("const {0} {1}", type, value);
                }
                else
                {
                    Debug.LogError("理论不应该走到这里 value : " + value + " type :" + type);
                }
            }
        }
        else
        {
            Debug.LogError(filePath + "无法找到" + VariableName + "的初始化方法");
        }
       
        return VariableName;
    }

    public static bool FindOperator(string line, out string op, out string opinxml)
    {
        op = "";
        opinxml = "";

        Match m = FindFirstMatch(line, @"&&");
        if (m != null && m.Length > 0)
        {
            op = "&&";
            opinxml = "And"; 
            return true;
        }

        m = FindFirstMatch(line, @"||");
        if (m != null && m.Length > 0)
        {
            op = "||";
            opinxml = "Or";
            return true;
        }

        m = FindFirstMatch(line, @"==");
        if (m != null && m.Length > 0)
        {
            op = "==";
            opinxml = "Equal";
            return true;
        }

        m = FindFirstMatch(line, @"!=");
        if (m != null && m.Length > 0)
        {
            op = "!=";
            opinxml = "NotEqual";
            return true;
        }

        m = FindFirstMatch(line, @">=");
        if (m != null && m.Length > 0)
        {
            op = ">=";
            opinxml = "GreaterEqual";
            return true;
        }

        m = FindFirstMatch(line, @"<=");
        if (m != null && m.Length > 0)
        {
            op = "<=";
            opinxml = "LessEqual";
            return true;
        }

        m = FindFirstMatch(line, @"=");
        if (m != null && m.Length > 0)
        {
            op = "=";
            opinxml = "Assignment";
            return true;
        }

        m = FindFirstMatch(line, @">");
        if (m != null && m.Length > 0)
        {
            op = ">";
            opinxml = "Greater";
            return true;
        }

        m = FindFirstMatch(line, @"<");
        if (m != null && m.Length > 0)
        {
            op = "<";
            opinxml = "Less";
            return true;
        }
        return false;
    }

    public static bool isString(string str)
    {
        return str.StartsWith("\"") && str.EndsWith("\"");
    }

    public static bool isNum(string str)
    {
        float tmp;
        if (float.TryParse(str, out tmp))
        {
            return true;
        }
        else
        {
            //C# 的 数据类型后缀，这里要把最后一位强行删除后再判断下
            if (float.TryParse(str.Substring(0, str.Length - 1), out tmp))
            {
                return true;
            }
            else
            {
                //居然还有 1000uL 这种格式。。
                if (float.TryParse(str.Substring(0, str.Length - 2), out tmp))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool isBool(string str)
    {
        bool tmp;
        return bool.TryParse(str, out tmp);
    }

    public static bool isEBTStatus(string str)
    {
        return str.Contains("EBTStatus");
    }

    public bool tryGetVariable(string FilePath, string uid, out string paramName)
    {
        paramName = "";
        bool find = false;
        foreach (var agent in _agentTypes.Values)
        {
            foreach (var property in agent.Properties)
            {
                if (property.ID.ToString() == uid)
                {
                    paramName = property.BasicName;
                    find = true;
                    break;
                }
            }
        }
        //这里如果找不到可以考虑搜索同路径，但这个操作要遍历，所以比较慢
        if (!find)
        {
            string dir = Path.GetDirectoryName(FilePath);
            DirectoryInfo di = new DirectoryInfo(dir);
            FileInfo[] files = di.GetFiles();
            if (files != null && files.Length > 0)
            {
                for (int i = 0, imax = files.Length; i < imax; i++)
                {
                    string line;
                    if (SearchLine(files[i].FullName, "SetVariable" + @".*" + uid, out line))
                    {
                        string argstr = FindFirstMatch(line, @"\(.*\)").Value;
                        argstr = argstr.Substring(1, argstr.Length - 2);
                        string[] args = argstr.Split(',');
                        string argname = args[0].Replace(" ", "");
                        argname = argname.Replace("\"", "");

                        string type = FindFirstMatch(line, @"\<.*\>").Value;
                        type = type.Substring(1, type.Length - 2);

                        AddProperty(type, argname, "");

                        paramName = argname;

                        return true;
                    }
                }
            }
        }


        return find;
    }

    public void AddProperty(string type, string name, string value)
    {
        AgentType wrapperAgent = TryGetWrapperAgent();

        PropertyDef property = wrapperAgent.Properties.Find(x => x.BasicName == name);

        if (property != null)
        {
            property.DefaultValue = value;
        }
        else
        {
            //之前没有存储过这个
            //先判断下类型是否常规类型，非常规类型，一律按照string存储
            if (!isSupportedClass(type))
            {
                type = "string";
            }

            property = new PropertyDef();
            property.AgentType = wrapperAgent;
            property.NativeType = GetNativeTypeName(GetTypeFromName(type));
            property.Type = GetTypeFromName(type);
            property.ClassName = Node.AgentType;
            property.BasicName = name;
            property.DefaultValue = value;

            property.Variable = new VariableDef();
            //property.Variable.Value = value;
            

            wrapperAgent.Properties.Add(property);
        }

        //string propName = GetAttribute(propNode, "name");

        //if (string.IsNullOrEmpty(propName))
        //{
        //    propName = GetAttribute(propNode, "Name");
        //}

        //try
        //{
        //    string isStatic = GetAttribute(propNode, "static");

        //    if (string.IsNullOrEmpty(isStatic))
        //    {
        //        isStatic = GetAttribute(propNode, "Static");
        //    }

        //    bool bStatic = (!string.IsNullOrEmpty(isStatic) && isStatic == "true");

        //    string isPublic = GetAttribute(propNode, "public");

        //    if (string.IsNullOrEmpty(isPublic))
        //    {
        //        isPublic = GetAttribute(propNode, "Public");
        //    }

        //    bool bPublic = (string.IsNullOrEmpty(isPublic) || isPublic == "true");

        //    string isReadonly = GetAttribute(propNode, "readonly");

        //    if (string.IsNullOrEmpty(isReadonly))
        //    {
        //        isReadonly = GetAttribute(propNode, "Readonly");
        //    }

        //    bool bReadonly = (!string.IsNullOrEmpty(isReadonly) && isReadonly == "true");

        //    string propType = GetAttribute(propNode, "type");

        //    if (string.IsNullOrEmpty(propType))
        //    {
        //        propType = GetAttribute(propNode, "TypeFullName");
        //    }

        //    Type type = GetTypeFromName(propType);

        //    string classname = GetAttribute(propNode, "classname");

        //    if (string.IsNullOrEmpty(classname))
        //    {
        //        classname = GetAttribute(propNode, "Class");
        //    }

        //    if (string.IsNullOrEmpty(classname))
        //    {
        //        classname = agent.Name;
        //    }

        //    string propDisp = GetAttribute(propNode, "disp");

        //    if (string.IsNullOrEmpty(propDisp))
        //    {
        //        propDisp = GetAttribute(propNode, "DisplayName");
        //    }

        //    if (string.IsNullOrEmpty(propDisp))
        //    {
        //        propDisp = propName;
        //    }

        //    string propDesc = GetAttribute(propNode, "desc");

        //    if (string.IsNullOrEmpty(propDesc))
        //    {
        //        propDesc = GetAttribute(propNode, "Desc");
        //    }

        //    //PropertyDef prop = new PropertyDef(agent, type, classname, propName, propDisp, propDesc);
        //    PropertyDef prop = new PropertyDef();
        //    prop.AgentType = agent;
        //    prop.Type = type;
        //    prop.ClassName = classname;
        //    prop.Name = propName;
        //    prop.DisplayName = propDisp;
        //    prop.BasicDescription = propDesc;
        //    prop.IsStatic = bStatic;
        //    prop.IsPublic = bPublic;
        //    prop.IsReadonly = bReadonly;

        //    prop.Variable = new VariableDef();

        //    agent.Properties.Add(prop);
        //}
        //catch (Exception)
        //{
        //    Debug.LogError(string.Format("error when loading Agent '{0}' Member '{1}'", agent.Name, propName));
        //    //MessageBox.Show(errorInfo, "Loading Custom Meta", MessageBoxButtons.OK);
        //}

    }

    public void AddMethod( string OriAgentName, string functionname)
    {
        string filePath = Application.dataPath + "/Scripts/GameLogic/" + OriAgentName + ".cs";
        string baseAgentFilePath = Application.dataPath + "/Scripts/GameLogic/" + "BTBaseAgent" + ".cs";
        if (File.Exists(filePath))
        {
            string line;
            if (SearchLine(filePath, " " + functionname + @"\(.*\)", out line))
            {
                //string returnType = line.Split(' ')[1];
                string[] front = line.Substring(0, line.IndexOf(functionname) - 1).Split(' ');
                string returnType = front[front.Length - 1];
                //returnType = returnType.Substring(1, returnType.Length - 2);
                Match m = FindLastMatch(line, @"\(.*\)");
                string argstr = m.Value.Substring(1, m.Value.Length - 2);

                string[] argnames = null;
                string[] argtypes = null;
                if (!string.IsNullOrEmpty(argstr))
                {
                    string[] args = argstr.Split(',');
                    if (args != null && args.Length > 0)
                    {
                        argnames = new string[args.Length];
                        argtypes = new string[args.Length];
                        for (int i = 0; i < args.Length; i++)
                        {
                            argtypes[i] = FindFirstMatch(args[i], @"[\S]{1,} ").Value.Replace(" ", "");
                            argnames[i] = FindLastMatch(args[i], @" [\S]{1,}").Value.Replace(" ", "");
                        }
                    }
                }

                AddMethod(returnType, functionname, argnames, argtypes);
            }
            else
            {
                if (File.Exists(baseAgentFilePath))
                {
                    if (SearchLine(baseAgentFilePath, " " + functionname + @"\(.*\)", out line))
                    {
                        string returnType = line.Split(' ')[1];
                        Match m = FindLastMatch(line, @"\(.*\)");
                        string argstr = m.Value.Substring(1, m.Value.Length - 2);

                        string[] argnames = null;
                        string[] argtypes = null;
                        if (!string.IsNullOrEmpty(argstr))
                        {
                            string[] args = argstr.Split(',');
                            if (args != null && args.Length > 0)
                            {
                                argnames = new string[args.Length];
                                argtypes = new string[args.Length];
                                for (int i = 0; i < args.Length; i++)
                                {
                                    argtypes[i] = FindFirstMatch(args[i], @"[\S]{1,} ").Value.Replace(" ", "");
                                    argnames[i] = FindLastMatch(args[i], @" [\S]{1,}").Value.Replace(" ", "");
                                }
                            }
                        }


                        AddMethod(returnType, functionname, argnames, argtypes);
                    }
                    else
                    {
                        Debug.LogError(filePath + " 中未能找到 : " + functionname);
                    }
                }
                else
                {
                    Debug.LogError("未能找到 : " + OriAgentName + " 对应的解析文件: " + baseAgentFilePath);
                }
            }
        }
        else
        {
            Debug.LogError("未能找到 : " + OriAgentName + " 对应的解析文件: " + filePath);
        }
    }

    public void AddMethod(string returnTypeName, string methodname, string[] argnames, string[] argtypes)
    {

        if (!isSupportedClass(returnTypeName))
        {
            returnTypeName = "string";
        }

        AgentType wrapperAgent = TryGetWrapperAgent();

        MethodDef method = wrapperAgent.Methods.Find(x => x.BasicName == methodname);
        if (method != null)
        {

        }
        else
        {
            string methodName = methodname;

            try
            {
                string returnTypename = returnTypeName;

                Type returnType = GetTypeFromName(returnTypename);

                string isStatic = "false";

                bool bStatic = (!string.IsNullOrEmpty(isStatic) && isStatic == "true");

                string isPublic = "true";

                bool bPublic = (string.IsNullOrEmpty(isPublic) || isPublic == "true");

                string classname = wrapperAgent.Name;

               

                methodName = string.Format("{0}::{1}", wrapperAgent.Name, methodName);

                //MethodDef method = new MethodDef(agent, memberType, classname, methodName, methodDisp, methodDesc, "", returnType);
                method = new MethodDef();
                method.AgentType = wrapperAgent;
                method.MemberType = MemberType.Method;
                method.Name = methodName;
                method.ClassName = classname;

                method.NativeReturnType = GetNativeTypeName(returnType);
                method.ReturnType = returnType;
                method.IsStatic = bStatic;
                method.IsPublic = bPublic;

                //agent.AddMethod(method);
                wrapperAgent.Methods.Add(method);

                if (argnames != null && argtypes != null)
                {
                    for (int i = 0; i < argnames.Length && i < argtypes.Length; i ++)
                    {
                        string paramName = argnames[i];

                        string paramTypename = argtypes[i];

                        Type paramType = GetTypeFromName(paramTypename);

                        if (paramType == null)
                        {
                            paramType = typeof(string);
                        }


                        string nativeType = GetNativeTypeName(paramType);

                        //MethodDef.Param param = new MethodDef.Param(paramName, paramType, nativeType, paramDisp, paramDesc);
                        MethodDef.Param param = new MethodDef.Param();
                        param.Name = paramName;
                        param.Type = paramType;
                        param.NativeType = nativeType;


                        method.Params.Add(param);
                    }
                }
            }
            catch (Exception)
            {
                Debug.LogError(string.Format("error when loading Agent '{0}' Method '{1}'", wrapperAgent.Name, methodName));
                //MessageBox.Show(errorInfo, "Loading Custom Meta", MessageBoxButtons.OK);
            }
        }
    }

    //static string metaPath = Application.dataPath + "/meta.xml";
    private void preLoadMeta()
    {
        XmlDocument _metaFile = new XmlDocument();
        //生成默认配置
        _metaFile.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
<meta>
  <types />
  <agents>
    <agent classfullname=""behaviac::Agent"" DisplayName=""behaviac::Agent"" Desc="""" IsRefType=""true"" IsImplemented=""true"">
      <Method Name=""VectorLength"" DisplayName=""VectorLength"" Desc=""VectorLength"" Class=""behaviac::Agent"" ReturnType=""int"" ReturnTypeFullName=""System.Int32"" Static=""true"" Public=""true"" istask=""false"">
        <Param Name=""param0"" Type=""const IList&amp;"" TypeFullName=""System.Collections.IList"" DisplayName=""param0"" Desc=""param0"" />
      </Method>
      <Method Name=""VectorAdd"" DisplayName=""VectorAdd"" Desc=""VectorAdd"" Class=""behaviac::Agent"" ReturnType=""void"" ReturnTypeFullName=""System.Void"" Static=""true"" Public=""true"" istask=""false"">
        <Param Name=""param0"" Type=""IList&amp;"" TypeFullName=""System.Collections.IList"" DisplayName=""param0"" Desc=""param0"" />
        <Param Name=""param1"" Type=""const System::Object&amp;"" TypeFullName=""System.Object"" DisplayName=""param1"" Desc=""param1"" />
      </Method>
      <Method Name=""VectorRemove"" DisplayName=""VectorRemove"" Desc=""VectorRemove"" Class=""behaviac::Agent"" ReturnType=""void"" ReturnTypeFullName=""System.Void"" Static=""true"" Public=""true"" istask=""false"">
        <Param Name=""param0"" Type=""IList&amp;"" TypeFullName=""System.Collections.IList"" DisplayName=""param0"" Desc=""param0"" />
        <Param Name=""param1"" Type=""const System::Object&amp;"" TypeFullName=""System.Object"" DisplayName=""param1"" Desc=""param1"" />
      </Method>
      <Method Name=""VectorContains"" DisplayName=""VectorContains"" Desc=""VectorContains"" Class=""behaviac::Agent"" ReturnType=""bool"" ReturnTypeFullName=""System.Boolean"" Static=""true"" Public=""true"" istask=""false"">
        <Param Name=""param0"" Type=""IList&amp;"" TypeFullName=""System.Collections.IList"" DisplayName=""param0"" Desc=""param0"" />
        <Param Name=""param1"" Type=""const System::Object&amp;"" TypeFullName=""System.Object"" DisplayName=""param1"" Desc=""param1"" />
      </Method>
      <Method Name=""VectorClear"" DisplayName=""VectorClear"" Desc=""VectorClear"" Class=""behaviac::Agent"" ReturnType=""void"" ReturnTypeFullName=""System.Void"" Static=""true"" Public=""true"" istask=""false"">
        <Param Name=""param0"" Type=""IList&amp;"" TypeFullName=""System.Collections.IList"" DisplayName=""param0"" Desc=""param0"" />
      </Method>
      <Method Name=""LogMessage"" DisplayName=""LogMessage"" Desc=""LogMessage"" Class=""behaviac::Agent"" ReturnType=""void"" ReturnTypeFullName=""System.Void"" Static=""true"" Public=""true"" istask=""false"">
        <Param Name=""param0"" Type=""const char*"" TypeFullName=""System.String"" DisplayName=""param0"" Desc=""param0"" />
      </Method>
    </agent>
  </agents>
  <instances />
</meta>");

        for (int i = 1; i < _metaFile.ChildNodes.Count; ++i)
        {
            XmlNode root = _metaFile.ChildNodes[i];

            if (root.Name == "meta")
            {
                foreach (XmlNode xmlNode in root.ChildNodes)
                {
                    if (xmlNode.Name == "agents")
                    {
                        _agentsXMLNodes.Add(xmlNode);
                    }
                    else if (xmlNode.Name == "types")
                    {
                        _typesXMLNodes.Add(xmlNode);
                    }
                    else if (xmlNode.Name == "instances")
                    {

                    }
                }
            }
            else if (root.Name == "agents")
            {
                _agentsXMLNodes.Add(root);
            }
        }

        foreach (XmlNode typesXmlNode in _typesXMLNodes)
        {
            LoadTypes(typesXmlNode);
        }

        foreach (XmlNode agentsXmlNode in _agentsXMLNodes)
        {
            LoadAgents( agentsXmlNode);
        }

    }

    /// <summary>
    /// 解析并导出行为树
    /// </summary>
    /// <param name="srcPath"></param>
    /// <param name="savePath"></param>
    /// <param name="ErrorStr"></param>
    /// <returns></returns>
    public bool TryParseAndSaveFile(string srcPath, string savePath, out string ErrorStr)
    {
        ErrorStr = "";

        if (!File.Exists(srcPath))
        {
            ErrorStr = "源文件不存在";
            return false;
        }

        if (!isBehaviourTreeClass(srcPath))
        {
            ErrorStr = "源文件非行为树代码";
            return false;
        }

        Node root = new Node();
        root.Name = "bt";
        root.Properties = Decoders[0].PropertiesTemplate();
        root.Properties["AgentType"] = Node.AgentType;

        ParseTreeFile(srcPath, root);

        savePath = Path.ChangeExtension(savePath, "xml");

        if (string.IsNullOrEmpty(savePath))
        {
            ErrorStr = "存储地址为空";
            return false;
        }

        string saveDirectory = Path.GetDirectoryName(savePath);

        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        #region 写入XML
        XmlDocument xmlfile = new XmlDocument();
        XmlDeclaration declaration = xmlfile.CreateXmlDeclaration("1.0", "utf-8", null);
        xmlfile.AppendChild(declaration);

        XmlElement Behavior = xmlfile.CreateElement("Behavior");
        xmlfile.AppendChild(Behavior);

        Behavior.SetAttribute("Version", "5");
        Behavior.SetAttribute("NoError", "true");

        //AddXmlChild(xmlfile, root, bt);
        //ParseTree(xmlfile, root, bt);
        TranslateBehaviourToXML(root, xmlfile, Behavior);

        xmlfile.Save(savePath);
        #endregion



        return true;
    }

    /// <summary>
    /// 保存Meta文件
    /// </summary>
    /// <param name="SavePath"></param>
    public void SaveMeta(string SavePath)
    {
        #region 保存meta
        string metaDir = Path.GetDirectoryName(SavePath);
        if (!Directory.Exists(metaDir))
        {
            Directory.CreateDirectory(metaDir);
        }
        XmlDocument metaFile = new XmlDocument();
        metaFile.AppendChild(metaFile.CreateXmlDeclaration("1.0", "utf-8", null));

        XmlElement meta = metaFile.CreateElement("meta");
        metaFile.AppendChild(meta);

        SaveTypes(metaFile, meta);
        SaveAgents(metaFile, meta);

        metaFile.Save(SavePath);
        #endregion
    }


    /// <summary>
    /// 判断是否为行为树代码
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public bool isBehaviourTreeClass(string filePath)
    {
        if (File.Exists(filePath))
        {
            StreamReader sr = new StreamReader(filePath);

            string line = sr.ReadLine();

            while (line != null)
            {
                if (Regex.IsMatch(line, @"build_behavior_tree\(BehaviorTree bt\)"))
                {
                    sr.Close();
                    return true;
                }
                line = sr.ReadLine();
            }

            sr.Close();
        }
        return false;
    }

    /// <summary>
    /// 解析行为树
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="root"></param>
    private void ParseTreeFile(string filePath, Node root)
    {
        if (File.Exists(filePath))
        {
            List<Node> nodes = new List<Node>();

            nodes.Add(root);

            StreamReader sr = new StreamReader(filePath);
            string line = sr.ReadLine();

            while (line != null)
            {
                LineSyntax syntax = AnalyzeLine(line);

                switch (syntax)
                {
                    default:
                    case LineSyntax.UnDefine:
                        break;
                    case LineSyntax.Constructor:
                        #region Constructor
                        {
                            string[] strs = line.Split(' ');
                            string classname = strs[0].Replace("\t", "");
                            string instancename = strs[1];
                            Node tmp = new Node();
                            tmp.Name = instancename;

                            //对于classname，纯英文应该是直接继承自行为节点，应该有对应的Decoder
                            //但对于Action等需要重写的行为节点，需要找到对应的文件，来分析继承自哪种行为节点，以便用对应的Decoder来解析
                            //或者可以根据Behaviac类名的生成规律，取第一位应为为对应的行为节点来分析是哪种行为节点,这里采用这种偷懒的方式

                            if (classname.IndexOf("_") > 0)
                            {
                                //派生出来的行为节点
                                classname = classname.Substring(0, classname.IndexOf("_"));
                            }
                            else
                            {
                                //直接就为行为节点
                            }

                            BehaviorNodeDecoder decoder = GetDeocoder(classname);
                            if (decoder != null)
                            {
                                tmp.Properties = decoder.PropertiesTemplate();

                                //Debug.Log("对应的类文件名:" + Path.GetDirectoryName(filePath) + "/" + strs[0].Replace("\t", "") + ".cs");
                                string classfile = Path.GetDirectoryName(filePath) + "/" + strs[0].Replace("\t", "") + ".cs";
                                if (File.Exists(classfile))
                                {
                                    decoder.DecoderClassFile(classfile, tmp);
                                }
                                else
                                {
                                    Debug.LogError(classfile + "找不到，无法为" + instancename + "解析");
                                }
                            }
                            else
                            {
                                Debug.LogError(classname + " 没有对应的Decoder");
                                tmp.Properties = Decoders[0].PropertiesTemplate();
                            }
                            nodes.Add(tmp);
                        }
                        #endregion
                        break;
                    case LineSyntax.InstanceCallMethod:
                        #region InstanceCallMethod
                        {
                            int dotindex = line.IndexOf(".");
                            int bracketindex = line.IndexOf("(");
                            int lastbracketindex = line.LastIndexOf(")");
                            string instancename = line.Substring(0, dotindex - 0);
                            instancename = instancename.Replace("\t", "");
                            instancename = instancename.Replace(" ", "");

                            string methodname = line.Substring(dotindex + 1, bracketindex - dotindex - 1);
                            methodname = methodname.Replace("\t", "");
                            methodname = methodname.Replace(" ", "");

                            string argsname = line.Substring(bracketindex + 1, lastbracketindex - bracketindex - 1);
                            argsname = argsname.Replace("\"", "");

                            Node tmp = nodes.Find(x => x.Name == instancename);

                            if (tmp != null)
                            {
                                //BehaviorNodeDecoder decoder = Decoders.Find(x => tmp.Properties["Class"].EndsWith(x.ClassName()));
                                BehaviorNodeDecoder decoder = GetDeocoder(tmp.Properties["Class"]);
                                if (decoder != null)
                                {
                                    //tmp
                                    //decoder
                                    decoder.Process(nodes, tmp, methodname, argsname);
                                }
                                else
                                {
                                    Debug.LogError(tmp.Properties["Class"] + "没有对应Decoder");
                                }
                            }
                            else
                            {
                                Debug.LogError(instancename + "没有构造");
                            }
                        }
                        #endregion
                        break;
                }

                line = sr.ReadLine();
            }

            sr.Close();
        }
    }


    /// <summary>
    /// 解析行语法
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    public LineSyntax AnalyzeLine(string line)
    {
        if (Regex.IsMatch(line, @".*\..*\(.*\);"))
        {
            return LineSyntax.InstanceCallMethod;
        }

        if (Regex.IsMatch(line, @" new .*\(.*\);"))
        {
            return LineSyntax.Constructor;
        }

        return LineSyntax.UnDefine;
    }

    /// <summary>
    /// 根据类名找到对应的解析类
    /// </summary>
    /// <param name="classname"></param>
    /// <returns></returns>
    public BehaviorNodeDecoder GetDeocoder(string classname)
    {
        return Decoders.Find(x => classname.EndsWith(x.ClassName()));
    }

    /// <summary>
    /// 保存当前已经解析的行为树到XML
    /// </summary>
    /// <param name="root"></param>
    /// <param name="xmlFile"></param>
    /// <param name="Behavior"></param>
    void TranslateBehaviourToXML(Node root, XmlDocument xmlFile, XmlElement Behavior)
    {
        XmlElement elem = xmlFile.CreateElement("Node");
        //property
        foreach (var obj in root.Properties)
        {
            elem.SetAttribute(obj.Key, obj.Value);
        }
        Behavior.AppendChild(elem);

        //comment
        XmlElement comment = xmlFile.CreateElement("Comment");
        comment.SetAttribute("Background", "NoColor");
        comment.SetAttribute("Text", "");
        elem.AppendChild(comment);

        //parameters

        //descriptorRefs
        XmlElement DescriptorRefs = xmlFile.CreateElement("DescriptorRefs");
        DescriptorRefs.SetAttribute("value", "0:");
        elem.AppendChild(DescriptorRefs);

        //attachments

        //children
        foreach (var obj in root.Connectors)
        {
            AppendChild(xmlFile, obj, elem);
        }
    }

    /// <summary>
    /// 给行为树XML增加节点
    /// </summary>
    /// <param name="xmlFile"></param>
    /// <param name="connector"></param>
    /// <param name="parent"></param>
    void AppendChild(XmlDocument xmlFile, Connector connector, XmlElement parent)
    {
        XmlElement elem_connector = xmlFile.CreateElement("Connector");
        elem_connector.SetAttribute("Identifier", connector.Identifier);

        parent.AppendChild(elem_connector);



        foreach (var node in connector.Nodes)
        {
            XmlElement elem = xmlFile.CreateElement("Node");
            //property
            foreach (var properties in node.Properties)
            {
                elem.SetAttribute(properties.Key, properties.Value);
            }
            elem_connector.AppendChild(elem);

            //comment
            XmlElement comment = xmlFile.CreateElement("Comment");
            comment.SetAttribute("Background", "NoColor");
            comment.SetAttribute("Text", "");
            elem.AppendChild(comment);

            //children
            if (node.Connectors != null)
            {
                foreach (var obj in node.Connectors)
                {
                    AppendChild(xmlFile, obj, elem);
                }
            }
        }
    }

    /// <summary>
    /// 根据正则搜索行
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="regex"></param>
    /// <param name="line"></param>
    /// <returns></returns>
    public static bool SearchLine(string filePath, string regex, out string line)
    {
        //line = "";
        //regex.IsMatch()
        using (StreamReader sr = new StreamReader(filePath))
        {
            line = sr.ReadLine();

            while (line != null)
            {
                if (Regex.IsMatch(line, regex))
                {
                    sr.Close();
                    return true;
                }
                line = sr.ReadLine();
            }

            sr.Close();
        }
        return false;
    }

    /// <summary>
    /// 正则搜索
    /// </summary>
    /// <param name="line"></param>
    /// <param name="regex"></param>
    /// <returns></returns>
    public static Match FindFirstMatch(string line, string regex)
    {
        return Regex.Match(line, regex);
    }

    /// <summary>
    /// 正则搜索
    /// </summary>
    /// <param name="line"></param>
    /// <param name="regex"></param>
    /// <returns></returns>
    public static Match FindLastMatch(string line, string regex)
    {
        MatchCollection mc = FindMatches(line, regex);
        return mc.Count > 0 ? mc[mc.Count - 1] : null;
    }

    /// <summary>
    /// 正则搜索
    /// </summary>
    /// <param name="line"></param>
    /// <param name="regex"></param>
    /// <returns></returns>
    public static MatchCollection FindMatches(string line, string regex)
    {
        return Regex.Matches(line, regex);
    }

    #region 从行为树编辑器源码拷贝过来的
    /// <summary>
    /// Retrieves an attribute from a XML node. If the attribute does not exist an exception is thrown.
    /// </summary>
    /// <param name="node">The XML node we want to get the attribute from.</param>
    /// <param name="att">The name of the attribute we want.</param>
    /// <returns>Returns the attributes value. Is always valid.</returns>
    private static string GetAttribute(XmlNode node, string att)
    {
        XmlNode value = node.Attributes.GetNamedItem(att);

        if (value != null && value.NodeType == XmlNodeType.Attribute)
        {
            return value.Value;
        }

        return string.Empty;
    }

    private void LoadTypes(XmlNode rootNode)
    {
        try
        {
            if (rootNode == null)
            {
                return;
            }

            foreach (XmlNode xmlNode in rootNode.ChildNodes)
            {
                if (xmlNode.Name == "enumtype")
                {
                    string enumName = GetAttribute(xmlNode, "Type");
                    //EnumType enumType = TypeManager.Instance.FindEnum(enumName);
                    EnumType enumType = _enumTypes.Find(x => x.Fullname.Replace("::", ".") == enumName.Replace("::", "."));

                    if (enumType != null)
                    {
                        //TypeManager.Instance.Enums.Remove(enumType);
                        _enumTypes.Remove(enumType);
                    }

                    string[] enumNames = enumName.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    enumName = enumNames[enumNames.Length - 1];

                    string isCustomized = GetAttribute(xmlNode, "IsCustomized");
                    string isImplemented = GetAttribute(xmlNode, "IsImplemented");
                    string ns = GetAttribute(xmlNode, "Namespace");
                    string exportLocation = GetAttribute(xmlNode, "ExportLocation");
                    exportLocation = exportLocation.Replace("\\\\", "/");
                    exportLocation = exportLocation.Replace("\\", "/");
                    string displayName = GetAttribute(xmlNode, "DisplayName");
                    string desc = GetAttribute(xmlNode, "Desc");

                    //enumType = new EnumType(isCustomized == "true", isImplemented == "true", enumName, ns, exportLocation, displayName, desc);
                    enumType = new EnumType();
                    enumType.isCustomized = isCustomized == "true";
                    enumType.isImplemented = isCustomized == "true";
                    enumType.enumName = enumName;
                    enumType.Namespace = ns;
                    enumType.ExportLocation = exportLocation;
                    enumType.DisplayName = displayName;
                    enumType.Description = desc;


                    foreach (XmlNode memberNode in xmlNode.ChildNodes)
                    {
                        if (memberNode.Name == "enum")
                        {
                            string memberNativeValue = GetAttribute(memberNode, "NativeValue");
                            string memberName = GetAttribute(memberNode, "Value");
                            string[] memberNames = memberName.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            memberName = memberNames[memberNames.Length - 1];

                            string memberDisplayName = GetAttribute(memberNode, "DisplayName");
                            string memberDesc = GetAttribute(memberNode, "Desc");
                            string memberValue = GetAttribute(memberNode, "MemberValue");

                            EnumType.EnumMemberType enumMember = new EnumType.EnumMemberType();
                            enumMember.NativeValue = memberNativeValue;
                            enumMember.Name = memberName;
                            enumMember.DisplayName = memberDisplayName;
                            enumMember.Description = memberDesc;

                            try
                            {
                                enumMember.Value = int.Parse(memberValue);
                            }
                            catch
                            {
                                enumMember.Value = -1;
                            }

                            enumType.Members.Add(enumMember);
                        }
                    }

                    //TypeManager.Instance.Enums.Add(enumType);
                    _enumTypes.Add(enumType);
                }
                else if (xmlNode.Name == "struct")
                {
                    string structName = GetAttribute(xmlNode, "Type");
                    //StructType structType = TypeManager.Instance.FindStruct(structName);
                    StructType structType = _stuctTypes.Find(x => x.Fullname.Replace("::", ".") == structName.Replace("::", "."));


                    if (structType != null)
                    {
                        //TypeManager.Instance.Structs.Remove(structType);
                        _stuctTypes.Remove(structType);
                    }

                    string[] structNames = structName.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    structName = structNames[structNames.Length - 1];

                    string isRef = GetAttribute(xmlNode, "IsRefType");
                    string isCustomized = GetAttribute(xmlNode, "IsCustomized");
                    string isImplemented = GetAttribute(xmlNode, "IsImplemented");
                    string ns = GetAttribute(xmlNode, "Namespace");
                    string baseName = GetAttribute(xmlNode, "Base");
                    string exportLocation = GetAttribute(xmlNode, "ExportLocation");
                    exportLocation = exportLocation.Replace("\\\\", "/");
                    exportLocation = exportLocation.Replace("\\", "/");
                    string displayName = GetAttribute(xmlNode, "DisplayName");
                    string desc = GetAttribute(xmlNode, "Desc");
                    string isStatic = GetAttribute(xmlNode, "Static");

                    //structType = new StructType(isRef == "true", isCustomized == "true", isImplemented == "true", structName, ns, baseName, exportLocation, displayName, desc);
                    structType = new StructType();
                    structType.isRef = isRef == "true";
                    structType.isCustomized = isCustomized == "true";
                    structType.isImplemented = isImplemented == "true";
                    structType.structName = structName;
                    structType.Namespace = ns;
                    structType.BaseName = baseName;
                    structType.ExportLocation = exportLocation;
                    structType.DisplayName = displayName;
                    structType.Description = desc;

                    foreach (XmlNode memberNode in xmlNode.ChildNodes)
                    {
                        if (memberNode.Name == "Member")
                        {
                            string memberName = GetAttribute(memberNode, "Name");
                            string[] memberNames = memberName.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            memberName = memberNames[memberNames.Length - 1];

                            string memberType = GetAttribute(memberNode, "TypeFullName");

                            if (string.IsNullOrEmpty(memberType))
                            {
                                memberType = GetAttribute(memberNode, "Type");
                            }

                            Type type = GetTypeFromName(memberType);

                            string memberDisplayName = GetAttribute(memberNode, "DisplayName");
                            string memberDesc = GetAttribute(memberNode, "Desc");

                            string memberReadonly = GetAttribute(memberNode, "Readonly");

                            //PropertyDef structProp = new PropertyDef(null, type, structName, memberName, memberDisplayName, memberDesc);
                            PropertyDef structProp = new PropertyDef();
                            structProp.Type = type;
                            structProp.ClassName = structName;
                            structProp.Name = memberName;
                            structProp.DisplayName = displayName;
                            structProp.BasicDescription = memberDesc;
                            structProp.IsStatic = (isStatic == "true");
                            structProp.IsReadonly = (memberReadonly == "true");

                            if (string.IsNullOrEmpty(structProp.NativeType))
                            {
                                structProp.NativeType = memberType;
                            }

                            //structType.AddProperty(structProp);
                            structType.Properties.Add(structProp);
                        }
                    }

                    //TypeManager.Instance.Structs.Add(structType);
                    _stuctTypes.Add(structType);
                }
            }
        }
        catch (Exception)
        {
            Debug.LogError(string.Format("errors when loading custom types"));
            //MessageBox.Show(errorInfo, "Loading Meta", MessageBoxButtons.OK);
        }
    }

    private void LoadAgents(XmlNode rootNode)
    {
        try
        {
            if (rootNode == null)
            {
                return;
            }

            // Set the default base agent.
            //if (Plugin.AgentTypes.Count == 0)
            //{
            //    AgentType agent = new AgentType(typeof(Agent), "Agent", "", false, false, "Agent", "", false, true, "");
            //    Plugin.AgentTypes.Add(agent);
            //}

            string agentName;

            // first pass, to load all the agents as it might be used as a property of another agent
            foreach (XmlNode xmlNode in rootNode.ChildNodes)
            {
                if (xmlNode.Name == "agent")
                {
                    LoadAgentType(xmlNode, out agentName);
                }
            }

            foreach (XmlNode xmlNode in rootNode.ChildNodes)
            {
                if (xmlNode.Name == "agent")
                {
                    AgentType agent = LoadAgentType(xmlNode, out agentName);

                    foreach (XmlNode bbNode in xmlNode)
                    {
                        if (bbNode.Name == "properties")
                        {
                            foreach (XmlNode propNode in bbNode)
                            {
                                loadCustomProperties(propNode, agent);
                            }
                        }
                        else if (bbNode.Name == "methods")
                        {
                            foreach (XmlNode methodNode in bbNode)
                            {
                                loadCustomMethods(methodNode, agent);
                            }
                        }
                        else if (bbNode.Name == "Member")
                        {
                            loadCustomProperties(bbNode, agent);
                        }
                        else if (bbNode.Name == "Method")
                        {
                            loadCustomMethods(bbNode, agent);
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            Debug.LogError(string.Format("error when loading custom members"));

            //MessageBox.Show(errorInfo, "Loading Custom Meta", MessageBoxButtons.OK);
        }
    }

    private AgentType LoadAgentType(XmlNode xmlNode, out string agentName)
    {
        agentName = GetAttribute(xmlNode, "type");

        if (string.IsNullOrEmpty(agentName))
        {
            agentName = GetAttribute(xmlNode, "classfullname");
        }

        //AgentType agent = Plugin.GetAgentType(agentName);
        AgentType agent = null;
        foreach (var obj in _agentTypes)
        {
            if (obj.Key.EndsWith(agentName))
            {
                agent = obj.Value;
                break;
            }
        }

        if (agent == null)
        {
            string agentBase = GetAttribute(xmlNode, "base");
            int baseIndex = -1;

            //for (int i = 0; i < Plugin.AgentTypes.Count; ++i)
            //{
            //    if (Plugin.AgentTypes[i].Name == agentBase)
            //    {
            //        baseIndex = i;
            //        break;
            //    }
            //}

            string oldName = GetAttribute(xmlNode, "OldName");

            string agentDisp = GetAttribute(xmlNode, "disp");

            if (string.IsNullOrEmpty(agentDisp))
            {
                agentDisp = GetAttribute(xmlNode, "DisplayName");
            }

            string agentDesc = GetAttribute(xmlNode, "desc");

            if (string.IsNullOrEmpty(agentDesc))
            {
                agentDesc = GetAttribute(xmlNode, "Desc");
            }

            if (string.IsNullOrEmpty(agentDisp))
            {
                agentDisp = agentName;
            }

            string isCustomized = GetAttribute(xmlNode, "IsCustomized");
            string isImplemented = GetAttribute(xmlNode, "IsImplemented");
            string exportLocation = GetAttribute(xmlNode, "ExportLocation");
            exportLocation = exportLocation.Replace("\\\\", "/");
            exportLocation = exportLocation.Replace("\\", "/");

            //agent = new AgentType(isCustomized == "true", isImplemented == "true", agentName, oldName, (baseIndex > -1) ? Plugin.AgentTypes[baseIndex] : null, exportLocation, agentDisp, agentDesc);
            agent = new AgentType();
            agent.IsCustomized = isCustomized == "true";
            agent.IsImplemented = isImplemented == "true";
            agent.Name = agentName;
            agent.OldName = oldName;
            agent.Base = null;
            agent.DisplayName = agentDisp;
            agent.Description = agentDesc;

            //Plugin.AgentTypes.Add(agent);
            _agentTypes[agent.Name] = agent;
        }

        return agent;
    }

    private void loadCustomProperties(XmlNode propNode, AgentType agent)
    {
        //Debug.Check(propNode.Name == "property" || propNode.Name == "Member");

        string propName = GetAttribute(propNode, "name");

        if (string.IsNullOrEmpty(propName))
        {
            propName = GetAttribute(propNode, "Name");
        }

        try
        {
            string isStatic = GetAttribute(propNode, "static");

            if (string.IsNullOrEmpty(isStatic))
            {
                isStatic = GetAttribute(propNode, "Static");
            }

            bool bStatic = (!string.IsNullOrEmpty(isStatic) && isStatic == "true");

            string isPublic = GetAttribute(propNode, "public");

            if (string.IsNullOrEmpty(isPublic))
            {
                isPublic = GetAttribute(propNode, "Public");
            }

            bool bPublic = (string.IsNullOrEmpty(isPublic) || isPublic == "true");

            string isReadonly = GetAttribute(propNode, "readonly");

            if (string.IsNullOrEmpty(isReadonly))
            {
                isReadonly = GetAttribute(propNode, "Readonly");
            }

            bool bReadonly = (!string.IsNullOrEmpty(isReadonly) && isReadonly == "true");

            string propType = GetAttribute(propNode, "type");

            if (string.IsNullOrEmpty(propType))
            {
                propType = GetAttribute(propNode, "TypeFullName");
            }

            Type type = GetTypeFromName(propType);

            string classname = GetAttribute(propNode, "classname");

            if (string.IsNullOrEmpty(classname))
            {
                classname = GetAttribute(propNode, "Class");
            }

            if (string.IsNullOrEmpty(classname))
            {
                classname = agent.Name;
            }

            string propDisp = GetAttribute(propNode, "disp");

            if (string.IsNullOrEmpty(propDisp))
            {
                propDisp = GetAttribute(propNode, "DisplayName");
            }

            if (string.IsNullOrEmpty(propDisp))
            {
                propDisp = propName;
            }

            string propDesc = GetAttribute(propNode, "desc");

            if (string.IsNullOrEmpty(propDesc))
            {
                propDesc = GetAttribute(propNode, "Desc");
            }

            //PropertyDef prop = new PropertyDef(agent, type, classname, propName, propDisp, propDesc);
            PropertyDef prop = new PropertyDef();
            prop.AgentType = agent;
            prop.Type = type;
            prop.ClassName = classname;
            prop.Name = propName;
            prop.DisplayName = propDisp;
            prop.BasicDescription = propDesc;
            prop.IsStatic = bStatic;
            prop.IsPublic = bPublic;
            prop.IsReadonly = bReadonly;

            string defaultValue = GetAttribute(propNode, "defaultvalue");

            if (!string.IsNullOrEmpty(defaultValue))
            {
                prop.Variable = new VariableDef();
                //Plugin.InvokeTypeParser(result, type, defaultValue, (object value) => prop.Variable.Value = value, null);
            }

            //agent.AddProperty(prop);
            agent.Properties.Add(prop);
        }
        catch (Exception)
        {
            Debug.LogError(string.Format("error when loading Agent '{0}' Member '{1}'", agent.Name, propName));
            //MessageBox.Show(errorInfo, "Loading Custom Meta", MessageBoxButtons.OK);
        }
    }

    private void loadCustomMethods(XmlNode methodNode, AgentType agent)
    {
        //Debug.Check(methodNode.Name == "method" || methodNode.Name == "Method");

        string methodName = GetAttribute(methodNode, "name");

        if (string.IsNullOrEmpty(methodName))
        {
            methodName = GetAttribute(methodNode, "Name");
        }

        try
        {
            string returnTypename = GetAttribute(methodNode, "ReturnType");

            if (string.IsNullOrEmpty(returnTypename))
            {
                returnTypename = GetAttribute(methodNode, "ReturnTypeFullName");
            }

            Type returnType = GetTypeFromName(returnTypename);

            string isStatic = GetAttribute(methodNode, "static");

            if (string.IsNullOrEmpty(isStatic))
            {
                isStatic = GetAttribute(methodNode, "Static");
            }

            bool bStatic = (!string.IsNullOrEmpty(isStatic) && isStatic == "true");

            string isPublic = GetAttribute(methodNode, "public");

            if (string.IsNullOrEmpty(isPublic))
            {
                isPublic = GetAttribute(methodNode, "Public");
            }

            bool bPublic = (string.IsNullOrEmpty(isPublic) || isPublic == "true");

            string classname = GetAttribute(methodNode, "classname");

            if (string.IsNullOrEmpty(classname))
            {
                classname = GetAttribute(methodNode, "Class");
            }

            if (string.IsNullOrEmpty(classname))
            {
                classname = agent.Name;
            }

            string methodDisp = GetAttribute(methodNode, "disp");

            if (string.IsNullOrEmpty(methodDisp))
            {
                methodDisp = GetAttribute(methodNode, "DisplayName");
            }

            if (string.IsNullOrEmpty(methodDisp))
            {
                methodDisp = methodName;
            }

            string methodDesc = GetAttribute(methodNode, "desc");

            if (string.IsNullOrEmpty(methodDesc))
            {
                methodDesc = GetAttribute(methodNode, "Desc");
            }

            bool istask = (GetAttribute(methodNode, "istask") == "true");
            //bool isevent = (GetAttribute(methodNode, "isevent") == "true");

            MemberType memberType = MemberType.Method;

            if (istask)
            {
                memberType = MemberType.Task;
            }

            methodName = string.Format("{0}::{1}", agent.Name, methodName);

            //MethodDef method = new MethodDef(agent, memberType, classname, methodName, methodDisp, methodDesc, "", returnType);
            MethodDef method = new MethodDef();
            method.AgentType = agent;
            method.MemberType = memberType;
            method.Name = methodName;
            method.ClassName = classname;
            method.DisplayName = methodDisp;
            method.BasicDescription = methodDesc;
            method.NativeReturnType = GetNativeTypeName(returnType);
            method.ReturnType = returnType;
            method.IsStatic = bStatic;
            method.IsPublic = bPublic;

            //agent.AddMethod(method);
            agent.Methods.Add(method);

            foreach (XmlNode paramNode in methodNode)
            {
                string paramName = GetAttribute(paramNode, "name");

                if (string.IsNullOrEmpty(paramName))
                {
                    paramName = GetAttribute(paramNode, "Name");
                }

                string paramTypename = GetAttribute(paramNode, "type");

                if (string.IsNullOrEmpty(paramTypename))
                {
                    paramTypename = GetAttribute(paramNode, "TypeFullName");
                }

                Type paramType = GetTypeFromName(paramTypename);

                string isOutStr = GetAttribute(paramNode, "isout");

                if (string.IsNullOrEmpty(isOutStr))
                {
                    isOutStr = GetAttribute(paramNode, "IsOut");
                }

                string isRefStr = GetAttribute(paramNode, "isref");

                if (string.IsNullOrEmpty(isRefStr))
                {
                    isRefStr = GetAttribute(paramNode, "IsRef");
                }

                string isConstStr = GetAttribute(paramNode, "IsConst");

                string nativeType = GetNativeTypeName(paramType);

                string paramDisp = GetAttribute(paramNode, "disp");

                if (string.IsNullOrEmpty(paramDisp))
                {
                    paramDisp = GetAttribute(paramNode, "DisplayName");
                }

                if (string.IsNullOrEmpty(paramDisp))
                {
                    paramDisp = paramName;
                }

                string paramDesc = GetAttribute(paramNode, "desc");

                if (string.IsNullOrEmpty(paramDesc))
                {
                    paramDesc = GetAttribute(paramNode, "Desc");
                }

                //MethodDef.Param param = new MethodDef.Param(paramName, paramType, nativeType, paramDisp, paramDesc);
                MethodDef.Param param = new MethodDef.Param();
                param.Name = paramName;
                param.Type = paramType;
                param.NativeType = nativeType;
                param.DisplayName = paramDisp;
                param.Description = paramDesc;

                param.IsOut = (isOutStr == "true");
                param.IsRef = (isRefStr == "true");
                param.IsConst = (isConstStr == "true");

                method.Params.Add(param);
            }
        }
        catch (Exception)
        {
            Debug.LogError(string.Format("error when loading Agent '{0}' Method '{1}'", agent.Name, methodName));
            //MessageBox.Show(errorInfo, "Loading Custom Meta", MessageBoxButtons.OK);
        }
    }

    private void SaveTypes(XmlDocument bbfile, XmlNode meta)
    {
        XmlElement root = bbfile.CreateElement("types");
        meta.AppendChild(root);

        foreach (EnumType enumType in _enumTypes)
        {
            string enumFullname = enumType.Fullname;

            XmlElement enumEle = bbfile.CreateElement("enumtype");
            enumEle.SetAttribute("Type", enumFullname);
            enumEle.SetAttribute("Namespace", enumType.Namespace);

            if (enumType.isCustomized)
            {
                enumEle.SetAttribute("IsCustomized", "true");
            }

            if (enumType.isImplemented)
            {
                enumEle.SetAttribute("IsImplemented", "true");
            }

            if (!string.IsNullOrEmpty(enumType.ExportLocation))
            {
                enumEle.SetAttribute("ExportLocation", enumType.ExportLocation);
            }

            enumEle.SetAttribute("DisplayName", enumType.DisplayName);
            enumEle.SetAttribute("Desc", enumType.Description);

            foreach (EnumType.EnumMemberType member in enumType.Members)
            {
                XmlElement memberEle = bbfile.CreateElement("enum");

                member.Namespace = enumType.Namespace;
                if (!string.IsNullOrEmpty(enumType.Namespace) && !member.NativeValue.Contains("::"))
                {
                    member.NativeValue = enumType.Namespace + "::" + member.NativeValue;
                }

                memberEle.SetAttribute("NativeValue", member.NativeValue);
                memberEle.SetAttribute("Value", member.Name);
                memberEle.SetAttribute("DisplayName", member.DisplayName);
                memberEle.SetAttribute("Desc", member.Description);
                memberEle.SetAttribute("MemberValue", member.Value.ToString());

                enumEle.AppendChild(memberEle);
            }

            root.AppendChild(enumEle);
        }

        foreach (StructType structType in _stuctTypes)
        {
            string structFullname = structType.Fullname;

            XmlElement structEle = bbfile.CreateElement("struct");

            structEle.SetAttribute("Type", structFullname);
            structEle.SetAttribute("Namespace", structType.Namespace);
            structEle.SetAttribute("Base", structType.BaseName);

            if (structType.isRef)
            {
                structEle.SetAttribute("IsRefType", "true");
            }

            if (structType.isCustomized)
            {
                structEle.SetAttribute("IsCustomized", "true");
            }

            if (structType.isImplemented)
            {
                structEle.SetAttribute("IsImplemented", "true");
            }

            if (!string.IsNullOrEmpty(structType.ExportLocation))
            {
                structEle.SetAttribute("ExportLocation", structType.ExportLocation);
            }

            structEle.SetAttribute("DisplayName", structType.DisplayName);
            structEle.SetAttribute("Desc", structType.Description);

            foreach (PropertyDef member in structType.Properties)
            {
                XmlElement memberEle = bbfile.CreateElement("Member");

                memberEle.SetAttribute("Name", member.BasicName);
                memberEle.SetAttribute("DisplayName", member.DisplayName);
                memberEle.SetAttribute("Desc", member.BasicDescription);

                //Debug.Check(member.Type != null);
                memberEle.SetAttribute("Type", member.NativeType);
                memberEle.SetAttribute("TypeFullName", member.Type.FullName);

                memberEle.SetAttribute("Class", structFullname);
                memberEle.SetAttribute("Public", "true");

                if (member.IsReadonly)
                {
                    memberEle.SetAttribute("Readonly", "true");
                }

                structEle.AppendChild(memberEle);
            }

            root.AppendChild(structEle);
        }
    }

    private void SaveAgents(XmlDocument bbfile, XmlNode meta)
    {
        XmlElement root = bbfile.CreateElement("agents");
        meta.AppendChild(root);

        foreach (AgentType agent in _agentTypes.Values)
        {
            XmlElement bbEle = bbfile.CreateElement("agent");
            bbEle.SetAttribute("classfullname", agent.Name);

            if (agent.Base != null)
            {
                bbEle.SetAttribute("base", agent.Base.Name);
            }
            else
            {
                //偷个懒，为空的情况下，默认agent的 base 是behaviac::Agent
                if (agent.Name != "behaviac::Agent")
                {
                    bbEle.SetAttribute("base", "behaviac::Agent");
                }
            }

            if (!string.IsNullOrEmpty(agent.OldName) && agent.OldName != agent.Name)
            {
                bbEle.SetAttribute("OldName", agent.OldName);
            }

            bbEle.SetAttribute("DisplayName", agent.DisplayName);
            bbEle.SetAttribute("Desc", agent.Description);
            bbEle.SetAttribute("IsRefType", "true");

            if (agent.IsStatic)
            {
                bbEle.SetAttribute("IsStatic", "true");
            }

            if (agent.IsCustomized)
            {
                bbEle.SetAttribute("IsCustomized", "true");
            }

            if (agent.IsImplemented)
            {
                bbEle.SetAttribute("IsImplemented", "true");
            }

            if (!string.IsNullOrEmpty(agent.ExportLocation))
            {
                bbEle.SetAttribute("ExportLocation", agent.ExportLocation);
            }

            foreach (PropertyDef prop in agent.Properties)
            {
                //if (prop.IsArrayElement || prop.IsPar || prop.IsInherited)
                //{
                //    continue;
                //}

                XmlElement propEle = bbfile.CreateElement("Member");

                propEle.SetAttribute("Name", prop.BasicName);
                propEle.SetAttribute("DisplayName", prop.DisplayName);
                propEle.SetAttribute("Desc", prop.BasicDescription);
                propEle.SetAttribute("Class", prop.ClassName);
                propEle.SetAttribute("Type", prop.NativeType);
                propEle.SetAttribute("TypeFullName", (prop.Type != null) ? prop.Type.FullName : prop.NativeType);

                if (prop.IsCustomized)
                {
                    propEle.SetAttribute("IsCustomized", "true");
                }

                propEle.SetAttribute("defaultvalue", prop.DefaultValue);
                propEle.SetAttribute("Static", prop.IsStatic ? "true" : "false");
                propEle.SetAttribute("Public", prop.IsPublic ? "true" : "false");
                propEle.SetAttribute("Readonly", prop.IsReadonly ? "true" : "false");

                bbEle.AppendChild(propEle);
            }

            foreach (MethodDef method in agent.Methods)
            {
                //if (method.IsInherited)
                //{
                //    continue;
                //}

                XmlElement methodEle = bbfile.CreateElement("Method");

                methodEle.SetAttribute("Name", method.BasicName);
                methodEle.SetAttribute("DisplayName", method.DisplayName);
                methodEle.SetAttribute("Desc", method.BasicDescription);
                methodEle.SetAttribute("Class", method.ClassName);
                methodEle.SetAttribute("ReturnType", string.IsNullOrEmpty(method.NativeReturnType) ? "void" : method.NativeReturnType);
                methodEle.SetAttribute("ReturnTypeFullName", method.ReturnType != null ? method.ReturnType.FullName : "System.Void");
                methodEle.SetAttribute("Static", method.IsStatic ? "true" : "false");
                methodEle.SetAttribute("Public", method.IsPublic ? "true" : "false");
                methodEle.SetAttribute("istask", (method.MemberType == MemberType.Task) ? "true" : "false");
                //methodEle.SetAttribute("isevent", (method.IsNamedEvent || method.MemberType == MemberType.Task) ? "true" : "false");

                foreach (MethodDef.Param param in method.Params)
                {
                    XmlElement paramEle = bbfile.CreateElement("Param");

                    paramEle.SetAttribute("Name", param.Name);
                    paramEle.SetAttribute("Type", param.NativeType);
                    paramEle.SetAttribute("TypeFullName", (param.Type != null) ? param.Type.FullName : param.NativeType);

                    if (param.IsOut)
                    {
                        paramEle.SetAttribute("IsOut", "true");
                    }

                    if (param.IsRef)
                    {
                        paramEle.SetAttribute("IsRef", "true");
                    }

                    if (param.IsConst)
                    {
                        paramEle.SetAttribute("IsConst", "true");
                    }

                    paramEle.SetAttribute("DisplayName", param.DisplayName);
                    paramEle.SetAttribute("Desc", param.Description);

                    methodEle.AppendChild(paramEle);
                }

                bbEle.AppendChild(methodEle);
            }

            root.AppendChild(bbEle);
        }

        XmlElement instanceRoot = bbfile.CreateElement("instances");
        meta.AppendChild(instanceRoot);

        //foreach (Plugin.InstanceName_t instance in Plugin.InstanceNames)
        //{
        //    XmlElement instanceEle = bbfile.CreateElement("instance");
        //    instanceEle.SetAttribute("name", instance.Name);
        //    instanceEle.SetAttribute("class", instance.ClassName);
        //    instanceEle.SetAttribute("DisplayName", instance.DisplayName);
        //    instanceEle.SetAttribute("Desc", instance.Desc);

        //    instanceRoot.AppendChild(instanceEle);
        //}
    }

    public static Type GetTypeFromName(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return null;
        }

        switch (typeName)
        {
            case "bool":
                return typeof(bool);

            case "int":
                return typeof(int);

            case "uint":
                return typeof(uint);

            case "short":
                return typeof(short);

            case "ushort":
                return typeof(ushort);

            case "char":
                return typeof(char);

            case "sbyte":
                return typeof(sbyte);

            case "ubyte":
                return typeof(byte);

            case "byte":
                return typeof(byte);

            case "long":
                return typeof(long);

            case "ulong":
                return typeof(ulong);

            case "float":
                return typeof(float);

            case "double":
                return typeof(double);

            case "string":
                return typeof(string);

            case "behaviac.EBTStatus":
            case "EBTStatus":
                return typeof(behaviac.EBTStatus);
        }

        Type type = null;

        bool isVec = typeName.StartsWith("vector<");
        bool isList = false;

        if (!isVec)
        {
            isList = typeName.StartsWith("List<");
        }

        if (isVec || isList)
        {
            try
            {
                if (isVec)
                {
                    typeName = typeName.Substring(7, typeName.Length - 8);
                }
                else
                {
                    typeName = typeName.Substring(5, typeName.Length - 6);
                }

                typeName = typeName.Replace("::", "_");

                type = GetTypeFromName(typeName);

                if (type != null)
                {
                    type = typeof(List<>).MakeGenericType(type);

                    return type;
                }
            }
            catch
            {
            }
        }

        if (type == null)
        {
            type = GetType(typeName);
        }

        if (type == null && (typeName.Contains("::") || typeName.Contains(".")))
        {
            typeName = typeName.Replace("::", "_");
            typeName = typeName.Replace(".", "_");
            type = GetType(typeName);
        }

        if (type == null)
        {
            typeName = "XMLPluginBehaviac." + typeName;
            type = GetType(typeName);
        }

        return type;
    }

    /// <summary>
    /// Returns the type of a given class name. It searches all loaded plugins for this type.
    /// </summary>
    /// <param name="fullname">The name of the class we want to get the type for.</param>
    /// <returns>Returns the type if found in any loaded plugin. Retuns null if it could not be found.</returns>
    public static Type GetType(string fullname)
    {
        // search base class
        Type type = Type.GetType(fullname);

        if (type != null)
        {
            return type;
        }

        bool bHasNamespace = false;

        if (fullname.IndexOf('.') != -1)
        {
            bHasNamespace = true;
        }

        // search loaded plugins
        //foreach (Assembly assembly in __loadedPlugins)
        //{
        //    if (bHasNamespace)
        //    {
        //        type = assembly.GetType(fullname);

        //    }
        //    else
        //    {
        //        Type[] assemblyTypes = assembly.GetTypes();

        //        for (int j = 0; j < assemblyTypes.Length; j++)
        //        {
        //            if (assemblyTypes[j].Name == fullname)
        //            {
        //                type = assemblyTypes[j];
        //                break;
        //            }
        //        }
        //    }

        //    if (type != null)
        //    {
        //        return type;
        //    }
        //}

        // it could be a List<> type
        if (fullname.StartsWith("System.Collections.Generic.List"))
        {
            int startIndex = fullname.IndexOf("[[");

            if (startIndex > -1)
            {
                int endIndex = fullname.IndexOf(",");

                if (endIndex < 0)
                {
                    endIndex = fullname.IndexOf("]]");
                }

                if (endIndex > startIndex)
                {
                    string item = fullname.Substring(startIndex + 2, endIndex - startIndex - 2);
                    type = GetType(item);

                    if (type != null)
                    {
                        type = typeof(List<>).MakeGenericType(type);
                        return type;
                    }
                }
            }
        }

        return null;
    }

    public static string GetNativeTypeName(Type type, bool bForDisplay = false)
    {
        if (type == null)
        {
            return string.Empty;
        }

        if (IsArrayType(type))
        {
            Type itemType = type.GetGenericArguments()[0];
            string itemTypeStr = GetNativeTypeName(itemType, bForDisplay);

            //if (!itemTypeStr.EndsWith("*") && Plugin.IsRefType(itemType))
            //{
            //    itemTypeStr += "*";
            //}

            return string.Format("vector<{0}>", itemTypeStr);
        }

        string typeStr = GetNativeTypeName(type.Name, false, bForDisplay);

        //if (!typeStr.EndsWith("*") && Plugin.IsRefType(type))
        //{
        //    typeStr += "*";
        //}

        return typeStr;
    }

    public static string GetNativeTypeName(string typeName, bool withNamespace = false, bool bForDisplay = false)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return string.Empty;
        }

        foreach (KeyValuePair<string, string> pair in ms_type_mapping)
        {
            if (pair.Key == typeName)
            {
                if (bForDisplay)
                {
                    return pair.Value;
                }

                return pair.Value;
            }
            else
            {
                string refType = pair.Key + "&";

                if (refType == typeName)
                {
                    string ret = pair.Value;

                    if (bForDisplay)
                    {
                        ret = pair.Value;
                    }

                    return ret + "&";
                }
            }
        }

        typeName = typeName.Replace("const char*", "cszstring");
        typeName = typeName.Replace("char*", "szstring");
        typeName = typeName.Replace("const ", "");
        typeName = typeName.Replace("unsigned long long", "ullong");
        typeName = typeName.Replace("signed long long", "llong");
        typeName = typeName.Replace("long long", "llong");
        typeName = typeName.Replace("unsigned ", "u");

        if (!typeName.Contains("signed char"))
        {
            typeName = typeName.Replace("signed ", "");
        }

        //typeName = TypeManager.Instance.GetCurrentName(typeName);

        //if (NamesInNamespace.ContainsKey(typeName))
        //{
        //    string nativeTypeName = NamesInNamespace[typeName];

        //    //Type type = Plugin.GetType(typeName);
        //    //if (type != null && type.IsSubclassOf(typeof(Behaviac.Design.Agent)))
        //    //{
        //    //    nativeTypeName += "*";
        //    //}

        //    return nativeTypeName;
        //}

        if (withNamespace)
        {
            return typeName;
        }

        string[] types = typeName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        return types[types.Length - 1];
    }

    static Dictionary<string, string> ms_type_mapping = new Dictionary<string, string>()
        {
            {"Boolean"          , "bool"},
            {"System.Boolean"   , "bool"},
            {"Int32"            , "int"},
            {"System.Int32"     , "int"},
            {"UInt32"           , "uint"},
            {"System.UInt32"    , "uint"},
            {"Int16"            , "short"},
            {"System.Int16"     , "short"},
            {"UInt16"           , "ushort"},
            {"System.UInt16"    , "ushort"},
            {"Int8"             , "sbyte"},
            {"System.Int8"      , "sbyte"},
            {"SByte"            , "sbyte"},
            {"System.SByte"     , "sbyte"},
            {"UInt8"            , "ubyte"},
            {"System.UInt8"     , "ubyte"},
            {"Byte"             , "ubyte"},
            {"System.Byte"      , "ubyte"},
            {"Char"             , "char"},
            {"Int64"            , "long"},
            {"System.Int64"     , "long"},
            {"UInt64"           , "ulong"},
            {"System.UInt64"    , "ulong"},
            {"Single"           , "float"},
            {"System.Single"    , "float"},
            {"Double"           , "double"},
            {"System.Double"    , "double"},
            {"String"           , "string"},
            {"System.String"    , "string"},
            {"Void"             , "void"},
            {"System.Void"      , "void"},
            {"Behaviac.Designer.llong",  "llong"},
            {"Behaviac.Designer.ullong", "ullong"}
        };

    public static bool IsArrayType(Type type)
    {
        return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }
    #endregion
}
