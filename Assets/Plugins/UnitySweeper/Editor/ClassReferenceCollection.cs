using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using UnityEditor;

namespace UnitySweeper
{
    public class ClassReferenceCollection : IReferenceCollection
    {
        // guid : types
        private List<CollectionData> references;

        // type : guid
        private Dictionary<Type, List<string>> code2FileDic = new Dictionary<Type, List<string>>();
        private readonly List<TypeDate> fileTypeList;
        
        public const string XMP_PATH = "referenceType2File.xml";

        private List<TypeDate> FileTypeXML
        {
            get
            {
                if (File.Exists(XMP_PATH))
                {
                    using (var reader = new StreamReader(XMP_PATH))
                    {
                        XmlSerializer serialize = new XmlSerializer(typeof(List<TypeDate>));
                        return (List<TypeDate>) serialize.Deserialize(reader);
                    }
                }

                return new List<TypeDate>();
            }
            set
            {
                using (var writer = new StreamWriter(XMP_PATH))
                {
                    XmlSerializer serialize = new XmlSerializer(typeof(List<TypeDate>));
                    serialize.Serialize(writer, value);
                }
            }
        }

        TypeDate GetTypeData(string guid)
        {
            if (fileTypeList.Exists(c => c.guid == guid) == false)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                fileTypeList.Add(new TypeDate
                {
                    guid = guid,
                    fileName = path,
                    timeStamp = File.GetLastWriteTime(path)
                });
            }

            return fileTypeList.First(c => c.guid == guid);
        }

        private bool isSaveEditorCode;

        public ClassReferenceCollection(bool saveStiroCode = false)
        {
            isSaveEditorCode = saveStiroCode;
            fileTypeList = FileTypeXML;
        }

        public void Init(List<CollectionData> refs)
        {
            references = refs;
        }

        public void CollectionFiles()
        {
            // connect each classes.
            var firstPassList = new List<string>();
            if (Directory.Exists("Assets/Plugins"))
            {
                firstPassList.AddRange(CodeList("Assets/Plugins"));
            }
            if (Directory.Exists("Assets/Standard Assets"))
            {
                firstPassList.AddRange(CodeList("Assets/Standard Assets"));
            }

            // Connect the files and class.
            var codes = CodeList("Assets/").Where(c => firstPassList.Contains(c) == false);

            var allFirstpassTypes = collectionAllFastspassClasses();
            CollectionCodeFileDictionary(allFirstpassTypes, firstPassList.ToArray());

            var alltypes = CollectionAllClasses();
            CollectionCodeFileDictionary(alltypes, codes.ToArray());
            alltypes.AddRange(allFirstpassTypes);


            FileTypeXML = fileTypeList;

            foreach (var type in alltypes)
            {
                List<string> list = null;
                if (code2FileDic.ContainsKey(type) == false)
                {
                    list = new List<string>();
                    code2FileDic.Add(type, list);
                }
                else
                {
                    list = code2FileDic[type];
                }

                var fullName = type.FullName;
                var assembly = type.Assembly.FullName;
                if (fileTypeList.Exists(c => c.assembly == assembly && c.typeFullName.Contains(fullName)))
                {
                    var datas = fileTypeList.Where(c => c.assembly == assembly && c.typeFullName.Contains(fullName));
                    foreach (var data in datas)
                    {
                        list.Add(data.guid);
                    }
                }
            }

            float count = 1, max = firstPassList.Count;
            foreach (var codepath in firstPassList)
            {
                EditorUtility.DisplayProgressBar("analytics", codepath, count++ / max);
                CollectionReferenceClasses(AssetDatabase.AssetPathToGUID(codepath), allFirstpassTypes);
            }

            count = 1;
            max = codes.Count();
            foreach (var codepath in codes)
            {
                EditorUtility.DisplayProgressBar("analytics", codepath, count++ / max);
                CollectionReferenceClasses(AssetDatabase.AssetPathToGUID(codepath), alltypes);
            }

            if (isSaveEditorCode)
            {
                CollectionCustomEditorClasses(alltypes);
            }
        }

        List<string> CodeList(string path)
        {
            string[] codes = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);

            List<string> needUpdateFileList = new List<string>();

