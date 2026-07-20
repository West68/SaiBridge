using System;
using System.Text;
using System.Windows.Automation;

namespace SaiBridge.Automation;

public static class UiAutomationHelper
{
    public static Action<string>? Logger;
    public static StringBuilder Result = new();


    public static string DumpTree(IntPtr hwnd)
    {
        Result.Clear();

        AutomationElement root =
            AutomationElement.FromHandle(hwnd);


        Dump(root, 0);


        return Result.ToString();
    }



    private static void Dump(
        AutomationElement element,
        int depth)
    {
        try
        {
            string indent =
                new string(' ', depth * 2);


            string name =
                element.Current.Name;


            string className =
                element.Current.ClassName;


            bool focus =
                element.Current.HasKeyboardFocus;


            string type =
                element.Current.ControlType.ProgrammaticName;


            string automationId =
                element.Current.AutomationId;



            Result.AppendLine(
                $"{indent}{type} | {name} | Focus:{focus}"
            );


            if (!string.IsNullOrEmpty(automationId))
            {
                Result.AppendLine(
                    $"{indent}  AutomationId: {automationId}");
            }


            if (!string.IsNullOrEmpty(className))
            {
                Result.AppendLine(
                    $"{indent}  ClassName: {className}");
            }



            // ValuePattern
            try
            {
                if (element.TryGetCurrentPattern(
                    ValuePattern.Pattern,
                    out object valuePattern))
                {
                    Result.AppendLine(
                        $"{indent}  [ValuePattern]");
                }
            }
            catch
            {

            }



            // TextPattern
            try
            {
                if (element.TryGetCurrentPattern(
                    TextPattern.Pattern,
                    out object textPattern))
                {
                    Result.AppendLine(
                        $"{indent}  [TextPattern]");
                }
            }
            catch
            {

            }



            // 遍历子节点
            AutomationElementCollection children =
     element.FindAll(
         TreeScope.Children,
         Condition.TrueCondition);


            foreach (AutomationElement child in children)
            {
                Dump(child, depth + 1);
            }

        }
        catch
        {

        }
    }
    public static AutomationElement? FindFile(
    IntPtr hwnd,
    string fileName)
    {
        AutomationElement root =
            AutomationElement.FromHandle(hwnd);


        return Find(
            root,
            fileName);
    }


    private static AutomationElement? Find(
    AutomationElement element,
    string fileName)
    {
        try
        {
            Console.WriteLine(
                "开始搜索:" + fileName);


            AutomationElementCollection children =
                element.FindAll(
                    TreeScope.Descendants,
                    Condition.TrueCondition);


            Console.WriteLine(
                "节点数量:" + children.Count);


            foreach (AutomationElement child in children)
            {
                string name = "";

                try
                {
                    name = child.Current.Name;
                }
                catch
                {

                }


                Console.WriteLine(
                    "节点:" + name);


                if (!string.IsNullOrEmpty(name) &&
                    name.Contains(fileName))
                {
                    Console.WriteLine(
                        "找到:" + name);

                    return child;
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }


        return null;
    }

    public static AutomationElement? FindFileByDump(
    IntPtr hwnd,
    string fileName)
    {
        AutomationElement root =
            AutomationElement.FromHandle(hwnd);


        return FindByChildren(
            root,
            fileName);
    }



    private static AutomationElement? FindByChildren(
        AutomationElement element,
        string fileName)
    {
        try
        {
            string name =
                element.Current.Name;


            if (!string.IsNullOrEmpty(name) &&
                name.Contains(fileName))
            {
                return element;
            }


            AutomationElementCollection children =
                element.FindAll(
                    TreeScope.Children,
                    Condition.TrueCondition);


            foreach (AutomationElement child in children)
            {
                var result =
                    FindByChildren(
                        child,
                        fileName);


                if (result != null)
                    return result;
            }

        }
        catch
        {

        }


        return null;
    }

    public static AutomationElement? FindByAutomationId(
    IntPtr hwnd,
    string id)
    {
        AutomationElement root =
            AutomationElement.FromHandle(hwnd);


        return root.FindFirst(
            TreeScope.Descendants,
            new PropertyCondition(
                AutomationElement.AutomationIdProperty,
                id));
    }

    public static AutomationElement? FindFileNode(
    IntPtr hwnd,
    string fileName)
    {
        AutomationElement root =
            AutomationElement.FromHandle(hwnd);


        AutomationElementCollection list =
            root.FindAll(
                TreeScope.Descendants,
                Condition.TrueCondition);


        foreach (AutomationElement item in list)
        {
            try
            {
                string name =
                    item.Current.Name;

                Logger?.Invoke(
           $"发现节点:{name}\n");

                if (name.Contains(fileName))
                {
                    return item;
                }
            }
            catch
            {

            }
        }


        return null;
    }

    public static void TestPatterns(IntPtr hwnd)
    {
        AutomationElement root =
            AutomationElement.FromHandle(hwnd);


        AutomationElementCollection list =
            root.FindAll(
                TreeScope.Descendants,
                Condition.TrueCondition);


        foreach (AutomationElement item in list)
        {
            try
            {
                string name =
                    item.Current.Name;


                var rect =
                    item.Current.BoundingRectangle;


                Result.AppendLine(
                    $"名字:{name} 位置:{rect.Left},{rect.Top},{rect.Right},{rect.Bottom}");
            }
            catch
            {

            }
        }
    }
}