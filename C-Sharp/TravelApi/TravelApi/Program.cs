using TravelApi.Repository;
using Microsoft.EntityFrameworkCore;
using TravelApi.Services;

namespace TravelApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // CORS配置
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.SetIsOriginAllowed(_ => true)  // 修改这里
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials();
                    });
            });

            // Swagger配置
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // 数据库配置
            String? connectionString = builder.Configuration.GetConnectionString("travelDB");
            builder.Services.AddDbContext<TravelContext>(opt => opt.UseMySQL(connectionString));

            // 服务注册
            builder.Services.AddScoped<ITravelService, TravelService>();
            builder.Services.AddScoped<IJournalService, JournalService>();
            builder.Services.AddScoped<ITodoItemService, TodoItemService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddHttpClient<PythonService>();
            builder.Services.AddScoped<IChatService, ChatService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // 删除HTTPS重定向
            // app.UseHttpsRedirection();

            // 添加CORS中间件 - 注意这个位置
            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}