            foreach (var code in codes)
            {
                var guid = AssetDatabase.AssetPathToGUID(code);
                if (fileTypeList.Exists(c => c.guid == guid) == false)
                {
                    needUpdateFileList.Add(code);
                    continue;
                }

                var filetype = GetTypeData(guid);

                var timeStamp = filetype.timeStamp;
                var time = File.GetLastWriteTime(code);
                if (time != timeStamp)
                {
                    filetype.timeStamp = time;
                    needUpdateFileList.Add(code);
                }
            }

            return needUpdateFileList;
        }

        void CollectionCodeFileDictionary(List<Type> alltypes, string[] codes)
        {
            float count = 1;
            foreach (var codePath in codes)
            {
                EditorUtility.DisplayProgressBar("checking", "search files", count++ / codes.Length);

                // connect file and classes.
                var code = StripComment(File.ReadAllText(codePath));
                var guid = AssetDatabase.AssetPathToGUID(codePath);

                var typeList = GetTypeData(guid);
                typeList.typeFullName.Clear();

                foreach (var type in alltypes)
                {
                    if (type.IsNested)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(type.Namespace) == false)
                    {
                        var namespacepattern = string.Format("namespace\\s*{0}[{{\\s\\n]", type.Namespace);
                        if (Regex.IsMatch(code, namespacepattern) == false)
                        {
                            continue;
                        }
                    }

                    string typeName = type.IsGenericTypeDefinition
                        ? type.GetGenericTypeDefinition().Name.Split('`')[0]
                        : type.Name;
                    if (type.IsClass)
                    {
                        if (Regex.IsMatch(code, string.Format("class\\s*{0}?[\\s:<{{]", typeName)))
                        {
                            typeList.Add(type);

                            var nested = type.GetNestedTypes(BindingFlags.Public | BindingFlags.Instance);

                            foreach (var nestedType in nested)
                            {
                                typeList.Add(nestedType);
                            }
                        }
                    }
                    else if (type.IsInterface)
                    {
                        if (Regex.IsMatch(code, string.Format("interface\\s*{0}[\\s<{{]", typeName)))
                        {
                            typeList.Add(type);
                        }
                    }
                    else if (type.IsEnum)
                    {
                        if (Regex.IsMatch(code, string.Format("enum\\s*{0}[\\s{{]", type.Name)))
                        {
                            typeList.Add(type);
                        }
                    }
                    else
                    {
                        if (Regex.IsMatch(code, string.Format("struct\\s*{0}[\\s:<{{]", typeName)))
                        {
                            typeList.Add(type);
                            continue;
                        }

                        if (Regex.IsMatch(code, string.Format("delegate\\s*{0}\\s\\(", typeName)))
                        {
                            typeList.Add(type);
                        }
                    }
                }
            }
        }

        List<Type> CollectionAllClasses()
        {
            const string assemblyDll = "Library/ScriptAssemblies/Assembly-CSharp.dll";
            const string assemblyDllEditor = "Library/ScriptAssemblies/Assembly-CSharp-Editor.dll";
            string path2Project = Environment.CurrentDirectory;
            
            List<Type> alltypes = new List<Type>();

            if (File.Exists(assemblyDll))
            {
                string absPath = Path.GetFullPath(Path.Combine(path2Project, @assemblyDll));
                alltypes.AddRange(Assembly.LoadFile(absPath).GetTypes());
            }
            if (isSaveEditorCode && File.Exists(assemblyDllEditor))
            {
                string absPath = Path.GetFullPath(Path.Combine(path2Project, @assemblyDllEditor));
                alltypes.AddRange(Assembly.LoadFile(absPath).GetTypes());
            }

            return alltypes.ToList();
        }

        List<Type> collectionAllFastspassClasses()
        {
            const string assemblyFirstpassDll = "Library/ScriptAssemblies/Assembly-CSharp-firstpass.dll";
            const string assemblyFirstpassEditorDll = "Library/ScriptAssemblies/Assembly-CSharp-Editor-firstpass.dll";
            string path2Project = Environment.CurrentDirectory;
        
            List<Type> alltypes = new List<Type>();
            if (File.Exists(assemblyFirstpassDll))
            {
                string absPath = Path.GetFullPath(Path.Combine(path2Project, @assemblyFirstpassDll));
                alltypes.AddRange(Assembly.LoadFile(absPath).GetTypes());
            }
            if (isSaveEditorCode && File.Exists(assemblyFirstpassEditorDll))
            {
                string absPath = Path.GetFullPath(Path.Combine(path2Project, @assemblyFirstpassEditorDll));
                alltypes.AddRange(Assembly.LoadFile(absPath).GetTypes());
            }
            return alltypes;
        }

        public static string StripComment(string code)
        {
            code = Regex.Replace(code, "//.*[\\n\\r]", "");
            code = Regex.Replace(code, "/\\*.*[\\n\\r]\\*/", "");
            return code;
        }

        void CollectionReferenceClasses(string guid, List<Type> types)
        {
            var codePath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(codePath) || File.Exists(codePath) == false)
            {
                return;
            }

            var code = StripComment(File.ReadAllText(codePath));

            List<string> referenceList = null;
            CollectionData reference = null;

            if (references.Exists(c => c.fileGuid == guid) == false)
            {
                referenceList = new List<string>();
                reference = new CollectionData
                {
                    fileGuid = guid,
                    referenceGuids = referenceList,
                };
                references.Add(reference);
            }
            else
            {
                reference = references.Find(c => c.fileGuid == guid);
                referenceList = reference.referenceGuids;
            }

            referenceList.Clear();

            var timestamp = File.GetLastWriteTime(codePath);
            reference.timeStamp = timestamp;

            foreach (var type in types)
            {
                if (code2FileDic.ContainsKey(type) == false || code2FileDic[type].Contains(guid))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(type.Namespace) == false)
                {
                    var namespacepattern = string.Format("([namespace|using][\\s]{0}[{{\\s\\r\\n\\r;]|{0}\\.)",
                        type.Namespace);
                    if (Regex.IsMatch(code, namespacepattern) == false)
                    {
                        continue;
                    }
                }

                string match = string.Empty;

                if (type.IsGenericTypeDefinition)
                {
                    string typeName = type.GetGenericTypeDefinition().Name.Split('`')[0];
                    match = string.Format("[!|&\\]\\[\\.\\s<(]{0}[\\.\\s\\n\\r>,<(){{]", typeName);
                }
                else
                {
                    string typeName = type.Name.Split('`')[0].Replace("Attribute", "");
                    match = string.Format("[!|&\\]\\[\\.\\s<(]{0}[\\.\\s\\n\\r>,<(){{\\]]", typeName);


                    //  check Extension Methods

                    if (Regex.IsMatch(code, string.Format("this\\s{0}\\s", typeName)))
                    {
                        foreach (var file in code2FileDic[type])
                        {
                            foreach (var baseReference in references.Where(c => c.fileGuid == file))
                            {
                                baseReference.referenceGuids.Add(guid);
                            }
                        }
                    }
                }

                if (Regex.IsMatch(code, match))
                {
                    var typeGuids = code2FileDic[type];
                    foreach (var typeGuid in typeGuids)
                    {
                        if (referenceList.Contains(typeGuid) == false)
                        {
                            referenceList.Add(typeGuid);
                        }
                    }
                }
            }
        }

        void CollectionCustomEditorClasses(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                if (code2FileDic.ContainsKey(type) == false)
                {
                    continue;
                }

                var attributes = type.GetCustomAttributes(typeof(CustomEditor), true);
                foreach (var attribute in attributes)
                {
                    if (attribute is CustomEditor == false)
                    {
                        continue;
                    }

                    var customEditor = attribute as CustomEditor;
                    var customEditorReferenceTypeField = typeof(CustomEditor).GetField("m_InspectedType",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                    var customEditorReferenceType = (Type) customEditorReferenceTypeField.GetValue(customEditor);

                    if (code2FileDic.ContainsKey(customEditorReferenceType) == false)
                    {
                        continue;
                    }

                    foreach (var filePath in code2FileDic[customEditorReferenceType])
                    {
                        if (references.Exists(c => c.fileGuid == filePath) == false)
                        {
                            continue;
                        }

                        foreach (var refs in references.Where(c => c.fileGuid == filePath))
                        {
                            var list = refs.referenceGuids;
                            list.AddRange(code2FileDic[type]);
                        }
                    }
                }
            }
        }
    }
}