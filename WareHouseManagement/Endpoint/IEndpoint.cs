namespace WareHouseManagement.Endpoint
{
    public interface IEndpoint
    {
        static abstract void MapEndpoint(IEndpointRouteBuilder app);
    }
}
