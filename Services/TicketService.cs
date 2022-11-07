using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services;

public class TicketService : ITicketService
{
    private readonly ApplicationDbContext _context;
    private readonly IRolesService _rolesService;
    private readonly IProjectService _projectService;

    public TicketService(ApplicationDbContext context, IRolesService rolesService, IProjectService projectService)
    {
        _context = context;
        _rolesService = rolesService;
        _projectService = projectService;
    }

    public async Task AddNewTicketAsync(Ticket ticket)
    {
        try
        {
            _context.Add(ticket);
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task UpdateTicketAsync(Ticket ticket)
    {
        try
        {
            _context.Update(ticket);
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<Ticket> GetTicketByIdAsync(int ticketId)
    {
        try
        {
            return await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task ArchiveTicketAsync(Ticket ticket)
    {
        try
        {
            ticket.Archived = true;
            _context.Update(ticket);
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task AssignTicketAsync(int ticketId, string userId)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

        try
        {
            if (ticket != null)
            {
                try
                {
                    ticket.DeveloperUserId = userId;
                    ticket.TicketStatusId = (await LookupTicketStatusIdAsync("Development")).Value;
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<Ticket>> GetArchivedTicketsAsync(int companyId)
    {
        try
        {
            var tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(t => t.Archived == true).ToList();
            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
    {
        try
        {
            var tickets = await _context.Projects
                .Where(p => p.CompanyId == companyId)
                .SelectMany(p => p.Tickets)
                .Include(t => t.Attachments)
                .Include(t => t.Comments)
                .Include(t => t.DeveloperUser)
                .Include(t => t.History)
                .Include(t => t.OwnerUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .Include(t => t.Project)
                .ToListAsync();
            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
    {
        int priorityId = (await LookupTicketPriorityIdAsync(priorityName)).Value;

        try
        {
            var tickets = await _context.Projects
                .Where(p => p.CompanyId == companyId)
                .SelectMany(p => p.Tickets)
                .Include(t => t.Attachments)
                .Include(t => t.Comments)
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .Include(t => t.Project)
                .Where(t => t.TicketPriorityId == priorityId)
                .ToListAsync();
            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
    {
        int statusId = (await LookupTicketStatusIdAsync(statusName)).Value;

        try
        {
            var tickets = await _context.Projects
                .Where(p => p.CompanyId == companyId)
                .SelectMany(p => p.Tickets)
                .Include(t => t.Attachments)
                .Include(t => t.Comments)
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .Include(t => t.Project)
                .Where(t => t.TicketStatusId == statusId)
                .ToListAsync();
            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
    {
        int typeId = (await LookupTicketTypeIdAsync(typeName)).Value;

        try
        {
            var tickets =  await _context.Projects
                .Where(p => p.CompanyId == companyId)
                .SelectMany(p => p.Tickets)
                .Include(t => t.Attachments)
                .Include(t => t.Comments)
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .Include(t => t.Project)
                .Where(t => t.TicketTypeId == typeId)
                .ToListAsync();
            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ApplicationUser> GetTicketDeveloperAsync(int ticketId, int companyId)
    {
        var developer = new ApplicationUser();

        try
        {
            var ticket = (await GetAllTicketsByCompanyAsync(companyId)).FirstOrDefault(t => t.Id == ticketId);

            if (ticket?.DeveloperUserId != null)
            {
                developer = ticket.DeveloperUser;
            }

            return developer;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId)
    {
        var tickets = new List<Ticket>();

        try
        {
            if (role == Roles.Admin.ToString())
            {
                tickets = await GetAllTicketsByCompanyAsync(companyId);
            }
            else if (role == Roles.Developer.ToString())
            {
                tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(t => t.DeveloperUserId == userId).ToList();
            }
            else if (role == Roles.Submitter.ToString())
            {
                tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(t => t.OwnerUserId == userId).ToList();
            }
            else if (role == Roles.ProjectManager.ToString())
            {
                tickets = await GetTicketsByUserIdAsync(userId, companyId);
            }

            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
    {
        var applicationUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        var tickets = new List<Ticket>();

        try
        {
            if (await _rolesService.IsUserInRoleAsync(applicationUser, Roles.Admin.ToString()))
            {
                tickets = (await _projectService.GetAllProjectsByCompany(companyId)).SelectMany(p => p.Tickets).ToList();
            }
            else if (await _rolesService.IsUserInRoleAsync(applicationUser, Roles.Developer.ToString()))
            {
                tickets = (await _projectService.GetAllProjectsByCompany(companyId)).SelectMany(p => p.Tickets).Where(t => t.DeveloperUserId == userId).ToList();
            }
            else if (await _rolesService.IsUserInRoleAsync(applicationUser, Roles.Submitter.ToString()))
            {
                tickets = (await _projectService.GetAllProjectsByCompany(companyId)).SelectMany(p => p.Tickets).Where(t => t.OwnerUserId == userId).ToList();
            }
            else if (await _rolesService.IsUserInRoleAsync(applicationUser, Roles.ProjectManager.ToString()))
            {
                tickets = (await _projectService.GetUserProjectsAsync(userId)).SelectMany(t => t.Tickets).ToList();
            }

            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId)
    {
        var tickets = new List<Ticket>();

        try
        {
            tickets = (await GetTicketsByRoleAsync(role, userId, companyId)).Where(t => t.ProjectId == projectId).ToList();
            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
    {
        var tickets = new List<Ticket>();

        try
        {
            tickets = (await GetAllTicketsByStatusAsync(companyId, statusName)).Where(t => t.ProjectId == projectId).ToList();
            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId)
    {
        var tickets = new List<Ticket>();

        try
        {
            tickets = (await GetAllTicketsByPriorityAsync(companyId, priorityName)).Where(t => t.ProjectId == projectId).ToList();
            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
    {
        var tickets = new List<Ticket>();

        try
        {
            tickets = (await GetAllTicketsByTypeAsync(companyId, typeName)).Where(t => t.ProjectId == projectId).ToList();
            return tickets;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
    {
        try
        {
            var priority = await _context.TicketPriorities.FirstOrDefaultAsync(p => p.Name == priorityName);
            return priority?.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<int?> LookupTicketStatusIdAsync(string statusName)
    {
        try
        {
            var status = await _context.TicketStatuses.FirstOrDefaultAsync(p => p.Name == statusName);
            return status?.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<int?> LookupTicketTypeIdAsync(string typeName)
    {
        try
        {
            var type = await _context.TicketTypes.FirstOrDefaultAsync(p => p.Name == typeName);
            return type?.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}