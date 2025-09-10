using System.Globalization;
using TaskManagementSystem.Models;
using TaskManagementSystem.Service;

namespace TaskManagementSystem.Utilities
{
    public class UserInput
    {
        private readonly ITaskService _taskService;

        public UserInput(ITaskService taskService)
        {
            _taskService = taskService;
        }

        private T GetInput<T>(string prompt, Func<string, (bool success, T value)> parser, bool isRequired = false, T? defaultValue = default, string errorMessage = "Invalid format.") where T : struct
        {
            while (true)
            {
                Console.Write(prompt);
                string userInput = Console.ReadLine()?.Trim() ?? string.Empty;

                if (defaultValue.HasValue && userInput.Length == 0)
                {
                    return defaultValue.Value;
                }

                if (isRequired && userInput.Length == 0)
                {
                    ConsoleUI.ErrorMessage("You can't leave this field empty.");
                    continue;
                }

                var (success, value) = parser(userInput);
                if (success) return value;
                else ConsoleUI.ErrorMessage(errorMessage);
            }
        }

        public string GetString(string prompt, bool isRequired = false, string? defaultValue = null)
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

                return userInput;
            }
        }

        public int GetInt(string prompt, bool isRequired = false)
        {
            var intParser = (string userInput) =>
            {
                bool success = int.TryParse(userInput, out int result);
                return (success, result);
            };

            return GetInput(prompt, intParser, isRequired, errorMessage: "Invalid input, please only enter numbers.");
        }

        public DateTime GetDateTime(string prompt, string dateFormat, DateTime? defaultValue = null)
        {
            var dateTimeParser = (string userInput) =>
            {
                bool success = DateTime.TryParseExact(userInput, dateFormat, null, DateTimeStyles.None, out DateTime result);
                return (success, result);
            };

            return GetInput(prompt, dateTimeParser, false, defaultValue, errorMessage: "Invalid date format, please try again.");
        }

        public PriorityLevel GetPriority(string prompt, bool isRequired = false, PriorityLevel? defaultValue = null)
        {
            var priorityParser = (string userInput) =>
            {
                bool success = Enum.TryParse(userInput, out PriorityLevel result);
                return (success, result);
            };

            return GetInput(prompt, priorityParser, isRequired, errorMessage: "Invalid input, please select the exact priority.");
        }

    }
}
