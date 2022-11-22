using System.Collections.Generic;
using BookingRentals.Booking;
using BookingRentals.Calendar;
using BookingRentals.Rental;
using MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using Swashbuckle.AspNetCore.Swagger;
using VacationRental.Api.Models;
using VacationRental.Booking;
using VacationRental.Calendar;
using VacationRental.Rental;

namespace VacationRental.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(opts => opts.SwaggerDoc("v1", new Info { Title = "Vacation rental information", Version = "v1" }));

            services.AddSingleton<IRepository<IdentifiedRentalItem>,InMemoryRepository<IdentifiedRentalItem>>();
            services.AddSingleton<IRepository<IdentifiedBookingItem>, InMemoryRepository<IdentifiedBookingItem>>();
            services.AddSingleton<IRepository<IdentifiedCalendarBookingItem>, InMemoryRepository<IdentifiedCalendarBookingItem>>();
            services.AddSingleton<ICalendar>(opt => new CalendarService(opt.GetService<IRepository<IdentifiedCalendarBookingItem>>()));
            services.AddSingleton<IRental>(opt => new RentalService(opt.GetService<IRepository<IdentifiedRentalItem>>()));
            services.AddSingleton<IBooking>(opt => new BookingService(opt.GetService<IRepository<IdentifiedBookingItem>>()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(opts => opts.SwaggerEndpoint("/swagger/v1/swagger.json", "VacationRental v1"));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            
        }
    }
}
