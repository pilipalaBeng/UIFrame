using UnityEngine;
using System.Collections;
using System.IO;

namespace IMCUIEditor.UI
{
    public class CreateIMCFormCsharp
    {
        //private const string scriptPath = "Assets/_IMControl/IMC/UI/Resources/";
        private const string scriptName = "#CreateIMCFormName#";
        private const string cloningName = "#CreateScriptName#";
        private const string txtPostfix = ".cs.txt";
        private static CreateIMCFormCsharp instance;

        public static CreateIMCFormCsharp Instance
        {
            get
            {
                if (instance == null)
                    instance = new CreateIMCFormCsharp();
                return CreateIMCFormCsharp.instance;
            }
        }
        public void CreateScript(string name)
        {
            var files = System.IO.Directory.GetFiles(Application.dataPath, "#CreateIMCFormName#.cs.txt", System.IO.SearchOption.AllDirectories);
            string content = "";
            string tempStr = files[0];
            if (File.Exists(tempStr))
            {
                tempStr = tempStr.Replace("#CreateIMCFormName#", "#CreateScriptName#");
                //File.Copy(scriptPath + scriptName + txtPostfix, scriptPath + cloningName + txtPostfix, true);
                File.Copy(files[0], tempStr, true);
                //content = File.ReadAllText(scriptPath + cloningName + txtPostfix);
                content = File.ReadAllText(tempStr);
            }

            if (content.Contains(scriptName))
            {
                content = content.Replace(scriptName, name);
            }
            //File.WriteAllText(scriptPath + cloningName + txtPostfix, string.Empty);
            File.WriteAllText(tempStr, string.Empty);

            FileStream fs = new FileStream(tempStr, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(content);
            sw.Flush();
            fs.Close();

            if (File.Exists(tempStr))
            {
                FileInfo fi = new FileInfo(tempStr);
                fi.MoveTo("Assets/" + name + ".cs");
            }
        }
    }
}