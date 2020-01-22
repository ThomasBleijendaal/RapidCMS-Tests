using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RapidCMS.Core.Enums;
using RapidCMSTests.EFCore;
using RapidCMSTests.Models.Cms;
using RapidCMSTests.Repositories;

namespace RapidCMSTests
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddDbContext<TestDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Db"));
            }, ServiceLifetime.Transient, ServiceLifetime.Transient);

            services.AddTransient<PersonRepository>();
            services.AddTransient<CountryRepository>();

            services.AddRapidCMS(config =>
            {
                config.AllowAnonymousUser();

                config.AddCollection<PersonCmsModel, PersonRepository>("people", "People", people =>
                {
                    people.AddSelfAsRecursiveCollection();

                    people.SetTreeView(x => x.Name);

                    people.SetListView(view =>
                    {
                        view.AddDefaultButton(DefaultButtonType.New);

                        view.AddRow(row =>
                        {
                            row.AddDefaultButton(DefaultButtonType.Edit);
                            row.AddDefaultButton(DefaultButtonType.Delete);

                            row.AddField(x => x.Name);
                        });
                    });

                    people.SetNodeEditor(editor =>
                    {
                        editor.AddDefaultButton(DefaultButtonType.SaveExisting);
                        editor.AddDefaultButton(DefaultButtonType.SaveNew);
                        editor.AddDefaultButton(DefaultButtonType.Delete);

                        editor.AddSection(section =>
                        {
                            section.AddField(x => x.Name);

                            section.AddRelatedCollectionList<CountryCmsModel, CountryRepository>(list =>
                            {
                                list.SetListEditor(editor =>
                                {
                                    editor.AddDefaultButton(DefaultButtonType.New);
                                    editor.AddDefaultButton(DefaultButtonType.Add);
                                    editor.AddDefaultButton(DefaultButtonType.Return);

                                    editor.AddSection(row =>
                                    {
                                        row.AddDefaultButton(DefaultButtonType.SaveExisting);
                                        row.AddDefaultButton(DefaultButtonType.SaveNew);
                                        row.AddDefaultButton(DefaultButtonType.Pick);
                                        row.AddDefaultButton(DefaultButtonType.Remove);

                                        row.AddField(x => x.Name);
                                    });
                                });
                            });
                        });
                    });
                });

                config.AddCollection<CountryCmsModel, CountryRepository>("countries", "Countries", countries =>
                {
                    countries.SetTreeView(x => x.Name);

                    countries.SetListView(view =>
                    {
                        view.AddDefaultButton(DefaultButtonType.New);

                        view.AddRow(row =>
                        {
                            row.AddDefaultButton(DefaultButtonType.Edit);
                            row.AddDefaultButton(DefaultButtonType.Delete);

                            row.AddField(x => x.Name);
                        });
                    });

                    countries.SetNodeEditor(editor =>
                    {
                        editor.AddDefaultButton(DefaultButtonType.SaveExisting);
                        editor.AddDefaultButton(DefaultButtonType.SaveNew);
                        editor.AddDefaultButton(DefaultButtonType.Delete);

                        editor.AddSection(section =>
                        {
                            section.AddField(x => x.Name);
                        });
                    });
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRapidCMS(env.IsDevelopment());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
