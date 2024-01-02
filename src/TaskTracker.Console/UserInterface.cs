using TaskTracker.Console.Services;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Console;

public class UserInterface(TaskManager taskManager)
{
    private readonly WriteLineCentered _menuWidthWriter = new(12);
    private readonly WriteLineCentered _showTasksWriter = new(16);
    private readonly ISystemDescriptor _systemDescriptor = OSDetector.GetOSInfo();
    private readonly TaskManager _taskManager = taskManager;

    public async Task RunAsync()
    {
        while (true)
        {
            ShowMenu();
            string choiceRaw = Terminal.ReadLine() ?? string.Empty;
            _ = Enum.TryParse(choiceRaw, out TaskChoice choice);

            await ChoiceHanler(choice);
        }
    }

    private async Task ChoiceHanler(TaskChoice choice)
    {
        switch (choice)
        {
            case TaskChoice.Add:
                AddTask();
                break;
            case TaskChoice.View:
                await ViewTasks();
                break;
            case TaskChoice.Remove:
                await RemoveTask();
                break;
            case TaskChoice.MarkAsDone:
                await MarkTaskAsDone();
                break;
            case TaskChoice.Exit:
                Exit();
                break;
            default:
                _menuWidthWriter.WriteLine("Invalid choice. Please enter a valid choice.");
                break;
        }
    }

    private void ShowMenu()
    {
        Terminal.Clear();
        _menuWidthWriter.WriteLine(
            $"{_systemDescriptor.GetOSEmoji}  Welcome to the Todo List App! {_systemDescriptor.GetOSEmoji}\n");

        _menuWidthWriter.WriteLine("===================================");
        _menuWidthWriter.WriteLine($"Operacional System: {_systemDescriptor.GetOSName} {_systemDescriptor.GetOSEmoji}\n");
        _menuWidthWriter.WriteLine("1. Add Task");
        _menuWidthWriter.WriteLine("2. Remove Task");
        _menuWidthWriter.WriteLine("3. Mark Task as Completed");
        _menuWidthWriter.WriteLine("4. View Tasks");
        _menuWidthWriter.WriteLine("5. Exit");
        _menuWidthWriter.WriteLine("===================================\n");

        _menuWidthWriter.Write("Enter your choice (1-5): ");
    }

    private async void AddTask()
    {
        _menuWidthWriter.Write("Enter task description: ");
        string description = Terminal.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(description))
        {
            _menuWidthWriter.WriteLine("Invalid description. Please enter a valid description.");
            AddTask();
            return;
        }

        try
        {
            await _taskManager.AddTask(new TaskItem(description));
            Terminal.WriteLine("");
            _menuWidthWriter.WriteLine("Task added successfully!");
            Terminal.ReadKey();
        }
        catch (Exception error)
        {
            Terminal.WriteLine(error.Message);
            Terminal.WriteLine(error.StackTrace);
        }
    }

    private async Task MarkTaskAsDone()
    {
        _menuWidthWriter.Write("Enter task id: ");
        string id = Terminal.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(id))
        {
            _menuWidthWriter.WriteLine("Invalid id. Please enter a valid id.");
            await MarkTaskAsDone();
            return;
        }

        try
        {
            await _taskManager.MarkTaskAsDone(int.Parse(id));
            _menuWidthWriter.WriteLine("Task marked as done successfully!");
            Terminal.ReadKey();
        }
        catch (Exception error)
        {
            Terminal.WriteLine(error.Message);
            Terminal.WriteLine(error.StackTrace);
        }
    }

    private async Task ViewTasks()
    {
        try
        {
            IEnumerable<TaskItem> tasks = await _taskManager.GetAllTasks();

            Terminal.Clear();
            _showTasksWriter.WriteLine("=================================================\n");
            foreach (TaskItem task in tasks)
            {
                _showTasksWriter.WriteLine($"Id: {task.Id}");
                _showTasksWriter.WriteLine($"Description: {task.Description}");
                _showTasksWriter.WriteLine($"IsComplete: {task.IsComplete}");
                _showTasksWriter.WriteLine("=================================================\n");
            }
            Terminal.ReadKey();
        }
        catch (Exception error)
        {
            Terminal.WriteLine(error.Message);
            Terminal.WriteLine(error.StackTrace);
        }
    }

    private async Task RemoveTask()
    {
        _menuWidthWriter.Write("Enter task id: ");
        string id = Terminal.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(id))
        {
            _menuWidthWriter.WriteLine("Invalid id. Please enter a valid id.");
            await _taskManager.RemoveTask(int.Parse(id));
            return;
        }

        try
        {
            await _taskManager.RemoveTask(int.Parse(id));
            _menuWidthWriter.WriteLine("Task removed successfully!");
                        Terminal.ReadKey();
        }
        catch (Exception error)
        {
            Terminal.WriteLine(error.Message);
            Terminal.WriteLine(error.StackTrace);
        }
    }

    private void Exit()
    {
        _menuWidthWriter.WriteLine("Thank you for using the Todo List App!");
        Terminal.Clear();
        Environment.Exit(0);
    }

}