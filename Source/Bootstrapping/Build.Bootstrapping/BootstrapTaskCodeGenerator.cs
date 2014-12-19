namespace Build.Bootstrapping
{
    using System.Collections.Generic;
    using System.Text;

    public class BootstrapTaskCodeGenerator
    {
        public string GenerateTask(IBootstrapTask bootstrapTask)
        {
            StringBuilder taskString = new StringBuilder();
            taskString.AppendLine("<UsingTask");
            taskString.AppendLine("\tTaskName=\"BootstrapMsBuildByConvention\"");
            taskString.AppendLine("\tTaskFactory=\"CodeTaskFactory\"");
            taskString.AppendLine("\tAssemblyFile=\"$(MSBuildToolsPath)\\Microsoft.Build.Tasks.v4.0.dll\">");
            taskString.AppendLine("\t<ParameterGroup>");

            if (bootstrapTask.TaskParameters != null)
            {
                foreach (TaskParameter taskParameter in bootstrapTask.TaskParameters)
                {
                    taskString.AppendFormat(
                        "\t\t<{0} ParameterType=\"{1}\" Output=\"{2}\"></{0}>\n", taskParameter.Name, taskParameter.Type, taskParameter.IsOutput.ToString().ToLowerInvariant());
                }
            }


            taskString.AppendLine("\t</ParameterGroup>");
            taskString.AppendLine("\t<Task>");

            List<string> usings = bootstrapTask.GenerateUsings();
            string code = bootstrapTask.GenerateCSharpCode();
            this.AddUsings(taskString, usings);
            this.AddCode(taskString, code);
            taskString.AppendLine("\t</Task>");
            taskString.AppendLine("</UsingTask>");
            return taskString.ToString();
        }

        private void AddCode(StringBuilder taskString, string code)
        {
            taskString.AppendFormat("<Code Type=\"Fragment\" Language=\"cs\"><![CDATA[\n{0}\n]]>\n</Code>", code);
        }

        private void AddUsings(StringBuilder taskString, IEnumerable<string> usings)
        {
            if (usings != null)
            {
                foreach (string usingStatement in usings)
                {
                    taskString.AppendFormat("<Using Namespace=\"{0}\"/>", usingStatement);
                }
            }
        }
    }
}