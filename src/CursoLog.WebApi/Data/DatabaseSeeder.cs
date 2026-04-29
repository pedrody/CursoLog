using CursoLog.WebApi.Constants;
using CursoLog.WebApi.Models;
using CursoLog.WebApi.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace CursoLog.WebApi.Data;

public static class DatabaseSeeder
{
    private static List<Category> categories;
    private static List<Technology> technologies;
    private static List<Platform> platforms;
    private static List<Instructor> instructors;
    private static List<Course> courses;
    private static List<ApplicationUser> users;

    static DatabaseSeeder()
    {
        categories = new List<Category>
        {
            new("Development"),
            new("Data Science"),
            new("Design"),
            new("Marketing"),
            new("Soft Skills")
        };

        technologies = new List<Technology>
        {
            new("C#", TechnologyType.Language),
            new("Java", TechnologyType.Language),
            new("Python", TechnologyType.Language),
            new("JavaScript", TechnologyType.Language),
            new("React", TechnologyType.Framework),
            new("Angular", TechnologyType.Framework),
            new("ASP.NET Core", TechnologyType.Framework),
            new("PostgreSQL", TechnologyType.Database),
            new("AWS", TechnologyType.Cloud),
            new("Docker", TechnologyType.Tools)
        };

        platforms = new List<Platform>
        {
            new("Udemy", "https://www.udemy.com"),
            new("Coursera", "https://www.coursera.org"),
            new("Alura", "https://www.alura.com.br"),
            new("YouTube", "https://www.youtube.com"),
            new("Curso em Vídeo", "https://www.cursoemvideo.com")
        };

        instructors = new List<Instructor>
        {
            new("José Carlos Macoratti",
                "Profissional com mais de 20 anos de experiência em tecnologia, atuando em soluções desktop, web e mobile. Fundador do portal Macoratti, referência nacional em conteúdos técnicos. MVP Microsoft desde 2009, possui sólidos conhecimentos em diversas linguagens e boas práticas de desenvolvimento orientado a objetos.",
                "https://macoratti.com.br"),
            new("Luiz Otávio Miranda",
                "Luiz Otávio Miranda é desenvolvedor desde 2009, especialista em JavaScript, TypeScript e Python. Instrutor com mais de 600 horas de conteúdo e 300 mil alunos, atua com projetos full stack, DevOps e redes. Atualmente dedica-se integralmente ao ensino com foco prático e objetivo.",
                "https://otaviomiranda.com.br/"),
            new("Nelio Alves",
                "Nelio Alves é doutor em Engenharia de Software e professor desde 2001, com atuação em ensino superior, técnico e online. Oferece cursos em programação, Excel e modelagem de dados, com foco em fundamentos, prática e projetos, visando a formação profissional na área de tecnologia.",
                "https://devsuperior.com.br/"),
            new("Gustavo Guanabara",
                "Gustavo Guanabara é professor desde 1994, com experiência em diversos níveis de ensino, do infantil ao superior. Já atuou em cursos técnicos, universidades e na coordenação de tutoria em ensino a distância. Atualmente, realiza palestras em eventos nacionais e internacionais na área de tecnologia.",
                "https://www.cursoemvideo.com/")
        };

        courses = new List<Course>
        {
            new(
                "Curso C# Essencial (.NET 9.0, LINQ e IA)",
                "Formação completa em C# moderno com .NET 9, LINQ e Inteligência Artificial",
                "https://www.udemy.com/course/curso-c-essencial-2023-bonus-linq",
                CoursePriceRange.UnderOneHundred,
                CourseDurationRange.UnderFiftyHours,
                CourseLanguage.Portuguese,
                null,
                instructors[0].Id,
                platforms[0].Id,
                categories[0].Id
            ),
            new(
                "Curso Web API ASP .NET Core Essencial (.NET 8 / .NET 9)",
                "O essencial para criar APIs da Web na plataforma .NET",
                "https://www.udemy.com/course/curso-web-api-asp-net-core-essencial",
                CoursePriceRange.AboveOneHundred,
                CourseDurationRange.UnderFiftyHours,
                CourseLanguage.Portuguese,
                null,
                instructors[0].Id,
                platforms[0].Id,
                categories[0].Id
            ),
            new(
                "Curso de Python 3 do básico ao avançado - com projetos reais",
                "Python 3+ completo: PySide6, Django, Selenium, Regexp, Testes, TDD, POO, Design Patterns GoF, algoritmos e programação",
                "https://www.udemy.com/course/python-3-do-zero-ao-avancado",
                CoursePriceRange.AboveOneHundred,
                CourseDurationRange.AboveOneHundredHours,
                CourseLanguage.Portuguese,
                null,
                instructors[1].Id,
                platforms[0].Id,
                categories[0].Id
            ),
            new(
                "Java COMPLETO Programação Orientada a Objetos + Projetos",
                "Curso mais didático e completo de Java e OO, UML, JDBC, JavaFX, Spring Boot, JPA, Hibernate, MySQL, MongoDB e muito mais",
                "https://www.udemy.com/course/java-curso-completo",
                CoursePriceRange.AboveOneHundred,
                CourseDurationRange.UnderOneHundredHours,
                CourseLanguage.Portuguese,
                null,
                instructors[2].Id,
                platforms[0].Id,
                categories[0].Id
            ),
            new(
                "Algoritmos e Lógica de Programação",
                "Curso introdutório de algoritmos e lógica de programação, base para aprender linguagens como C, Java e PHP. Apresenta conceitos essenciais para criação de programas de forma simples e prática",
                "https://www.cursoemvideo.com/curso/curso-de-algoritmo/",
                CoursePriceRange.Free,
                CourseDurationRange.UnderFiftyHours,
                CourseLanguage.Portuguese,
                null,
                instructors[3].Id,
                platforms[4].Id,
                categories[0].Id
            )
        };

        users = new List<ApplicationUser>
        {
            new() { UserName = "pedro", Email = "pedro@email.com", Name = "Pedro", RegisteredAt = DateTime.UtcNow, EmailConfirmed = true },
            new() { UserName = "guilherme", Email = "guilherme@email.com", Name = "Guilherme", RegisteredAt = DateTime.UtcNow, EmailConfirmed = true }
        };
    }
    public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        string[] roles = ["Admin", "User"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }

