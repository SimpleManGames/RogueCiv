using SyntaxTree.VisualStudio.Unity.Bridge;
using UnityEditor;

[InitializeOnLoad]
public class ReferenceRemovalProjectHook
{
    static ReferenceRemovalProjectHook()
    {
        const string references = "\r\n    <Reference Include=\"Boo.Lang\" />\r\n    <Reference Include=\"UnityScript.Lang\" />";

        ProjectFilesGenerator.ProjectFileGeneration += (string name, string content) =>
            content.Replace(references, "");
    }
}