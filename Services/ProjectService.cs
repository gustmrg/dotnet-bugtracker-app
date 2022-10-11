using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Exception = System.Exception;

namespace BugTracker.Services;

public class ProjectService : IProjectService
{
    private readonly ApplicationDbContext _context;
    private readonly IRolesService _rolesService;

    public ProjectService(ApplicationDbContext context, IRolesService rolesService)
    {
        _context = context;
        _rolesService = rolesService;
    }

    public async Task AddNewProjectAsync(Project project)
    {
        _context.Add(project);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
    {
        var currentProjectManager = await GetProjectManagerAsync(projectId);

        // Remove the current Project Manager
        if (currentProjectManager != null)
        {
            try
            {
                await RemoveProjectManagerAsync(projectId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        // Add the new Project Manager
        try
        {
            await AddUserToProjectAsync(userId, projectId);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user != null)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (!await IsUserOnProjectAsync(userId, projectId))
            {
                try
                {
                    project.Members.Add(user);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public async Task ArchiveProjectAsync(Project project)
    {
        try
        {
            project.Archived = true;
            await UpdateProjectAsync(project);

            // Archive the tickets for the project
            foreach (Ticket ticket in project.Tickets)
            {
                ticket.ArchivedByProject = true;
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public async Task RestoreProjectAsync(Project project)
    {
        try
        {
            project.Archived = false;
            await UpdateProjectAsync(project);

            // Archive the tickets for the project
            foreach (Ticket ticket in project.Tickets)
            {
                ticket.ArchivedByProject = false;
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
    {
        var projects = new List<Project>();
        projects = await _context.Projects.Where(project => project.CompanyId == companyId && project.Archived == false)
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
        return projects;
    }

    public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
    {
        var projects = await GetAllProjectsByCompany(companyId);
        var priorityId = await LookupProjectPriorityId(priorityName);
        return projects.Where(p => p.ProjectPriorityId == priorityId).ToList();
    }

    public async Task<List<ApplicationUser>> GetAllProjectMembersExceptPMAsync(int projectId)
    {
        var developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
        var submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());
        var admins = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString());

        var teamMembers = developers.Concat(submitters).Concat(admins).ToList();

        return teamMembers;
    }

    public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
    {
        var projects = await GetAllProjectsByCompany(companyId);
        return projects.Where(p => p.Archived == true).ToList();
    }

    public async Task<List<ApplicationUser>> GetDevelopersOnProjectAsync(int projectId)
    {
        throw new NotImplementedException();
    }

    public async Task<ApplicationUser> GetProjectManagerAsync(int projectId)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        foreach (var member in project?.Members)
        {
            if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
            {
                return member;
            }
        }

        return null;
    }

    public async Task<List<ApplicationUser>> GetProjectMembersByRoleAsync(int projectId, string role)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        var members = new List<ApplicationUser>();

        foreach (var user in project.Members)
        {
            if (await _rolesService.IsUserInRoleAsync(user, role))
            {
                members.Add(user);
            }
        }

        return members;
    }

    public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
    {
        var project = await _context.Projects
            .Include(p => p.Tickets)
            .Include(p => p.Members)
            .Include(p => p.ProjectPriority)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);
        return project;
    }

    public async Task<List<ApplicationUser>> GetSubmittersOnProjectAsync(int projectId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ApplicationUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
    {
        var users = await _context.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToListAsync();

        return users.Where(u => u.CompanyId == companyId).ToList();
    }

    public async Task<List<Project>> GetUserProjectsAsync(string userId)
    {
        try
        {
            var userProjects = (await _context.Users
                .Include(u => u.Projects)
                    .ThenInclude(p => p.Company)
                .Include(u => u.Projects)
                    .ThenInclude(p => p.Members)
                .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.OwnerUser)
                .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.DeveloperUser)
                .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.TicketPriority)
                .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.TicketStatus)
                .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.TicketType)
                .FirstOrDefaultAsync(u => u.Id == userId)).Projects.ToList();

            return userProjects;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
    {
        var project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

        var result = false;

        if (project != null)
        {
            result = project.Members.Any(m => m.Id == userId);
        }

        return result;
    }

    public async Task<int> LookupProjectPriorityId(string priorityName)
    {
        var priorityId = (await _context.ProjectPriorities.FirstOrDefaultAsync(p => p.Name == priorityName)).Id;
        return priorityId;
    }

    public async Task RemoveProjectManagerAsync(int projectId)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        try
        {
            foreach (var member in project?.Members)
            {
                if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                {
                    await RemoveUserFromProjectAsync(member.Id, projectId);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
    
    public async Task RemoveUserFromProjectAsync(string userId, int projectId)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

            try
            {
                if (await IsUserOnProjectAsync(userId, projectId))
                {
                    project.Members.Remove(user);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
    {
        try
        {
            var members = await GetProjectMembersByRoleAsync(projectId, role);
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

            foreach (var user in members)
            {
                try
                {
                    project.Members.Remove(user);
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

    public async Task UpdateProjectAsync(Project project)
    {
        _context.Update(project);
        await _context.SaveChangesAsync();
    }
}