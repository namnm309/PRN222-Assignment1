using AutoMapper;
using DataAccessLayer.Entities;
using BusinessLayer.ViewModels;

namespace BusinessLayer.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Product mappings
            CreateMap<Product, ProductViewModel>()
                .ForMember(dest => dest.Q, opt => opt.Ignore())
                .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.BrandId))
                .ForMember(dest => dest.MinPrice, opt => opt.Ignore())
                .ForMember(dest => dest.MaxPrice, opt => opt.Ignore())
                .ForMember(dest => dest.InStock, opt => opt.MapFrom(src => src.StockQuantity > 0))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<ProductCreateViewModel, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Brand, opt => opt.Ignore());

            CreateMap<ProductEditViewModel, Product>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Brand, opt => opt.Ignore());

            // Customer mappings
            CreateMap<Customer, CustomerViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<CustomerViewModel, Customer>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // User mappings
            CreateMap<Users, UserCreateViewModel>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore())
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.DealerId, opt => opt.MapFrom(src => src.DealerId));

            CreateMap<UserCreateViewModel, Users>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Dealer, opt => opt.Ignore());

            // UserEditViewModel mappings
            CreateMap<Users, UserEditViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<UserEditViewModel, Users>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.DealerId, opt => opt.Ignore())
                .ForMember(dest => dest.Dealer, opt => opt.Ignore());

            // Brand mappings
            CreateMap<Brand, BrandViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            // Order mappings
            CreateMap<Order, OrderCreateViewModel>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.DealerId, opt => opt.MapFrom(src => src.DealerId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.SalesPersonId, opt => opt.MapFrom(src => src.SalesPersonId))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

            CreateMap<OrderCreateViewModel, Order>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.RegionId, opt => opt.Ignore())
                .ForMember(dest => dest.OrderNumber, opt => opt.Ignore())
                .ForMember(dest => dest.FinalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentStatus, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentMethod, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDate, opt => opt.Ignore())
                .ForMember(dest => dest.DeliveryDate, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentDueDate, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Dealer, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Region, opt => opt.Ignore())
                .ForMember(dest => dest.SalesPerson, opt => opt.Ignore());

            // Feedback mappings
            CreateMap<Feedback, FeedbackViewModel>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment));

            CreateMap<FeedbackViewModel, Feedback>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            // TestDrive mappings
            CreateMap<TestDrive, TestDriveViewModel>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.DealerId, opt => opt.MapFrom(src => src.DealerId))
                .ForMember(dest => dest.ScheduledDate, opt => opt.MapFrom(src => src.ScheduledDate))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.CustomerName, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerPhone, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerEmail, opt => opt.Ignore());

            CreateMap<TestDriveViewModel, TestDrive>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Dealer, opt => opt.Ignore());

            // PurchaseOrder mappings
            CreateMap<PurchaseOrder, PurchaseOrderViewModel>()
                .ForMember(dest => dest.DealerName, opt => opt.MapFrom(src => src.Dealer != null ? src.Dealer.Name : string.Empty))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.RequestedByName, opt => opt.MapFrom(src => src.RequestedBy != null ? src.RequestedBy.FullName : string.Empty))
                .ForMember(dest => dest.ApprovedByName, opt => opt.MapFrom(src => src.ApprovedBy != null ? src.ApprovedBy.FullName : string.Empty));

            // User mappings
            CreateMap<Users, UserViewModel>()
                .ForMember(dest => dest.DealerName, opt => opt.MapFrom(src => src.Dealer != null ? src.Dealer.Name : string.Empty));

            // Dealer mappings
            CreateMap<Dealer, DealerViewModel>()
                .ForMember(dest => dest.RegionName, opt => opt.MapFrom(src => src.Region != null ? src.Region.Name : string.Empty));

            // PricingPolicy mappings
            CreateMap<PricingPolicy, PricingPolicyViewModel>();

            // InventoryAllocation mappings
            CreateMap<InventoryAllocation, InventoryAllocationViewModel>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.DealerName, opt => opt.MapFrom(src => src.Dealer != null ? src.Dealer.Name : string.Empty));

            // DealerContract mappings
            CreateMap<DealerContract, DealerContractViewModel>()
                .ForMember(dest => dest.DealerName, opt => opt.MapFrom(src => src.Dealer != null ? src.Dealer.Name : string.Empty))
                .ForMember(dest => dest.RegionName, opt => opt.Ignore());
        }
    }
}
