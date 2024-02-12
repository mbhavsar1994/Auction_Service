using Contracts;
using MassTransit;

namespace AuctionService;

public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{
    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        Console.WriteLine("-->consuming faulty creation :");

        var exception = context.Message.Exceptions.First();

        if (exception.ExceptionType == "System.ArgumentException") 
        {
             Console.WriteLine(exception.Message); 

             context.Message.Message.Model = "foobar";

             await context.Publish(context.Message.Message);
        } 
        else
        {
            Console.WriteLine("Unhandled exception -- Log error to dashbord to alert");
        }
    }
}
