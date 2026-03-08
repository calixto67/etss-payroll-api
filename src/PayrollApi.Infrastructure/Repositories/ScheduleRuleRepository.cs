using Microsoft.EntityFrameworkCore;
using PayrollApi.Domain.Entities;
using PayrollApi.Domain.Interfaces.Repositories;
using PayrollApi.Infrastructure.Data;

namespace PayrollApi.Infrastructure.Repositories;

public class ScheduleRuleRepository : BaseRepository<ScheduleRule>, IScheduleRuleRepository
{
    public ScheduleRuleRepository(AppDbContext context) : base(context) { }

    public async Task<ScheduleRule> GetRuleAsync(CancellationToken cancellationToken = default)
    {
        var rule = await _dbSet.FirstOrDefaultAsync(cancellationToken);
        if (rule is not null) return rule;

        rule = new ScheduleRule { CreatedBy = "system" };
        await _dbSet.AddAsync(rule, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return rule;
    }
}
