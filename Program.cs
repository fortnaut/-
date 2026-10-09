using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32.SafeHandles;
using Npgsql;
class kosmos
{
    private const string ConnectionString = "\"Host=wotacoma.ru;Port=5090;Username=fortnaut;Password=CROSS;Database=fortnaut";

    
    static async Task Main(string[] args)
    {

        while (true)
        {
            Console.WriteLine("\n--- Меню ---");
            Console.WriteLine("1. Показать всех экипажей");
            Console.WriteLine("2. Выполнить запрос");
            Console.WriteLine("3. Изменить здоровье");
            Console.WriteLine("4. Выход");
            Console.WriteLine("Выберите пункт:  ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ShowCrewAsync();
                    break;
                case "2":
                    await SearchCrewAsync();
                    break;
                case "3":
                    await UpdateHealthAsync();
                    break;
                case "4":
                    return;

                default:
                    Console.WriteLine("Неверный ввод");
                    break;
            }

        }
    }

    static async Task ShowCrewAsync()
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        string sql = "SELECT id, name, role, health, status";
        using var command = connection.CreateCommand();

        using var reader = await command.ExecuteReaderAsync();

        Console.WriteLine(new string('-', 65));
        Console.WriteLine($"| {"ID",-3} | {"Имя",-18} | {"Роль",-12} | {"Здоровье",-9} | {"Статус",-10}");
        Console.WriteLine(new string('-', 65));

        while (await reader.ReadAsync())
        {
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            string role = reader.GetString(2);
            int health = reader.GetInt32(3);
            string statys = reader.GetString(4);

            Console.WriteLine($"| {id,-3} | {name,-18} | {role,-12} | {health,-9} | {statys,-10}");
        }
        Console.WriteLine(new string('-', 65));
    }
    static async Task SearchCrewAsync()
    {
        Console.WriteLine("Введите имя: ");
        string searchName = Console.ReadLine();

        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        string sql = "SELECT id, name, role, health, status";
        using var command = connection.CreateCommand();

        command.Parameters.AddWithValue("@name", $"%{searchName}%");

        using var reader = await command.ExecuteReaderAsync();

        Console.WriteLine(new string('-', 65));
        Console.WriteLine($"| {"ID",-3} | {"Имя",-18} | {"Роль",-12} | {"Здоровье",-9} | {"Статус",-10}");
        Console.WriteLine(new string('-', 65));

        bool found = false;
        while (await reader.ReadAsync())
        {
            found = true;
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            string role = reader.GetString(2);
            int health = reader.GetInt32(3);
            string statys = reader.GetString(4);

            Console.WriteLine($"| {id,-3} | {name,-18} | {role,-12} | {health,-9} | {statys,-10}");
        }
        if (found)
        {
            Console.WriteLine("Члены экипажа не найдены. ");
        }
        Console.WriteLine(new string('-', 65));
    }

    static async Task UpdateHealthAsync()
    {
        Console.WriteLine("Введите ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Некорректный ID,");
            return;
        }
        Console.WriteLine("Введите урон:");
        if (!int.TryParse(Console.ReadLine(), out int damage))
        {
            Console.WriteLine("Некорректное значение урона.");
            return;
        }
        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        string sql = "UPDATE crew SET healtch = health - @damage WHERE id = @id;";
        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@damage", damage);
        command.Parameters.AddWithValue("@id", id);

        int rowsAffected = await command.ExecuteNonQueryAsync();

        if (rowsAffected > 0)
        {
            Console.WriteLine($"Здоровье членов экипажа с Id {id} успешно обновлено.");
        }
        else
        {
            Console.WriteLine($"Член экипажа с ID {id} не найден.");
        }
    }
}
