using System.Net;
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());

builder.Services.AddMassTransit( x => 
{
   // Add consumer name space
   x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

   // Add the rabbitMq endpoint name formatter
   x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

      // Message retry action for specific endpoint when failed to process the message after consuming (case when Db is not accessible)
      x.UsingRabbitMq((context, cfg) => {

         cfg.ReceiveEndpoint("search-auction-created", e => {

            e.UseMessageRetry(r => r.Interval(5, 5));
            
            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
      });
        
      cfg.ConfigureEndpoints(context);
   });
});

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
   try
   {
      await DbInitializer.InitDb(app);
   }
   catch (System.Exception e)
   {
      Console.WriteLine(e);
   }
});

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetPolicy()
    => HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));

