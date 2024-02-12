using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    private IMapper _mapper;
    public AuctionCreatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }
    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine($"--Consuming auction is created {context.Message.Id}");

        var item = _mapper.Map<Item>(context.Message);

        if (item.Model == "foo")
            throw new ArgumentException("can not sell car name foo");

        await item.SaveAsync();
    }
}
