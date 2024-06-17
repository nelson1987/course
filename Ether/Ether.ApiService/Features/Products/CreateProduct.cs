using AutoMapper;
using Ether.ApiService.Contexts;
using Ether.ApiService.Features.Shared;
using Ether.ServiceDefaults.Entities;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Ether.ApiService.Features.Products;

public static class CreateProduct
{
    public record Request(string FirstName, string LastName, decimal Price);
    public record Response(int Id, string Name, decimal Price);
    public record Event(string FirstName, string LastName, decimal Price);
    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).MaximumLength(100);
            RuleFor(x => x.Price).GreaterThan(0);
        }
    }
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).MaximumLength(100);
            RuleFor(x => x.Price).GreaterThan(0);
        }
    }
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Request, Event>();
            CreateMap<Event, Product>();
            CreateMap<Product, Response>();
        }
    }
    public class Endpoint
    {
        public static void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v1/products", Get)
                .WithTags("Products");
            app.MapPost("api/v1/products", Post)
                .WithTags("Products");
        }

        public static async Task<IResult> Get(ApplicationDbContext context,
            CancellationToken cancellationToken)
        {
            return Results.Ok(await context.Products.ToListAsync());
        }

        public static async Task<IResult> Post(Request request,
        IValidator<Request> validator,
        IBus bus,
        IMongoClient client,
        CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            var collection = client.GetDatabase("mongodb").GetCollection<Request>("Requests");
            //IMongoCollection<Request> document = 
            await collection.InsertOneAsync(request, cancellationToken);

            var evento = request.MapTo<Event>();
            await bus.Publish(evento);

            return Results.Created();
        }
    }
    public class Dependencies
    {
        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IValidator<Request>, Validator>();
            services.AddScoped<IValidator<Product>, ProductValidator>();
        }
    }
    public class EventConsumer : IConsumer<Event>
    {
        private readonly ApplicationDbContext _yourDbContext;

        private readonly IValidator<Product> _validator;
        public EventConsumer(ApplicationDbContext yourDbContext, IValidator<Product> validator)
        {
            _yourDbContext = yourDbContext;
            _validator = validator;
        }

        public async Task Consume(ConsumeContext<Event> context)
        {
            var message = context.Message;
            var product = message.MapTo<Product>();

            var validationResult = await _validator.ValidateAsync(product);
            if (!validationResult.IsValid)
            {
                Task.FromResult(validationResult.Errors);
            }
            product.isActive = true;
            await _yourDbContext.Products.AddAsync(product);
            await _yourDbContext.SaveChangesAsync();
        }
    }
}