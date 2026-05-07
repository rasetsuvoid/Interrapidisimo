using Interrapidisimo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Interrapidisimo.Infrastructure.Persistence.Seeders;

public class DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
{
    public async Task SeedAsync()
    {
        await context.Database.MigrateAsync();

        if (await context.Professors.AnyAsync()) return;

        logger.LogInformation("Inicializando datos de la base de datos...");

        var professors = new[]
        {
            Professor.Create("Carlos", "Ramírez", "c.ramirez@university.edu", "PhD en Ingeniería de Software", "system"),
            Professor.Create("Ana", "González", "a.gonzalez@university.edu", "MSc en Ciencias de la Computación", "system"),
            Professor.Create("Luis", "Martínez", "l.martinez@university.edu", "PhD en Matemáticas Aplicadas", "system"),
            Professor.Create("María", "Rodríguez", "m.rodriguez@university.edu", "MSc en Bases de Datos", "system"),
            Professor.Create("Jorge", "Hernández", "j.hernandez@university.edu", "PhD en Redes y Sistemas Distribuidos", "system"),
        };

        await context.Professors.AddRangeAsync(professors);
        await context.SaveChangesAsync();

        var subjects = new[]
        {
            Subject.Create("Programación Orientada a Objetos", "POO-101", professors[0].Id, "Fundamentos de OOP con Java y C#", "system"),
            Subject.Create("Patrones de Diseño", "PAT-201", professors[0].Id, "GoF patterns y arquitecturas modernas", "system"),
            Subject.Create("Estructuras de Datos", "EDA-101", professors[1].Id, "Árboles, grafos, tablas hash", "system"),
            Subject.Create("Algoritmos Avanzados", "ALG-301", professors[1].Id, "Complejidad, backtracking, DP", "system"),
            Subject.Create("Cálculo Diferencial", "CAL-101", professors[2].Id, "Límites, derivadas e integrales", "system"),
            Subject.Create("Álgebra Lineal", "ALG-102", professors[2].Id, "Matrices, vectores y transformaciones", "system"),
            Subject.Create("Bases de Datos I", "BD-101", professors[3].Id, "Modelo relacional y SQL", "system"),
            Subject.Create("Bases de Datos II", "BD-201", professors[3].Id, "NoSQL, optimización y tuning", "system"),
            Subject.Create("Redes de Computadores", "RED-101", professors[4].Id, "Modelo OSI, TCP/IP, protocolos", "system"),
            Subject.Create("Seguridad Informática", "SEG-201", professors[4].Id, "Criptografía, OWASP y hacking ético", "system"),
        };

        await context.Subjects.AddRangeAsync(subjects);
        await context.SaveChangesAsync();

        logger.LogInformation("Base de datos inicializada. {ProfCount} profesores, {SubjCount} materias.",
            professors.Length, subjects.Length);
    }
}
