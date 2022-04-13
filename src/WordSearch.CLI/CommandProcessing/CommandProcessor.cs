using System.Reflection;
using System.Text;
using WordSearch.CLI.Exceptions;

namespace WordSearch.CLI.CommandProcessing
{
    public class CommandProcessor
    {
        private static readonly IReadOnlyDictionary<Type, Func<string, object>> parsers = GetParsers();

        private readonly ILookup<string, MethodInfo> commandMethods;
        private readonly object commandContainer;

        public CommandProcessor(object commandContainer)
        {
            commandMethods = GetCommandMethods(commandContainer.GetType());
            this.commandContainer = commandContainer;
        }

        public string Process(string input)
        {
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var commandName = NormalizeCommandName(parts[0]);

            if (commandName == NormalizeCommandName("help"))
            {
                return ShowHelp();
            }

            if (commandMethods.Contains(commandName))
            {
                var methodOverloads = commandMethods[commandName];
                var trimmedInput = parts.Skip(1).ToArray();

                foreach (var overload in methodOverloads)
                {
                    if (TryCallCommandMethod(overload, trimmedInput, out var result))
                    {
                        return result;
                    }
                }
                throw new CommandOverloadNotFoundException(commandName);
            }
            else
            {
                throw new CommandNotFoundException(commandName);
            }
        }

        public string ShowHelp()
        {
            var stringBuilder = new StringBuilder();
            foreach (var overloads in commandMethods)
            {
                stringBuilder.AppendLine(overloads.Key);
                foreach (var method in overloads)
                {
                    stringBuilder.Append('\t').AppendLine(GetMethodDescription(method));
                }
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }

        private static string GetMethodDescription(MethodInfo methodInfo)
        {
            var paramsDescription = string.Join(' ', methodInfo.GetParameters()
                .Select(x => $"{x.Name}:{x.ParameterType.Name}"));

            return $"{methodInfo.Name} {paramsDescription}";
        }

        private static string NormalizeCommandName(string commandName)
        {
            return commandName.ToLower();
        }

        private static ILookup<string, MethodInfo> GetCommandMethods(Type containerType)
        {
            var result = containerType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Where(IsCommandMethod)
                .ToLookup(x => NormalizeCommandName(x.Name));

            return result;
        }

        private static bool IsCommandMethod(MethodInfo method)
        {
            var result = method.ReturnType == typeof(string) 
                && method.Name != nameof(Object.ToString);

            return result;
        }

        private bool TryCallCommandMethod(MethodInfo methodInfo, string[] input, out string result)
        {
            result = null;

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
                else if (parameters[i].HasDefaultValue)
                {
                    paramValues[i] = parameters[i].DefaultValue;
                }
                else
                {
                    return false;
                }
            }

            result = (string)methodInfo.Invoke(commandContainer, paramValues);
            return true;
        }

        private static bool TryParseParam(Type type, string input, out object result)
        {
            result = null;
            if (parsers.TryGetValue(type, out var parser))
            {
                try
                {
                    result = parser(input);
                    return true;
                }
                catch (Exception) { }
            }
            return false;
        }

        private static IReadOnlyDictionary<Type, Func<string, object>> GetParsers()
        {
            var result = new Dictionary<Type, Func<string, object>>();
            AddParser(result, Convert.ToString);
            AddParser(result, Convert.ToByte);
            AddParser(result, Convert.ToSByte);
            AddParser(result, Convert.ToBoolean);
            AddParser(result, Convert.ToChar);
            AddParser(result, Convert.ToInt16);
            AddParser(result, Convert.ToUInt16);
            AddParser(result, Convert.ToInt32);
            AddParser(result, Convert.ToUInt32);
            AddParser(result, Convert.ToInt64);
            AddParser(result, Convert.ToUInt64);
            AddParser(result, Convert.ToSingle);
            AddParser(result, Convert.ToDouble);
            AddParser(result, Convert.ToDecimal);
            return result;
        }

        private static void AddParser<T>(
            Dictionary<Type, Func<string, object>> parsers,
            Func<string, T> parser)
        {
            parsers.Add(typeof(T), x => parser(x));
        }
    }
}
