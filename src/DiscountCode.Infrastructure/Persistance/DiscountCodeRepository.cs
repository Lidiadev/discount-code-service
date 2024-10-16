using DiscountCode.Domain;
using DiscountCode.Domain.Entities;
using DiscountCode.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiscountCode.Infrastructure.Persistance;

public class DiscountCodeRepository : IDiscountCodeRepository
{
    private readonly ILogger<DiscountCodeRepository> _logger;
    private readonly DiscountDbContext _context;

    public DiscountCodeRepository(ILogger<DiscountCodeRepository> logger, DiscountDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task AddAvailableCodesAsync(IList<AvailableDiscountCode> codes)
    {
        var availableDiscountCodeModels = codes
            .Select(c => new AvailableDiscountCodeModel
            {
                Code = c.Code,
                CreatedAt = c.CreatedAt
            })
            .ToList();
        
        await _context.AvailableDiscountCodes.AddRangeAsync(availableDiscountCodeModels);
        await _context.SaveChangesAsync();
    }

    public async Task<IList<AvailableDiscountCode>> GenerateCodesAsync(int count, int codeLength)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        
        var availableCodes = await _context.AvailableDiscountCodes
            .FromSql($"SELECT * FROM \"AvailableDiscountCodes\" WHERE LENGTH(\"Code\") = {codeLength} ORDER BY \"Id\" LIMIT {count} FOR UPDATE")
            .ToListAsync();

        try
        {
            if (availableCodes.Count != count)
            {
                _logger.LogWarning("Not all requested codes were available.");
                return [];
            }

            var discountCodes = availableCodes
                .Select(ac => new DiscountCodeModel
                {
                    Code = ac.Code,
                    CreatedAt = ac.CreatedAt,
                    IsUsed = false
                })
                .ToList();

            _context.AvailableDiscountCodes.RemoveRange(availableCodes);
            await _context.DiscountCodes.AddRangeAsync(discountCodes);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return availableCodes
                .Select(m => AvailableDiscountCode.Create(m.Code, m.CreatedAt))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving available codes to discount codes.");
            await transaction.RollbackAsync();
            
            return [];
        }
    }
    
    public async Task<bool> MoveToDiscountCodesAsync(IList<string> codes)
    {
        var availableCodes = await _context.AvailableDiscountCodes
            .Where(ac => codes.Contains(ac.Code))
            .ToListAsync();

        if (availableCodes.Count != codes.Count)
        {
            _logger.LogWarning(
                "Not all requested codes were available. Requested: {RequestedCount}, Available: {AvailableCount}",
                codes.Count, availableCodes.Count);
            return false;
        }

        var discountCodes = availableCodes
            .Select(ac => new DiscountCodeModel
            {
                Code = ac.Code,
                CreatedAt = ac.CreatedAt,
                IsUsed = false,
            });

        _context.AvailableDiscountCodes.RemoveRange(availableCodes);
        await _context.DiscountCodes.AddRangeAsync(discountCodes);

        return await SaveChangesAsync();
    }

    public async Task<bool> MarkAsModifiedAsync(Domain.Entities.DiscountCode discountCode)
    {
        var model = await _context.DiscountCodes
            .FirstOrDefaultAsync(dc => dc.Code == discountCode.Code);

        if (model == null)
        {
            return false; 
        }
        
        model.IsUsed = discountCode.IsUsed;
        model.UsedAt = discountCode.UsedAt;

        return await SaveChangesAsync();
    }
    
    private async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}