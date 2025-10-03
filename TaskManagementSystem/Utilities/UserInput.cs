using System.Globalization;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Utilities
{
    public static class UserInput
    {

        private static T GetInput<T>
            (
            string prompt,
            Func<string, (bool success, T value)> parser,
            Func<T, (bool success, string errorMessage)>? validator = null,
            bool isRequired = false,
            T? defaultValue = default,
            string parsingErrorMessage = "Invalid format."
            ) where T : struct
        {
            while (true)
            {
                Console.Write(prompt);
                string userInput = Console.ReadLine()?.Trim() ?? string.Empty;

                if (!isRequired && string.IsNullOrEmpty(userInput) && defaultValue != null)
                {
                    return defaultValue.Value;
                }

                if (isRequired && string.IsNullOrEmpty(userInput))
                {
                    ConsoleUI.ErrorMessage("You can't leave this field empty.");
                    continue;
                }

                var (parsedSuccess, parsedValue) = parser(userInput);

                if (!parsedSuccess)
                {
                    ConsoleUI.ErrorMessage(parsingErrorMessage);
                    continue;
                }

                if (validator != null)
                {
                    var (validationSuccess, validationErrorMessage) = validator(parsedValue);
                    if (!validationSuccess)
                    {
                        ConsoleUI.ErrorMessage(validationErrorMessage);
                        continue;
                    }
                }
                return parsedValue;
            }
        }

        public static string GetString(string prompt, bool isRequired = false, string? defaultValue = null, ICollection<string>? options = null)
        {
            while (true)
            {
                Console.Write(prompt);
                string userInput = Console.ReadLine()?.Trim() ?? string.Empty;

                if (!string.IsNullOrEmpty(defaultValue) && userInput.Length == 0)
                {
                    return defaultValue;
                }

                if (isRequired && userInput.Length == 0)
                {
                    ConsoleUI.ErrorMessage("You can't leave this field empty.");
                    continue;
                }

                if (options != null && options.Any())
                {
                    bool success = options.Any(opt => opt.Equals(userInput, StringComparison.OrdinalIgnoreCase));

                    if (!success)
                    {
                        ConsoleUI.ErrorMessage("Please select only in the options");
                        continue;
                    }


                }

                return userInput;
            }
        }

        public static int GetInt(string prompt, bool isRequired = false, ICollection<int>? options = null)
        {
            var intParser = (string userInput) =>
            {
                bool success = int.TryParse(userInput, out int result);
                return (success, result);
            };


            if (options != null && options.Any())
            {
                var intOptionsValidator = (int parsedValue) =>
                {
                    bool success = options.Contains(parsedValue);
                    string errorMessage = success ? "" : "Please select only in the options";
                    return (success, errorMessage);
                };

                return GetInput(prompt, intParser, validator: intOptionsValidator, isRequired, parsingErrorMessage: "Invalid input, please only enter numbers.");
            }
            else
            {
                return GetInput(prompt, intParser, validator: null, isRequired, parsingErrorMessage: "Invalid input, please only enter numbers.");
            }
        }

        public static DateTime GetDateTime(string prompt, string dateFormat, DateTime? defaultValue = null)
        {
            var dateTimeParser = (string userInput) =>
            {
                if (string.IsNullOrEmpty(userInput))
                {
                    return (true, DateTime.MinValue);
                }

                bool success = DateTime.TryParseExact(userInput, dateFormat, null, DateTimeStyles.None, out DateTime result);
                return (success, result);
            };

            return GetInput(prompt, dateTimeParser, validator: null, isRequired: false, defaultValue, parsingErrorMessage: "Invalid date format, please try again.");
        }

        public static PriorityLevel GetPriority(string prompt, bool isRequired = false, PriorityLevel? defaultValue = null)
        {
            var priorityParser = (string userInput) =>
            {
                bool success = Enum.TryParse(userInput, true, out PriorityLevel result);
                return (success, result);
            };

            var priortyValidator = (PriorityLevel parsedValue) =>
            {
                bool success = Enum.IsDefined(typeof(PriorityLevel), parsedValue);
                string errorMessage = success ? "" : $"'{parsedValue}' is not a valid priority level";
                return (success, errorMessage);
            };

            return GetInput(prompt, priorityParser, priortyValidator, isRequired, parsingErrorMessage: "Invalid input, please select the exact priority.");
        }

    }
}
