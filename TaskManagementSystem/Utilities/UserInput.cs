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

        public T GetInput<T>(string prompt, Func<string, (bool success, T value)> parser, bool isRequired = false, string errorMessage = "Invalid format.")
        {
            while (true)
            {
                Console.Write(prompt);
                string userInput = Console.ReadLine()?.Trim() ?? string.Empty;

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

        public int GetIntInput(string prompt, bool isRequired = false)
        {
            var intParser = (string userInput) =>
            {
                bool success = int.TryParse(userInput, out int result);
                return (success, result);
            };

            return GetInput<int>(prompt, intParser, isRequired, errorMessage: "Invalid input, please only enter numbers.");
        }
    }
}
