using AutoMapper;
using PersonalFinanceManagementAPI.Commands;
using PersonalFinanceManagementAPI.Database.Entities;
using PersonalFinanceManagementAPI.Models;

namespace PersonalFinanceManagementAPI.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //transactions
            CreateMap<TransactionsEntity, Transactions>()
                .ForMember(p => p.Id, e => e.MapFrom(x => x.Id));

            CreateMap<Transactions, TransactionsEntity>()
                .ForMember(et => et.Id, p => p.MapFrom(x => x.Id));

            CreateMap<TransactionsPagedSortedList<TransactionsEntity>, TransactionsPagedSortedList<Transactions>>();

            CreateMap<CsvLineTransactionsCommand, TransactionsEntity>()
                .ForMember(et => et.Id, p => p.MapFrom(x => x.Id));


            //categories
            CreateMap<CategoriesEntity, Categories>()
                .ForMember(p => p.Code, e => e.MapFrom(x => x.Code));

            CreateMap<Categories, CategoriesEntity>()
                .ForMember(et => et.Code, p => p.MapFrom(x => x.Code));

            CreateMap<CategoriesPagedSortedList<CategoriesEntity>, CategoriesPagedSortedList<Categories>>();

            CreateMap<CsvLineCategoriesCommand, CategoriesEntity>()
                .ForMember(et => et.Code, p => p.MapFrom(x => x.Code));


             //SpendingInCategory

            CreateMap<SpendingInCategoryEntity, SpendingInCategory>()
                .ForMember(p => p.Catcode, e => e.MapFrom(x => x.Catcode))
                .ForMember(p => p.Amount, e => e.MapFrom(x => x.Amount))
                .ForMember(p => p.Count, e => e.MapFrom(x => x.Count));

            CreateMap<SpendingInCategory, SpendingInCategoryEntity>()
                .ForMember(et => et.Catcode, p => p.MapFrom(x => x.Catcode))
                .ForMember(et => et.Amount, p => p.MapFrom(x => x.Amount))
                .ForMember(et => et.Count, p => p.MapFrom(x => x.Count));


            //SpendingsByCategoryPagedSortedList



            CreateMap<SpendingsByCategoryPagedSortedList<SpendingInCategoryEntity>, SpendingsByCategoryPagedSortedList<SpendingInCategory>>()
               .ForMember(dest => dest.Groups, opt => opt.MapFrom(src => src.Groups));

            CreateMap<SpendingsByCategoryPagedSortedList<SpendingInCategory>, SpendingsByCategoryPagedSortedList<SpendingInCategoryEntity>>()
                .ForMember(dest => dest.Groups, opt => opt.MapFrom(src => src.Groups));

            CreateMap<SpendingInCategoryEntity, SpendingInCategory>();
            CreateMap<SpendingInCategory, SpendingInCategoryEntity>();


            //splits

            /*
            CreateMap<SplitsEntity, Splits>()
                 .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                 .ForMember(dest => dest.Catcode, opt => opt.MapFrom(src => src.Catcode))
                 .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<Splits, SplitsEntity>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.Catcode, opt => opt.MapFrom(src => src.Catcode))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            */
            CreateMap<SplitsEntity, Splits>();

            CreateMap<Splits, SplitsEntity>();

            CreateMap<SplitTransCommand, SplitsEntity>()
               .ForMember(et => et.Catcode, p => p.MapFrom(x => x.Splits));






        }

    }
}
