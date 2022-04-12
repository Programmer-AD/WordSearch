using System.Reflection;
using WordSearch.Logic.Interfaces.Primary;

namespace WordSearch.CLI
{
    internal partial class CommandProcessor
    {
        private static readonly ILookup<string, MethodInfo> commandMethods = GetCommandMethods();

        private readonly IDatabaseManager databaseManager;
        public IDatabase UsedDatabase { get; private set; }

        public CommandProcessor(IDatabaseManager databaseManager)
        {
            this.databaseManager = databaseManager;
        }

        internal string Process(string input)
        {
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var methodName = NormalizeMethodName(parts[0]);

            if (commandMethods.Contains(methodName))
            {
                var methodOverloads = commandMethods[methodName];
                var trimmedInput = parts.Skip(1).ToArray();

                foreach (var overload in methodOverloads)
                {
                    if (TryCallCommandMethod(overload, trimmedInput, out var resultMessage))
                    {
                        return resultMessage;
                    }
                }
                return $"No overload found for your request";
            }
            else
            {
                return $"Unknown command \"{methodName}\"";
            }
        }

        private static string NormalizeMethodName(string methodName)
        {
            return methodName.ToLower();
        }

        private static ILookup<string, MethodInfo> GetCommandMethods()
        {
            var result = typeof(CommandProcessor)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                .Where(IsCommandMethod)
                .ToLookup(x => NormalizeMethodName(x.Name));

            return result;
        }

        private static bool IsCommandMethod(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttribute<CommandMethodAttribute>() != null
                && methodInfo.ReturnType == typeof(string);
        }

        private bool TryCallCommandMethod(MethodInfo methodInfo, string[] input, out string resultMessage)
        {
            resultMessage = string.Empty;

            var parameters = methodInfo.GetParameters();
            var paramValues = new object[parameters.Length];

            if (input.Length > parameters.Length)
            {
                return false;
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                if (i < input.Length)
                {
                    var type = parameters[i].ParameterType;
                    if (!TryParseParam(type, input[i], out paramValues[i]))
                    {
                        return false;
                    }
                }
                else if (parameters[i].DefaultValue != null)
                {
                    paramValues[i] = parameters[i].DefaultValue;
                }
                else
                {
                    return false;
                }
            }

            var result = methodInfo.Invoke(this, paramValues);
            resultMessage = (string)result;
            return true;
        }

        private static bool TryParseParam(Type type, string input, out object result)
        {
            result = null;
            try
            {
                switch (type.Name)
                {
                    case nameof(String):
                        result = input;
                        break;
                    case nameof(Byte):
                        result = Convert.ToByte(input);
                        break;
                    default:
                        return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