    public static async Task SeedUserAdminAsync(UserManager<ApplicationUser> userManager)
    {
        var adminEmail = "admin@email.com";
        var adminPassword = "Password@123";
        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

        if (existingAdmin is not null) return;

        var admin = new ApplicationUser
        {
            UserName = "admin",
            Email = adminEmail,
            EmailConfirmed = true,
            Name = "Administrator",
            RegisteredAt = DateTime.UtcNow
        };

        await userManager.CreateAsync(admin, adminPassword);
        await userManager.AddToRoleAsync(admin, Roles.Admin);
    }

    public static async Task SeedCategoriesAsync(AppDbContext context)
    {
        if (context.Categories.Any()) return;
        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }

    public static async Task SeedTechnologiesAsync(AppDbContext context)
    {
        if (context.Technologies.Any()) return;
        await context.Technologies.AddRangeAsync(technologies);
        await context.SaveChangesAsync();
    }

    public static async Task SeedPlatformsAsync(AppDbContext context)
    {
        if (context.Platforms.Any()) return;
        await context.Platforms.AddRangeAsync(platforms);
        await context.SaveChangesAsync();
    }

    public static async Task SeedInstructorsAsync(AppDbContext context)
    {
        if (context.Instructors.Any()) return;
        await context.Instructors.AddRangeAsync(instructors);
        await context.SaveChangesAsync();
    }

    public static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        foreach (var user in users)
        {
            var userExists = await userManager.FindByEmailAsync(user.Email!);
            if (userExists is null)
                await userManager.CreateAsync(user, "Password@123");
        }
    }

    public static async Task SeedCoursesAsync(AppDbContext context)
    {
        if (context.Courses.Any()) return;

        courses[0].Technologies.Add(technologies[0]);
        courses[1].Technologies.Add(technologies[0]);
        courses[1].Technologies.Add(technologies[6]);
        courses[2].Technologies.Add(technologies[2]);
        courses[3].Technologies.Add(technologies[1]);

        await context.Courses.AddRangeAsync(courses);
        await context.SaveChangesAsync();
    }

    public static async Task SeedReviewsAsync(AppDbContext context)
    {
        if (context.Reviews.Any()) return;

        var reviews = new List<Review>
        {
            new(courses[0].Id, 5.0m, "Excelente curso!", users[0].Id),
            new(courses[0].Id, 4.5m, "Muito bom, bem didático.", users[1].Id),
            new(courses[1].Id, 4.0m, "Gostei bastante.", users[0].Id),
            new(courses[2].Id, 5.0m, "O melhor curso de Python.", users[1].Id),
            new(courses[3].Id, 5.0m, "Gostei da didática do professor.", users[0].Id),
            new(courses[4].Id, 5.0m, "Melhor curso para introdução à algoritmos", users[1].Id),
        };
        await context.Reviews.AddRangeAsync(reviews);

        await context.SaveChangesAsync();
    }

    public static async Task SeedInitialDataAsync(
        AppDbContext context, 
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        await SeedRolesAsync(roleManager);
        await SeedUserAdminAsync(userManager);
        await SeedCategoriesAsync(context);
        await SeedTechnologiesAsync(context);
        await SeedPlatformsAsync(context);
        await SeedInstructorsAsync(context);
        await SeedUsersAsync(userManager);
        await SeedCoursesAsync(context);
        await SeedReviewsAsync(context);
    }
}
