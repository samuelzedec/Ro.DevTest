using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.ReadModels;

namespace RO.DevTest.Persistence.Repositories;

public class AdminSalesSummaryRepository(DefaultContext context) 
    : BaseRepository<AdminSalesSummary>(context), IAdminSalesSummaryRepository;