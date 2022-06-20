using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services;

public class CompanyInfoService : ICompanyInfoService
{
    private readonly ApplicationDbContext _context;
    
    public  CompanyInfoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
    {
        var result = new Company();
        if (companyId != null)
        {
            result = await _context.Companies
                .Include(company => company.Members)
                .Include(company => company.Projects)
                .Include(company => company.Invites)
                .FirstOrDefaultAsync(company => company.Id == companyId);
        }

        return result;
    }

    public async Task<List<ApplicationUser>> GetAllMembersAsync(int companyId)
    {
        var result = new List<ApplicationUser>();
        result = await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync();
        return result;
    }

    public async Task<List<Project>> GetAllProjectsAsync(int companyId)
    {
        var result = new List<Project>();
        result = await _context.Projects.Where(project => project.CompanyId == companyId)
            .Include(project => project.Members)
            .Include(project => project.Tickets)
                .ThenInclude(ticket => ticket.Comments)
            .Include(project => project.Tickets)
                .ThenInclude(ticket => ticket.Attachments)
            .Include(project => project.Tickets)
                .ThenInclude(ticket => ticket.History)
            .Include(project => project.Tickets)
                .ThenInclude(ticket => ticket.Notifications)
            .Include(project => project.Tickets)
                .ThenInclude(ticket => ticket.DeveloperUser)
            .Include(project => project.Tickets)
                .ThenInclude(ticket => ticket.OwnerUser)
            .Include(project => project.Tickets)
                .ThenInclude(ticket => ticket.TicketStatus)
            .Include(project => project.Tickets)
                .ThenInclude(ticket => ticket.TicketPriority)
            .Include(project => project.Tickets)
                .ThenInclude(ticket => ticket.TicketType)
            .Include(project => project.ProjectPriority)
            .ToListAsync();
        return result;
    }

    public async Task<List<Ticket>> GetAllTicketsAsync(int companyId)
    {
        var result = new List<Ticket>();
        var projects = new List<Project>();
        projects = await GetAllProjectsAsync(companyId);
        result = projects.SelectMany(project => project.Tickets).ToList();
        return result;
    }
}