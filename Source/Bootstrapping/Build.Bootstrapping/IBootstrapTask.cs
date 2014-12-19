namespace Build.Bootstrapping
{
    using System.Collections.Generic;

    public interface IBootstrapTask
    {
        IEnumerable<TaskParameter> TaskParameters { get; }

        List<string> GenerateUsings();

        string GenerateCSharpCode();

        void Execute();
    }
}