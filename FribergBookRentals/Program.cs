using AutoMapper;
using FribergbookRentals.Data.Constants;
using FribergbookRentals.Data.Dto;
using FribergbookRentals.Data.Models;
using FribergbookRentals.Data.Repositories;
using FribergBookRentals.Data;
using FribergBookRentals.Mapper;
using FribergBookRentals.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Text.Json;

namespace FribergBookRentals
{
    public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			
			// Database
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			// Repositories
			builder.Services.AddTransient<IBookLoanRepository, BookLoanRepository>();
            builder.Services.AddTransient<IBookRepository, BookRepository>();

			// Services
			builder.Services.AddScoped<ITempDataHelper, TempDataHelper>();

			// Identity
			builder.Services.AddDefaultIdentity<User>(options =>
			{
				options.SignIn.RequireConfirmedAccount = true;
				options.User.RequireUniqueEmail = true;
				options.SignIn.RequireConfirmedEmail = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireUppercase = true;
				options.Password.RequireDigit = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequiredLength = 8;
			})
			//.AddDefaultTokenProviders()
			.AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>();

			//builder.Services.AddAuthorizationCore(options =>
			//{
			//	options.AddPolicy(ApplicationPolicies.Member, policy =>
			//		policy.RequireClaim(ApplicationUserClaims.UserRole, ApplicationUserRoles.Member));
			//});
			
            // Automapper
            builder.Services.AddAutoMapper(typeof(EntityToViewModelAutoMapperProfile), typeof(ViewModelToEntityMapperProfile));

            builder.Services.AddControllersWithViews();
			//builder.Services.AddRazorPages();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
			app.MapRazorPages();

            // ==================================================================================================================
            // Migration and seeding
            // ==================================================================================================================
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();

#if DEBUG
                if (!context.Books.Any())
                {
                    var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "BookList.txt"));
                    var seedbooks = JsonSerializer.Deserialize<List<SeedBookDto>>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
					var mapper = services.GetRequiredService<IMapper>();
					var books = mapper.Map<List<Book>>(seedbooks);

                    IBookRepository bookRepository = new BookRepository(context);
                    bookRepository.AddBooksAsync(books!).Wait();
                }
#endif
            }

            app.Run();
		}
	}
}
