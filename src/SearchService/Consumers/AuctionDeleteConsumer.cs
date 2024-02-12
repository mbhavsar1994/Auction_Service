using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService;

public class AuctionDeleteConsumer : IConsumer<AuctionDeleted>
{
    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {
        Console.WriteLine($"--> Consuming auction deleted event:: {context.Message.Id} ");

        var id = context.Message.Id;

        var result = await DB.DeleteAsync<Item>(id);

        if(!result.IsAcknowledged)
            throw new MessageException(typeof(AuctionUpdated), "Problem deleting auction");

    }
}
