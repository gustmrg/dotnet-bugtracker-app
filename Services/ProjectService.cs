using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services;

public class ProjectService : IProjectService
{
    private readonly ApplicationDbContext _context;
    public ProjectService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddNewProjectAsync(Project project)
    {
        _context.Add(project);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
    {
        throw new NotImplementedException();
    }

    public async Task ArchiveProjectAsync(Project project)
    {
        project.Archived = true;
        _context.Update(project);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ApplicationUser>> GetAllProjectMembersExceptPMAsync(int projectId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ApplicationUser>> GetDevelopersOnProjectAsync(int projectId)
    {
        throw new NotImplementedException();
    }

    public async Task<ApplicationUser> GetProjectManagerAsync(int projectId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ApplicationUser>> GetProjectMembersByRoleAsync(int projectId, string role)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public async Task<List<Project>> GetUserProjectsAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsUserOnProject(string userId, int projectId)
    {
        throw new NotImplementedException();
    }

    public async Task<int> LookupProjectPriorityId(string priorityName)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveProjectManagerAsync(int projectId)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveUserFromProjectAsync(string userId, int projectId)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateProjectAsync(Project project)
    {
        _context.Update(project);
        await _context.SaveChangesAsync();
    }
}