﻿using DiscountCode.Domain;
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

    public async Task<IList<AvailableDiscountCode>> GetAvailableCodesAsync(int count)
    {
        var availableDiscountCodeModels = await _context.AvailableDiscountCodes
            .OrderBy(m => m.Id)
            .Take(count)
            .ToListAsync();
        
        _logger.LogInformation("Getting available discount codes: {Count}", availableDiscountCodeModels.Count);

        return availableDiscountCodeModels
            .Select(m => AvailableDiscountCode.Create(m.Code, m.CreatedAt))
            .ToList();
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
        
        _logger.LogInformation("Adding available discount codes: {Count}", availableDiscountCodeModels.Count);
        await _context.AvailableDiscountCodes.AddRangeAsync(availableDiscountCodeModels);
        
        await _context.SaveChangesAsync();
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

    public async Task<Domain.Entities.DiscountCode> GetDiscountCodeAsync(string code)
    {
        var model = await _context.DiscountCodes
            .FirstOrDefaultAsync(dc => dc.Code == code);

        if (model == null)
        {
            return null; 
        }

        var discountCode = Domain.Entities.DiscountCode.Create(model.Code, model.CreatedAt);
        if (model is { IsUsed: true, UsedAt: not null })
        {
            discountCode.MarkAsUsed(model.UsedAt.Value);
        }

        return discountCode;
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