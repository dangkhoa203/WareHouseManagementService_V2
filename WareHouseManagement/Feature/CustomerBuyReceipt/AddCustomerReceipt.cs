using WareHouseManagement.Endpoint;

namespace WareHouseManagement.Feature.CustomerBuyReceipt {
    public class AddCustomerReceipt : IEndpoint {
        public record Request();
        public record Response();
        public static void MapEndpoint(IEndpointRouteBuilder app) {
            throw new NotImplementedException();
        }
    }
}
